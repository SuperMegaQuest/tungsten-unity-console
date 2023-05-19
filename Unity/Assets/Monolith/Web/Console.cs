using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

using Monolith;

using UnityEngine;

public class Console : ConsoleView {

    private struct RequestContext {

        public HttpListenerContext context;
        public Match match;
        public bool pass;
        public string path;
        public int currentRoute;

        public HttpListenerRequest Request => context.Request;
        public HttpListenerResponse Response => context.Response;

        public RequestContext(HttpListenerContext context) {
            this.context = context;
            this.match = null;
            this.pass = false;
            this.path = WWW.EscapeURL(context.Request.Url.AbsolutePath);

            if (this.path == "/") {
                this.path = "/index.html";
            }
            this.currentRoute = 0;
        }

    }

#region Properties
    public override bool IsActive { get; protected set; }
#endregion Properties

#region Fields
    // private const string SERVER_URL = "http://localhost:8080/";
    private const ushort PORT = 55055;

    private readonly Dictionary<string, string> _fileTypes = new() {
        {"js",   "application/javascript"},
        {"png",  "image/png"},
        {"css",  "text/css"},
        {"htm",  "text/html"},
        {"html", "text/html"},
        {"ico",  "image/x-icon"},
    };

    private HttpListener _listener = null;
    private Queue<RequestContext> _mainThreadRequestQueue = new();
#endregion Fields

#region Private Methods
    protected override void Awake() {
        base.Awake();

        StartServer();
    }

    private void StartServer() {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://*:{PORT}/");
        _listener.Start();
        _listener.BeginGetContext(ListenerCallback, null);
    }

    private void ListenerCallback(IAsyncResult result) {
        var context = new RequestContext(_listener.EndGetContext(result));
        HandleRequest(context);

        // TODO: Why?
        if (_listener.IsListening) {
            _listener.BeginGetContext(ListenerCallback, null);
        }
    }

    private IEnumerator HandleMainThreadRequests() {
        while (true) {
            while (_mainThreadRequestQueue.Count == 0) {
                yield return new WaitForEndOfFrame();
            }

            lock (_mainThreadRequestQueue) {
                HandleRequest(_mainThreadRequestQueue.Dequeue());
            }
        }
    }

    private void HandleRequest(RequestContext context) {

    }
#endregion Private Methods

}