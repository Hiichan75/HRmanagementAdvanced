using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HRmanagementAdvanced.Models;
using Microsoft.AspNetCore.Authorization;

namespace HRmanagementAdvanced.Controllers;
[Authorize]
public class HomeController : Controller
{
   

private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            ViewData["Message"] = "Welcome, Admin!";
        }
        else
        {
            ViewData["Message"] = "Welcome to the HR Management System!";
        }

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
//