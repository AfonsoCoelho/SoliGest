using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SoliGest.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.UserIdentifier;

            await Clients.User(receiverId)
                         .SendAsync("ReceiveMessage", senderId, content);
        }
    }

}
