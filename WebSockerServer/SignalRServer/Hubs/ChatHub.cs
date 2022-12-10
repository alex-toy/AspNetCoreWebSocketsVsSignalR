using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"connection established : {Context.ConnectionId}");
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);
            Console.WriteLine($"Message received on {Context.ConnectionId}");
            string toClient = routeOb.To;
            if (toClient == string.Empty)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                await Clients.Client(toClient).SendAsync("ReceiveMessage", message);
            }
        }
    }
}
