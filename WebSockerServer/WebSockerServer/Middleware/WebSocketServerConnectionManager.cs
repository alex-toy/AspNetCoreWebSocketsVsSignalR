using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSockerServer.Middleware
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _socket = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _socket;
        }

        public string AddSocket(WebSocket socket)
        {
            string ConnID = Guid.NewGuid().ToString();
            _socket.TryAdd(ConnID, socket);
            Console.WriteLine($"connection added : {ConnID}");

            return ConnID;
        }
    }
}
