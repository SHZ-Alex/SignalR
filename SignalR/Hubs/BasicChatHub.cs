using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Data;

namespace SignalR.Hubs;

public class BasicChatHub(ApplicationDbContext db) : Hub
{
    public async Task SendMessageToAll(string user, string message)
    {
        await Clients.All.SendAsync("MessageReceived", user, message);
    }
    
    [Authorize]
    public async Task SendMessageToReceiver(string sender, string receiver, string message)
    {
        var userId = db.Users
            .Where(x => x.Email.ToLower() == receiver.ToLower())
            .Select(x => x.Id)
            .FirstOrDefault();
        
        if (!string.IsNullOrEmpty(userId))
            await Clients.User(userId).SendAsync("MessageReceived", sender, message);
    }
}