using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;

namespace SignalR.Controllers;

public class HomeController(IHubContext<DeathlyHallowsHub> hubContext) : Controller
{
    public async Task<IActionResult> DeathlyHallows(string type)
    {
        if (SD.DealthyHallowRace.ContainsKey(type))
        {
            SD.DealthyHallowRace[type]++;
        }
        
        await hubContext.Clients.All.SendAsync("UpdateDeathlyHallowCount",
            SD.DealthyHallowRace[SD.Cloak],
            SD.DealthyHallowRace[SD.Stone],
            SD.DealthyHallowRace[SD.Wand]);
        
        
        return Accepted();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}