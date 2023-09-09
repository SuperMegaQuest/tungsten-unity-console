using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Monolith;
using Monolith.Web;

using Newtonsoft.Json;

using UnityEngine;

public class WebView : ConsoleView {

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
    /// </summary>
    private readonly Queue<ConsoleLog> _logQueue = new();

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

                switch (request.HttpMethod) {
                    case "GET": {
                        string requestedPath = request.Url.AbsolutePath;
                        string filePath = Path.Combine(_dataPath, requestedPath.TrimStart('/'));

                        switch (requestedPath) {
                            case "/":
                                ServeFile(response, Path.Combine(_dataPath, "index.html"));
                                break;
                            case "/api/console":
                                ServeConsoleData(response);
                                break;
                            case "/api/logs":
                                ServeNewLogs(response);
                                break;
                            // case "/api/events":
                            //     ServeSSE(context);
                            //     break;
                            default: {
                                if (File.Exists(filePath)) {
                                    ServeFile(response, filePath);
                                }
                                else {
                                    response.StatusCode = 404;
                                    response.StatusDescription = "File not found.";
                                    response.Close();
                                }

                                break;
                            }
                        }

                        break;
                    }

                    case "POST": {
                        HandlePostRequest(request);
                        break;
                    }
                }
            }
            catch (HttpListenerException) {
                // listener was stopped, exit the loop
                break;
            }
            catch (InvalidOperationException) {
                // listener was stopped, exit the loop
                break;
            }
        }
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

        private void HandlePostRequest(HttpListenerRequest request) {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            string content = reader.ReadToEnd();
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            Debug.Log($"Received POST request: {data["text"]}");
        }

        private void ServeConsoleData(HttpListenerResponse response) {
            const string consoleData = @"{ ""data"": ""Hello from Unity"" }";

            byte[] buffer = Encoding.UTF8.GetBytes(consoleData);
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        [Serializable]
        private struct NewLogsResponse {

            public ConsoleLog[] logs;

        }

        private void ServeNewLogs(HttpListenerResponse response) {
            NewLogsResponse outputObj = new() {
                logs = _logQueue.ToArray(),
            };
            _logQueue.Clear();
            string outStr = JsonConvert.SerializeObject(outputObj, new ColorJsonConverter());

            // if(_logQueue.Count > 0) {
            //     ConsoleLog log = _logQueue.Dequeue();
            //     string serializedLog = JsonConvert.SerializeObject(log, new ColorJsonConverter());
            //     // outputString = $"{{ \"data\": {serializedLog} }}";
            //     outputString = serializedLog;
            // }
            // else {
            //     outputString = @"{ ""data"": ""No new logs"" }";
            // }

            byte[] buffer = Encoding.UTF8.GetBytes(outStr);
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        // private async void ServeSSE(HttpListenerContext context) {
        //     Debug.Log("[Tungsten] [WebView] SSE client connected");
        //
        //     context.Response.StatusCode = 200;
        //     context.Response.ContentType = "text/event-stream";
        //     context.Response.Headers.Add("Cache-Control", "no-cache");
        //     context.Response.Headers.Add("Connection", "keep-alive");
        //
        //     // keep alive
        //     while (true) {
        //         Debug.Log($"{_logQueue.Count} logs in the queue");
        //
        //         if (_logQueue.Count > 0) {
        //             ConsoleLog log = _logQueue.Dequeue();
        //
        //             Debug.Log($"Processing log {log.logString}");
        //
        //             string serializedLog = JsonConvert.SerializeObject(log);
        //
        //             byte[] message = Encoding.UTF8.GetBytes($"data: {serializedLog}\n\n");
        //             // byte[] message = Encoding.UTF8.GetBytes($"data: {DateTime.Now}\n\n");
        //             await context.Response.OutputStream.WriteAsync(message, 0, message.Length);
        //             await context.Response.OutputStream.FlushAsync();
        //         }
        //
        //         await Task.Delay(100);
        //
        //         // byte[] message = Encoding.UTF8.GetBytes($"data: {DateTime.Now}\n\n");
        //         // await context.Response.OutputStream.WriteAsync(message, 0, message.Length);
        //         // await context.Response.OutputStream.FlushAsync();
        //         // await Task.Delay(100);
        //     }
        // }

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

        // private void SendMessage(string message) {
        //     byte[] data = Encoding.UTF8.GetBytes($"data: {message}\n\n");
        //
        //     foreach (HttpListenerResponse response in _eventStreams.Values) {
        //         response.OutputStream.Write(data, 0, data.Length);
        //         response.OutputStream.Flush();
        //     }
        // }
#endregion Private

}