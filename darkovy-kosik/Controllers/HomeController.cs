using System.Security.Claims;
using darkovy_kosik.Models;
using Microsoft.AspNetCore.Authorization;
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
            return RedirectToAction("Logout", "Admin");
        }

        if (user.Families.Count == 1) {
            return RedirectToAction("Index", "Family", new { id = user.Families.First().Id });
        }
        
        return View("SelectFamily", user.Families);
    }
    
    [HttpGet]
    public async Task<IActionResult> SelectFamily() 
    {
        int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int userId);
    
        var user = await _context.Users
            .Include(u => u.Families)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) 
        {
            return RedirectToAction("Login", "Admin");
        }
        
        return View("SelectFamily", user.Families);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddFamily(string familyName) {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) 
        {
            return View("Welcome");
        }
        
        var user = await _context.Users.Include(u => u.Families).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) {
            return View("Welcome");
        }
        
        Family fam = new Family() {
            FamilyName =  familyName,
            AdminUserId =  userId
        };
        
        fam.Users.Add(user);
        _context.Families.Add(fam);
        await _context.SaveChangesAsync();

        return RedirectToAction("SelectFamily");
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveFamily(int id) {
        
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) 
        {
            return View("Welcome");
        }
        
        var fam = _context.Families.FirstOrDefault(f => f.Id == id);

        if (fam.AdminUserId != userId) {
            return RedirectToAction("Index", "Home");
        }
        
        _context.Families.Remove(fam);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("SelectFamily");
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddUserToFamily(int id, string email) {
        ViewBag.AddUserErrored = false;
        ViewBag.Error = "";
        
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) 
        {
            return View("Welcome");
        }
        
        var addedUser = await _context.Users.Include(u => u.Families).FirstOrDefaultAsync(u => u.Email == email);
        var fam = await _context.Families.FirstOrDefaultAsync(f => f.Id == id);
        
        if (fam.AdminUserId != userId) {
            return RedirectToAction("Index", "Home");
        }
        
        if (addedUser == null) {
            TempData["Error"] = "Uživatel nenalezen.";
            TempData["AddUserErrored"] = true;
            TempData["ErroredFamilyId"] = id;
        }
        else {
            addedUser.Families.Add(fam);
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction("SelectFamily");
    }
}