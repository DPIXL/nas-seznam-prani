using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using darkovy_kosik.Models;
using Microsoft.EntityFrameworkCore;

namespace darkovy_kosik.Controllers;

public class FamilyController : Controller {
    
    private readonly MyContext _context;

    public FamilyController(MyContext context) {
        _context = context;
    }
    
    public IActionResult Index(int? id) {
        
        var fam = _context.Families.Include(f => f.Users).FirstOrDefault(f  => f.Id == id);

        if (fam == null) {
            RedirectToAction("Index", "Home");
        }
        
        List<User> users = fam.Users.ToList();
        
        return View(users);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}