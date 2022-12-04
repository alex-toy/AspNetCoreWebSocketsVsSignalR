using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace WebSockerServer.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //WriteRequestParam(context);
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");

                string ConnID = _manager.AddSocket(webSocket);
                await SendConnIDAsync(webSocket, ConnID);

                await ReceiveMessage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"message received : {message}");
                        await RouteJSONMessageAsync(message);
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("receive close message");
                        var sockets = _manager.GetAllSockets();
                        string id = sockets.FirstOrDefault(s => s.Value == webSocket).Key;
                        sockets.TryRemove(id, out WebSocket sock);

                        await sock.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                        return;
                    }
                });
            }
            else
            {
                Console.WriteLine("hello from the second request delegate");
                await _next(context);
            }
        }

        private async Task SendConnIDAsync(WebSocket socket, string connID)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnID: {connID}");
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        public async Task RouteJSONMessageAsync(string message)
        {
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);

            if (Guid.TryParse(routeOb.To.ToString(), out Guid guidOutput))
            {
                Console.WriteLine($"message to {guidOutput}");
                var sock = _manager.GetAllSockets().FirstOrDefault(s => s.Key == routeOb.To.ToString());

                if (sock.Value != null)
                {
                    if (sock.Value.State == WebSocketState.Open)
                    {
                        dynamic stringMessage = Encoding.UTF8.GetBytes(routeOb.Message.ToString());
                        await sock.Value.SendAsync(stringMessage, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid recipient");
                }
            }
            else
            {
                Console.WriteLine("broadcast");
                foreach(var sock in _manager.GetAllSockets())
                {
                    if(sock.Value.State == WebSocketState.Open)
                    {
                        dynamic stringMessage = Encoding.UTF8.GetBytes(routeOb.Message.ToString());
                        await sock.Value.SendAsync(stringMessage, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }
    }
}
