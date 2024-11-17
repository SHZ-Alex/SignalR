using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR.Data;

namespace SignalR.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ChatRoomsController(ApplicationDbContext db) : ControllerBase
    {
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetChatRoom()
        {
            return await db.ChatRooms.ToListAsync();
        }
        
        
        [HttpGet]
        [Route(nameof(CurrentUser))]
        public async Task<ActionResult<object>> CurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return await db.Users
                .Where(u => u.Id != userId)
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();
        }
        
        [HttpPost]
        public async Task<ActionResult<ChatRoom>> PostChatRoom(ChatRoom chatRoom)
        {
            db.ChatRooms.Add(chatRoom);
            await db.SaveChangesAsync();
            
            return CreatedAtAction("GetChatRoom", new { id = chatRoom.Id }, chatRoom);
        }
        
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteChatRoom(int id)
        {
            var chatRoom = await db.ChatRooms.FindAsync(id);
            
            if (chatRoom == null)
            {
                return NotFound();
            }
            db.ChatRooms.Remove(chatRoom);
            await db.SaveChangesAsync();
            
            
            var room = await db.ChatRooms.FirstOrDefaultAsync();
            
            return Ok(new { deleted = id, selected = room?.Id ?? 0 });
        }
        
    }
}