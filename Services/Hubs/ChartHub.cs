using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Services.Hubs
{
    public class AxHub : Hub
    {
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
