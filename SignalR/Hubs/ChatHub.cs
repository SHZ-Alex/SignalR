using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Cache;
using SignalR.Data;

namespace SignalR.Hubs;

public class ChatHub(ApplicationDbContext db) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);


        if (string.IsNullOrEmpty(userId))
        {
            await base.OnConnectedAsync();
            return;
        }


        var userName = (await db.Users.FirstOrDefaultAsync(u => u.Id == userId))?.UserName;

        await Clients.Users(CacheHubConnections.OnlineUsers())
            .SendAsync("ReceiveUserConnected", userId, userName, CacheHubConnections.HasUser(userId));

        CacheHubConnections.AddUserConnection(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }
    
    public async Task SendPrivateMessage(string receiverId, string message, string receiverName)
    {
        var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        
        var senderName = (await db.Users.FirstOrDefaultAsync(u => u.Id == senderId))?.UserName;
        
        var users = new[] { senderId, receiverId };
        
        await Clients.Users(users!)
            .SendAsync("ReceivePrivateMessage", senderId, senderName, receiverId, message, Guid.NewGuid(),receiverName);
    }
    
    public async Task SendPublicMessage(int roomId,string message, string roomName)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = (await db.Users.FirstOrDefaultAsync(u => u.Id == userId))?.UserName;
        await Clients.All.SendAsync("ReceivePublicMessage", roomId, userId,userName, message,roomName);
    }
    
    public async Task SendDeleteRoomMessage(int deleted, int selected, string roomName)
    {
        var userId =  Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var userName = (await db.Users.FirstOrDefaultAsync(u => u.Id == userId))?.UserName;
        
        await Clients.All.SendAsync("ReceiveDeleteRoomMessage", deleted,selected, roomName,userName);
    }

    public async Task SendAddRoomMessage(int maxRoom, int roomId, string roomName)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var userName = (await db.Users.FirstOrDefaultAsync(u => u.Id == userId))?.UserName;

        await Clients.All.SendAsync("ReceiveAddRoomMessage", maxRoom, roomId, roomName, userId, userName);
    }
    
    public async Task SendOpenPrivateChat(string receiverId)
    {
        var username = Context.User?.FindFirstValue(ClaimTypes.Name);
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        await Clients.User(receiverId).SendAsync("ReceiveOpenPrivateChat", userId, username);
    }
    public async Task SendDeletePrivateChat(string chartId)
    {
        await Clients.All.SendAsync("ReceiveDeletePrivateChat", chartId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        if (CacheHubConnections.HasUserConnection(userId, Context.ConnectionId))
        {
            var userConnections = CacheHubConnections.Users[userId];
            userConnections.Remove(Context.ConnectionId);

            CacheHubConnections.Users.Remove(userId);
            if (userConnections.Count != 0)
                CacheHubConnections.Users.Add(userId, userConnections);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            var userName = (await db.Users.FirstOrDefaultAsync(u => u.Id == userId))?.UserName;

            await Clients.Users(CacheHubConnections.OnlineUsers())
                .SendAsync("ReceiveUserDisconnected", userId, userName);

            CacheHubConnections.AddUserConnection(userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}