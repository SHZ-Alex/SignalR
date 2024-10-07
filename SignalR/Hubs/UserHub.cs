using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class UserHub : Hub
{
    private const string UpdateTotalUser = "UpdateTotalUsers";
    private const string UpdateViews = "UpdateViews";
    private static int CountViews { get; set; }

    private static int TotalUsers { get; set; }
    
    public async Task<string> NewWindowLoaded(string name)
    {
        CountViews++;
        await Clients.All.SendAsync(UpdateViews, CountViews);
        return $"Total views {name} - {CountViews}";
    }
    
    public override async Task OnConnectedAsync()
    {
        TotalUsers++;
        await Clients.All.SendAsync(UpdateTotalUser, TotalUsers);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        TotalUsers--;
        await Clients.All.SendAsync(UpdateTotalUser, TotalUsers);
        await base.OnDisconnectedAsync(exception);
    }
}