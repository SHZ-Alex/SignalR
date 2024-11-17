using System.ComponentModel.DataAnnotations;

namespace SignalR.Data;

public class ChatRoom
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string Name { get; set; }
}