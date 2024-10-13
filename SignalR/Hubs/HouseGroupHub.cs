using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class HouseGroupHub : Hub
{
    private static List<string> GroupsJoined { get; set; } = [];
    
    public async Task JoinHouse(string houseName)
    {
        if (!GroupsJoined.Contains(Context.ConnectionId+":"+houseName))
        {
            GroupsJoined.Add(Context.ConnectionId + ":" + houseName);
            
            var houseList = GroupsJoined
                .Where(str => str.Contains(Context.ConnectionId))
                .Aggregate("", (current, str) => current + str.Split(':')[1] + " ");
            
            
            await Clients.Caller.SendAsync("subscriptionStatus", houseList, houseName.ToLower(), true);
            
            await Clients.Others.SendAsync("newMemberAddedToHouse", houseName);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, houseName);
        }
    }
    public async Task LeaveHouse(string houseName)
    {
        if (GroupsJoined.Contains(Context.ConnectionId + ":" + houseName))
        {
            GroupsJoined.Remove(Context.ConnectionId + ":" + houseName);
            
            var houseList = GroupsJoined
                .Where(str => str.Contains(Context.ConnectionId))
                .Aggregate("", (current, str) => current + str.Split(':')[1] + " ");
            
            await Clients.Caller.SendAsync("subscriptionStatus", houseList, houseName.ToLower(), false);
            
            await Clients.Others.SendAsync("newMemberRemovedFromHouse", houseName);
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, houseName);
        }
    }
    
    public async Task TriggerHouseNotify(string houseName)
    {
        await Clients.Group(houseName).SendAsync("triggerHouseNotification", houseName);
    }
}