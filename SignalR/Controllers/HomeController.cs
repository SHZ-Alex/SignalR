using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Cache;
using SignalR.Data;
using SignalR.Hubs;
using SignalR.Models;
using SignalR.ViewModel;

namespace SignalR.Controllers;

public class HomeController(IHubContext<VotingHub> hubContext,
    IHubContext<OrderHub> orderHub,
    ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Vote(string type)
    {
        if (LocalDatabase.Voting.ContainsKey(type))
            LocalDatabase.Voting[type]++;
        
        await hubContext.Clients.All.SendAsync("UpdateVotingCount",
            LocalDatabase.Voting[LocalDatabase.Osh],
            LocalDatabase.Voting[LocalDatabase.Burger],
            LocalDatabase.Voting[LocalDatabase.Kalik]);
        
        return Accepted();
    }
    
    public IActionResult BasicChat()
        => View();

    public IActionResult Index()
        => View();

    public IActionResult Notification()
        => View();
    
    public IActionResult Voting()
        => View();
    
    public IActionResult HarryPotterHouse()
        => View();
    
    [ActionName("Order")]
    public Task<IActionResult> Order()
    {
        string[] name = ["Alex", "Shahzod", "Djimi", "Bob", "Leo"];
        string[] itemName = ["Kalik", "DS1", "DS2", "DS3", "Elden Ring"];
        
        var rand = new Random();
        
        var index = rand.Next(name.Length);
        var order = new Order
        {
            Name = name[index],
            ItemName = itemName[index],
            Count = index
        };
        
        return Task.FromResult<IActionResult>(View(order));
    }
    
    [ActionName("Order")]
    [HttpPost]
    public async Task<IActionResult> OrderPost(Order order)
    {
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        
        // can send this order, but I'm lazy
        await orderHub.Clients.All.SendAsync("newOrder");
        return RedirectToAction(nameof(Order));
    }
    
    [ActionName("OrderList")]
    public Task<IActionResult> OrderList()
        => Task.FromResult<IActionResult>(View());
    
    public async Task<IActionResult> AdvancedChat()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var rooms = await db.ChatRooms.ToListAsync();
        
        ChatVM chatVm = new()
        {
            Rooms = rooms,
            MaxRoomAllowed = 4,
            UserId = userId,
        };
        
        return View(chatVm);
    }
    
    [HttpGet]
    public IActionResult GetAllOrder()
    {
        var productList = db.Orders.ToList();
        return Json(new { data = productList });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    
    [Authorize]
    public IActionResult Chat()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        ChatVM chatVm = new()
        {
            Rooms = db.ChatRooms.ToList(),
            MaxRoomAllowed = 4,
            UserId = userId,
        };
        return View(chatVm);
    }
}