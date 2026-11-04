using darkovy_kosik.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darkovy_kosik.Controllers;

public class GiftListController : Controller {
    // GET

    private readonly MyContext _context;

    public GiftListController(MyContext context) {
        _context = context;
    }
    
    
    public IActionResult Index(int? id) {

        var items = _context.Items.Where(x => x.UserId == id).ToList();

        var user = _context.Users.FirstOrDefault(x => x.Id == id);
        
        ViewBag.Username = user?.Username;
        ViewBag.UserID = user?.Id;
        
        
        
        int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int currentUserId);
        var currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);
        if (currentUser != null) {
            ViewBag.CurrentEmail = currentUser.Email;
        }
        
        return View(items);
    }
    
    [HttpPost]
    public async Task<IActionResult> ReserveItem(int itemId, string reserverName, int userId) {
        
        reserverName = reserverName.Trim();
        
        var item = await _context.Items.FindAsync(itemId);
    
        if (item != null && !item.IsReserved) {
            item.IsReserved = true;
            item.ReservedBy = reserverName;
        
            await _context.SaveChangesAsync();
        }
    
        // Send them back to the user's gift list to see the updated status
        return RedirectToAction("Index", new { id = userId });
    }
}