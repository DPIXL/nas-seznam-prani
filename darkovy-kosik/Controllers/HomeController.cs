using System.Security.Claims;
using darkovy_kosik.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darkovy_kosik.Controllers;

public class HomeController : Controller {
    
    private readonly MyContext _context;

    public HomeController(MyContext context) {
        _context = context;
    }
    
    public async Task<IActionResult> Index() {

        if (!User.Identity.IsAuthenticated) {
            return View("Welcome");
        }
        
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) 
        {
            return View("Welcome");
        }

        var user = await _context.Users.Include(u => u.Families).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) {
            RedirectToAction("Logout", "Admin");
        }

        if (user.Families.Count == 1) {
            return RedirectToAction("Index", "Family", new { id = user.Families.First().Id });
        }
        
        return View("SelectFamily", user.Families);
    }
}