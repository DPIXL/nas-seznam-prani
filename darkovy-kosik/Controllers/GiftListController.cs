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
        
        return View(items);
    }
    
    [HttpPost]
    public async Task<IActionResult> ReserveItem(int itemId, string reserverName, int userId) {
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