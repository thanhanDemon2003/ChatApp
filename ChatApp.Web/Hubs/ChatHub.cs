using Chat.Infrastructure.Data;
using ChatApp.Application.Utility;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }


        //public async Task OnConnectedAsync(string UserId)
        //{

        //    if (!String.IsNullOrEmpty(UserId))
        //    {
        //        var name = _db.Users.FirstOrDefault(u => u.Id == UserId).Name;
        //        Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserConnected", UserId, name);
        //        HubConnections.AddUserConnection(UserId, Context.ConnectionId);
        //    }
        //    await base.OnConnectedAsync();
        //}
        public async Task OnConnectedAsync(string UserId)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                var name = _db.Users.FirstOrDefault(u => u.Id == UserId).Name;
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserConnected", UserId, name);
                var usersOnline = HubConnections.OnlineUsers();
                foreach (var userIdOnline in usersOnline)
                {
                    if (userIdOnline != UserId)
                    {
                        var onlineUserName = _db.Users.FirstOrDefault(u => u.Id == userIdOnline).Name;
                        Clients.User(UserId).SendAsync("ReceiveUserConnected", userIdOnline, onlineUserName);
                    }
                }
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);


            }
            await base.OnConnectedAsync();
        }
        public async Task OnDisconnectedAsync(Exception? exception, string UserId)
        {

            if (HubConnections.HasUserConnection(UserId, Context.ConnectionId))
            {
                var UserConnections = HubConnections.Users[UserId];
                UserConnections.Remove(Context.ConnectionId);

                HubConnections.Users.Remove(UserId);
                if (UserConnections.Any())
                    HubConnections.Users.Add(UserId, UserConnections);
            }
            if (!String.IsNullOrEmpty(UserId))
            {
                var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserDisconnected", UserId, userName);
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        //public async Task SendAddRoomMessage(int groupId, string createById, List<string> userIds)
        //{
        //    var group = _db.Groups.FirstOrDefault(r => r.GroupId == groupId);
        //    var createBy = _db.Users.FirstOrDefault(u => u.Id == createById);
        //    var users = _db.Users.Where(u => userIds.Contains(u.Id)).ToList();

        //    await Clients.Users(userIds).SendAsync("ReceiveAddRoomMessage", group, createBy, users);
        //}
        //public async Task SendMessageGroup(int groupId, string message, string senderId)
        //{
        //    var group = _db.Groups.FirstOrDefault(r => r.GroupId == groupId);
        //    var sender = _db.Users.FirstOrDefault(u => u.Id == senderId);
        //    var users = _db.Users.Where(u => group.UserGroups.Select(ug => ug.UserId).Contains(u.Id)).ToList();

        //    await Clients.Groups(groupId.ToString()).SendAsync("ReceiveMessageGroup", group, sender, message, users);
        //}
        //public async Task SendMessageToUserAsync(string receiverId, string message, string senderId)
        //{
        //    var sender = _db.Users.FirstOrDefault(u => u.Id == senderId);
        //    var receiver = _db.Users.FirstOrDefault(u => u.Id == receiverId);

        //    await Clients.Users(receiverId).SendAsync("ReceiveMessageToUser", sender, message, receiver);
        //}
        //public async Task DeleteGroupChat(int groupId)
        //{
        //    await Clients.All.SendAsync("ReceiveDeleteGroupChat", groupId);
        //}
        //public async Task DeletePrivateChat(string chartId)
        //{
        //    await Clients.All.SendAsync("ReceiveDeletePrivateChat", chartId);
        //}




        //public override Task OnConnectedAsync()
        //{
        //    var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (!String.IsNullOrEmpty(UserId))
        //    {
        //        var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;
        //        Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserConnected", UserId, userName);
        //        HubConnections.AddUserConnection(UserId, Context.ConnectionId);
        //    }
        //    return base.OnConnectedAsync();
        //}
        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (HubConnections.HasUserConnection(UserId, Context.ConnectionId))
        //    {
        //        var UserConnections = HubConnections.Users[UserId];
        //        UserConnections.Remove(Context.ConnectionId);

        //        HubConnections.Users.Remove(UserId);
        //        if (UserConnections.Any())
        //            HubConnections.Users.Add(UserId, UserConnections);
        //    }

        //    if (!String.IsNullOrEmpty(UserId))
        //    {
        //        var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;
        //        Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserDisconnected", UserId, userName);
        //        HubConnections.AddUserConnection(UserId, Context.ConnectionId);
        //    }
        //    return base.OnDisconnectedAsync(exception);
        //}

        //public async Task SendAddRoomMessage(int maxRoom, int roomId, string roomName)
        //{
        //    var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;

        //    await Clients.All.SendAsync("ReceiveAddRoomMessage", maxRoom, roomId, roomName, UserId, userName);
        //}

        //public async Task SendDeleteRoomMessage(int deleted, int selected, string roomName)
        //{
        //    var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;

        //    await Clients.All.SendAsync("ReceiveDeleteRoomMessage", deleted, selected, roomName, userName);
        //}

        //public async Task SendPublicMessage(int roomId, string message, string roomName)
        //{
        //    var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;

        //    await Clients.All.SendAsync("ReceivePublicMessage", roomId, UserId, userName, message, roomName);
        //}

        //public async Task SendPrivateMessage(string receiverId, string message, string receiverName)
        //{
        //    var senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var senderName = _db.Users.FirstOrDefault(u => u.Id == senderId).UserName;

        //    var users = new string[] { senderId, receiverId };

        //    await Clients.Users(users).SendAsync("ReceivePrivateMessage", senderId, senderName, receiverId, message, Guid.NewGuid(), receiverName);
        //}

        //public async Task SendOpenPrivateChat(string receiverId)
        //{
        //    var username = Context.User.FindFirstValue(ClaimTypes.Name);
        //    var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    await Clients.User(receiverId).SendAsync("ReceiveOpenPrivateChat", userId, username);
        //}

        //public async Task SendDeletePrivateChat(string chartId)
        //{
        //    await Clients.All.SendAsync("ReceiveDeletePrivateChat", chartId);
        //}
    }
}
