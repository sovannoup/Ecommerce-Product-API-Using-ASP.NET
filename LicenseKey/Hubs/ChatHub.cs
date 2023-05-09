using LicenseKey.Controllers.Request;
using LicenseKey.Models;
using LicenseKey.Repository;
using Microsoft.AspNetCore.SignalR;
using System.Security.Principal;

namespace LicenseKey.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _userImg;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly ApplicationDbContext _appDbContext;

        public ChatHub(IDictionary<string, UserConnection> connections, ApplicationDbContext context)
        {
            _userImg = "";
            _connections = connections;
            _appDbContext = context;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", "label", $"{userConnection.User} has left");
                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            _connections[Context.ConnectionId].ImageUrl = userConnection.ImageUrl;


            List<ContactMessage> messages = _appDbContext.ContactMessage.Where(x => x.Room == userConnection.Room).ToList();
            //foreach (ContactMessage message in messages)
            //{
            //    message.SendDate = DateTime.Parse(message.SendDate.ToString("dd-MM-yyyy HH:mm"));
            // }
            await Clients.Client(Context.ConnectionId).SendAsync("PreviousMessage", messages);

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", "label", $"{userConnection.User} has joined conversation.");
            // await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "label", $"{userConnection.User} Just test conver!!!!");
            await SendUsersConnected(userConnection.Room);
        }

        public async Task SendMessage(string message)
        {
            if (message == null) return;
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                ContactMessage contactMessage = new(userConnection.User, userConnection.ImageUrl,userConnection.Role, userConnection.Room,message);
                _appDbContext.ContactMessage.Add(contactMessage);
                _appDbContext.SaveChanges();
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection, message);
            }
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}
