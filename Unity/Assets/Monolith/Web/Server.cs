using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

using Newtonsoft.Json;

using UnityEngine;

public class Server : MonoBehaviour {

    private HttpListener _listener;
    private Thread _listenerThread;
    private string _dataPath;

    private Dictionary<string, HttpListenerResponse> _eventStreams = new();

    // private float _last = 0.0f;

    private void Start() {
        _dataPath = $"{Application.dataPath}/www";

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://*:8080/"); // Bind to all IPs on the machine, port 8080.

        _listenerThread = new Thread(StartListener);
        _listenerThread.Start();
    }

    private void OnDestroy() {
        Cleanup();
    }

    private void OnApplicationQuit() {
        Cleanup();
    }

    // private void Update() {
    //     _last += Time.deltaTime;
    //     if (_last >= 5.0f) {
    //         SendMessage($"Hello, world! {_last.ToString()}");
    //         _last = 0.0f;
    //     }
    // }

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

                        if (requestedPath == "/api/console") {
                            ServeConsoleData(response);
                        }
                        else if (File.Exists(filePath)) {
                            ServeFile(response, filePath);
                        }
                        else {
                            response.StatusCode = 404;
                            response.StatusDescription = "File not found.";
                            response.Close();
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
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)) {
            string content = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            Debug.Log($"Received POST request: {data["text"]}");
        }
    }

    private void ServeConsoleData(HttpListenerResponse response) {
        const string consoleData = @"{ ""data"": ""Hello from Unity"" }";

        byte[] buffer = Encoding.UTF8.GetBytes(consoleData);
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

}