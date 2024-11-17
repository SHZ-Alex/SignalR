namespace SignalR.Cache;

public static class LocalDatabase
{
    static LocalDatabase()
    {
        Voting = new Dictionary<string, int>
        {
            { Osh, 0 },
            { Burger, 0 },
            { Kalik, 0 }
        };
    }

    public const string Kalik = "Kalik";
    public const string Burger = "Burger";
    public const string Osh = "Osh";
    
    public static readonly Dictionary<string, int> Voting;
}