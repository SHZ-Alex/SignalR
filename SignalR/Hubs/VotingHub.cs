using Microsoft.AspNetCore.SignalR;
using SignalR.Cache;

namespace SignalR.Hubs;

public class VotingHub : Hub
{
    public Dictionary<string,int> GetRaceStatus()
    {
        return LocalDatabase.Voting;
    }
}