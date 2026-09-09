using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using darkovy_kosik.Models;
using Microsoft.EntityFrameworkCore;

namespace darkovy_kosik.Controllers;

public class HomeController : Controller {
    
    private readonly MyContext _context;

    public HomeController(MyContext context) {
        _context = context;
    }
    
    public IActionResult Index() {
        
        List<User> users = _context.Users.ToList();
        
        return View(users);
    }

    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}