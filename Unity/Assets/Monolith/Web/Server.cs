using System;
using System.IO;
using System.Net;
using System.Threading;

using UnityEngine;

public class Server : MonoBehaviour {

    private HttpListener _listener;
    private Thread _listenerThread;
    private string _dataPath;

    private void Start() {
        _dataPath = $"{Application.dataPath}/www";

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://*:8080/"); // Bind to all IPs on the machine, port 8080.

        _listenerThread = new Thread(StartListener);
        _listenerThread.Start();
    }

    private void OnApplicationQuit() {
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

                if (File.Exists(filePath)) {
                    ServeFile(response, filePath);
                }
                else {
                    response.StatusCode = 404;
                    response.StatusDescription = "File not found.";
                    response.Close();
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

    private static string GetMimeType(string filePath) {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            _ => "application/octet-stream",
        };
    }

}