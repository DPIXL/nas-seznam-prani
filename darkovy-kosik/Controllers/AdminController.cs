using System.Security.Claims;
using darkovy_kosik.DTO;
using darkovy_kosik.HelperMethods;
using darkovy_kosik.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace darkovy_kosik.Controllers;

public class AdminController : Controller {
    
    private readonly MyContext _context;

    private readonly IWebHostEnvironment _env;

    public AdminController(MyContext context, IWebHostEnvironment env) {
        _context = context;
        _env = env;
    }
    
    [HttpGet]
    public IActionResult Login() {
        if (User.Identity != null && User.Identity.IsAuthenticated) {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId)) {
                return RedirectToAction("Panel", new { id = userId });
            }
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password) {
    
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null) {
        
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (isValid) {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, "WishlistCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365)
                };

                await HttpContext.SignInAsync("WishlistCookieAuth", principal, authProperties);
            
                return RedirectToAction("Panel", new { id = user.Id });
            }
        }
    
        ViewBag.Error = "Špatné jméno nebo heslo.";
        return View();
    }
    
    [HttpGet]
    public IActionResult Register() {
        if (User.Identity != null && User.Identity.IsAuthenticated) {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId)) {
                return RedirectToAction("Panel", new { id = userId });
            }
        }
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(string username, string password, string confirmPassword, string name, string email, IFormFile? banner) {

        ViewBag.Error = "";
        
        bool userExists = await _context.Users.AnyAsync(u => u.Username == username);

        if (userExists) {
            ViewBag.Error += "Toto uživatelské jméno již existuje.\n";
            return View();
        }

        if (password != confirmPassword) {
            ViewBag.Error += "Hesla se neshodují.\n";
            return View();
        }

        string bannerPath = "";
        
        if (banner != null && banner.Length > 0) {
            try {
                string uploadsFolder = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null
                    ? "/home/site/uploads/users"
                    : Path.Combine(_env.WebRootPath, "img", "uploads", "users");

                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + ".webp";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = banner.OpenReadStream())
                using (var image = await Image.LoadAsync(stream)) {
                    image.Mutate(x => x.Resize(new ResizeOptions {
                        Size = new Size(600, 0),
                        Mode = ResizeMode.Max
                    }));

                    await image.SaveAsWebpAsync(filePath);
                }

                bannerPath = "/img/uploads/users/" + uniqueFileName;
            }
            catch (UnknownImageFormatException) {
                Console.WriteLine("UPLOAD ERROR: Unsupported image format.");

                bannerPath = "/img/default-user.png";
            }
            catch (Exception ex) {
                Console.WriteLine("UPLOAD ERROR: " + ex.Message);

                bannerPath = "/img/default-user.png";
            }
        } else {
            bannerPath = "/img/default-user.png";
        }
        
        User user = new User() { 
            Username = username, 
            Password = BCrypt.Net.BCrypt.HashPassword(password), 
            Email = email, 
            Name = name, 
            CardImagePath = bannerPath 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
            
        return RedirectToAction("Login");
    }
    
    
    [HttpGet]
    [Authorize]
    public IActionResult Panel(int id) {
        
        var loggedInIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(loggedInIdString, out int loggedInUserId)) {
            if (loggedInUserId != id) {
                
                return RedirectToAction("Panel", new { id = loggedInUserId });
            }
        }

        PanelDTO dto = new PanelDTO();
        dto.Items = _context.Items.Where(i => i.UserId == id).ToList();
        dto.Item = new Item();
        dto.Item.UserId = id;
        
        return View(dto);
    }

    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync("WishlistCookieAuth");
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddItem([Bind(Prefix = "Item")] Item newItem, IFormFile? uploadedImage) {

        if (!ModelState.IsValid) {
            PanelDTO dto = new PanelDTO();
            dto.Item = newItem;
            dto.Items = _context.Items.Where(i => i.UserId == newItem.UserId).ToList();
            return View("Panel", dto);
        }
        
        if (uploadedImage != null && uploadedImage.Length > 0) {
            try {
                string uploadsFolder = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null 
                    ? "/home/site/uploads" 
                    : Path.Combine(_env.WebRootPath, "img", "uploads");

                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + ".webp";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = uploadedImage.OpenReadStream())
                using (var image = await Image.LoadAsync(stream)) {
                    image.Mutate(x => x.Resize(new ResizeOptions {
                        Size = new Size(600, 0),
                        Mode = ResizeMode.Max
                    }));
                
                    await image.SaveAsWebpAsync(filePath);
                }

                newItem.ImageUrl = "/img/uploads/" + uniqueFileName;
            }
            catch (UnknownImageFormatException) {
                Console.WriteLine("UPLOAD ERROR: Unsupported image format.");
       
                newItem.ImageUrl = "/img/default-gift.png";
            }
            catch (Exception ex) {
                Console.WriteLine("UPLOAD ERROR: " + ex.Message);
          
                newItem.ImageUrl = "/img/default-gift.png";
            }
        } 
        else {
   
            GetImageUrl giu = new GetImageUrl();
            string scrapedUrl = await giu.GetImageUrlAsync(newItem.Link);
        
            newItem.ImageUrl = !string.IsNullOrEmpty(scrapedUrl) 
                ? scrapedUrl 
                : "/img/default-gift.png";
        }

        _context.Items.Add(newItem);
        await _context.SaveChangesAsync();

        return RedirectToAction("Panel", new { id = newItem.UserId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteItem(int itemId, int userId) {
        var itemToDelete = await _context.Items.FindAsync(itemId);
    
        if (itemToDelete != null) {
            if (!string.IsNullOrEmpty(itemToDelete.ImageUrl) && 
                itemToDelete.ImageUrl.StartsWith("/img/uploads/")) {
            
                try {
                    string relativePath = itemToDelete.ImageUrl.Replace("/img/uploads/", "");

                    string persistentUploads = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null 
                        ? "/home/site/uploads" 
                        : Path.Combine(_env.WebRootPath, "img", "uploads");

                    string fullPath = Path.Combine(persistentUploads, relativePath);

                    if (System.IO.File.Exists(fullPath)) {
                        System.IO.File.Delete(fullPath);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Failed to delete image file: " + ex.Message);
                }
            }

            _context.Items.Remove(itemToDelete);
            await _context.SaveChangesAsync();
        }
    
        return RedirectToAction("Panel", new { id = userId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(string newPassword) {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        int userId = int.Parse(userIdString);
        
        var user =  await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Panel", new { id = userId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangeImage(int itemId, IFormFile uploadedImage) {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        int userId = int.Parse(userIdString);

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);

        if (!string.IsNullOrEmpty(item.ImageUrl) &&
            item.ImageUrl.StartsWith("/img/uploads/")) {

            try {
                string relativePath = item.ImageUrl.Replace("/img/uploads/", "");

                string persistentUploads = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null
                    ? "/home/site/uploads"
                    : Path.Combine(_env.WebRootPath, "img", "uploads");

                string fullPath = Path.Combine(persistentUploads, relativePath);

                if (System.IO.File.Exists(fullPath)) {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to delete image file: " + ex.Message);
            }
        }

        if (uploadedImage != null && uploadedImage.Length > 0) {
            try {
                string uploadsFolder = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null 
                    ? "/home/site/uploads" 
                    : Path.Combine(_env.WebRootPath, "img", "uploads");

                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + ".webp";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = uploadedImage.OpenReadStream())
                using (var image = await Image.LoadAsync(stream)) {
                    image.Mutate(x => x.Resize(new ResizeOptions {
                        Size = new Size(600, 0),
                        Mode = ResizeMode.Max
                    }));
                
                    await image.SaveAsWebpAsync(filePath);
                }

                item.ImageUrl = "/img/uploads/" + uniqueFileName;
            }
            catch (UnknownImageFormatException) {
                Console.WriteLine("UPLOAD ERROR: Unsupported image format.");
       
                item.ImageUrl = "/img/default-gift.png";
            }
            catch (Exception ex) {
                Console.WriteLine("UPLOAD ERROR: " + ex.Message);
          
                item.ImageUrl = "/img/default-gift.png";
            }
            
            await _context.SaveChangesAsync();
        } 
        
        return RedirectToAction("Panel", new { id = userId });
    }
}