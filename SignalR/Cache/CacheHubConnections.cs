namespace SignalR.Cache;

public static class CacheHubConnections
{
    //userid = connectionid
    public static readonly Dictionary<string, List<string>> Users = new();
    
    public static bool HasUserConnection(string userId, string connectionId)
    {
        try
        {
            if (Users.TryGetValue(userId, out var user))
            {
                return user.Any(p => p.Contains(connectionId));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        return false;
    }
    
    public static void AddUserConnection(string userId, string connectionId)
    {
        if (string.IsNullOrEmpty(userId) || HasUserConnection(userId, connectionId)) 
            return;


        if (Users.TryGetValue(userId, out var user))
        {
            user.Add(connectionId);
            return;
        }
        
        Users.Add(userId, [connectionId]);
    }
    
    public static bool HasUser(string userId)
    {
        try
        {
            if (Users.TryGetValue(userId, out var user))
                return user.Count != 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        return false;
    }
    
    
    public static List<string> OnlineUsers()
    {
        return Users.Keys.ToList();
    }
}