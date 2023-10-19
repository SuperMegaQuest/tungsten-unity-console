using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Monolith;
using Monolith.Web;

using Newtonsoft.Json;

using UnityEngine;

using Console = Monolith.Console;
using Debug = UnityEngine.Debug;

public class WebView : ConsoleView {

#region Types
    [Serializable]
    private struct NewLogsResponse {
        public ConsoleLog[] logs;
    }
#endregion Types

#region Public
    public override bool IsActive { get; protected set; }

    public override void ClearConsoleView() {
        base.ClearConsoleView();
    }
#endregion Public

#region Protected
    protected override void Awake() {
        base.Awake();

        IsActive = true;

        _dataPath = $"{Application.dataPath}/www";

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://*:8080/"); // Bind to all IPs on the machine, port 8080.

        _listenerThread = new Thread(StartListener);
        _listenerThread.Start();
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        Cleanup();
    }

    protected override void OnConsoleLogHistoryChanged() {
        base.OnConsoleLogHistoryChanged();

        _logQueue.Enqueue(Monolith.Console.ConsoleHistory.LatestLog);
    }
#endregion Protected

#region Private
    private HttpListener _listener;
    private Thread _listenerThread;
    private string _dataPath;

    /// <summary>
    /// Queue of logs that have been added since the last time the queue was processed.
    /// Should we store this per client?
    /// </summary>
    private readonly Queue<ConsoleLog> _logQueue = new();

    private Queue<Action> _actionQueue = new();

    private void Update() {
        // process action queue
        while (_actionQueue.Count > 0) {
            _actionQueue.Dequeue().Invoke();
        }
    }

    private void Cleanup() {
        _listener.Stop();
        _listenerThread.Abort();
    }

    private void StartListener() {
        _listener.Start();

        while (true) {
            try {
                HttpListenerContext context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string requestedPath = request.Url.AbsolutePath;
                string filePath = Path.Combine(_dataPath, requestedPath.TrimStart('/'));

                // Dictionary<(string path, string method), Action> blorp = new() {
                //     { ("/", "GET"), () => { ServeFile(response, Path.Combine(_dataPath, "index.html")); } },
                //     { ("/", "GET"), () => { ServeFile(response, Path.Combine(_dataPath, "index.html")); } },
                // };
                //
                // Action requestHandler = blorp[(request.Url.AbsolutePath, request.HttpMethod)];
                // requestHandler?.Invoke();

                Action action;
                switch (request) {
                    case { IsWebSocketRequest: true }:
                        action = () => HandleWebSocketRequest(context, request);
                        break;
                    case { HttpMethod: "GET", Url: { AbsolutePath: "/" } }:
                        action = () => ServeFile(response, Path.Combine(_dataPath, "index.html"));
                        break;
                    case { HttpMethod: "GET", Url: { AbsolutePath: "/log" } }:
                        action = () => ServeNewLogs(response);
                        break;
                    case { HttpMethod: "POST", Url: { AbsolutePath: "/command" } }:
                        action = () => HandleCommand(request);
                        break;
                    case { HttpMethod: "GET" }:
                        action = () => {
                            if (File.Exists(filePath)) {
                                ServeFile(response, filePath);
                            }
                            else {
                                response.StatusCode = 404;
                                response.StatusDescription = "File not found.";
                                response.Close();
                            }
                        };
                        break;
                    default:
                        action = null;
                        break;
                }

                action?.Invoke();
            }
            catch (HttpListenerException e) {
                // listener was stopped, exit the loop
                Debug.LogError($"[Console] [WebView] HttpListenerException: {e.Message}");
                break;
            }
            catch (InvalidOperationException e) {
                // listener was stopped, exit the loop
                Debug.LogError($"[Console] [WebView] InvalidOperationException: {e.Message}");
                break;
            }
        }
    }

    private async void HandleWebSocketRequest(HttpListenerContext context,  HttpListenerRequest request) {
        HttpListenerWebSocketContext webSocket = await context.AcceptWebSocketAsync(null);
        HandleWebSocket(webSocket.WebSocket);
    }

    private async void HandleWebSocket(WebSocket webSocket) {
        var buffer = new byte[1024];
        WebSocketReceiveResult result;

        do {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        } while (result.CloseStatus.HasValue == false);
    }

    private void HandleCommand(HttpListenerRequest request) {
        using StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string content = reader.ReadToEnd();
        Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

        _actionQueue.Enqueue(() => { Console.ExecuteCommand(data["command"]); });
    }

    private static void ServeFile(HttpListenerResponse response, string filePath) {
        string mimeType = GetMimeType(filePath);
        byte[] fileBytes = File.ReadAllBytes(filePath);

        response.ContentType = mimeType;
        response.ContentLength64 = fileBytes.Length;
        Stream output = response.OutputStream;
        output.Write(fileBytes, 0, fileBytes.Length);
        output.Close();
    }

    private void ServeNewLogs(HttpListenerResponse response) {
        NewLogsResponse outputObj = new() {
            logs = _logQueue.ToArray(),
        };
        _logQueue.Clear();
        string outStr = JsonConvert.SerializeObject(outputObj, new ColorJsonConverter());

        byte[] buffer = Encoding.UTF8.GetBytes(outStr);
        response.ContentLength64 = buffer.Length;

        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    private static string GetMimeType(string filePath) {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".svg" => "image/svg+xml",
            ".gif" => "image/gif",
            ".jpeg" => "image/jpeg",
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream",
        };
    }
#endregion Private

}