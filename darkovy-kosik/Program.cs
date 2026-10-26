using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;
using darkovy_kosik;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MyContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddAuthentication("WishlistCookieAuth")
    .AddCookie("WishlistCookieAuth", options =>
    {
        options.Cookie.Name = "WishlistAuthV2";
        options.LoginPath = "/Admin/Login"; 
        options.ExpireTimeSpan = TimeSpan.FromDays(365);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

string persistentUploads = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null 
    ? "/home/site/uploads" 
    : Path.Combine(builder.Environment.WebRootPath, "img", "uploads");

Directory.CreateDirectory(persistentUploads);

app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(persistentUploads),
    RequestPath = "/img/uploads"
});

var apkProvider = new FileExtensionContentTypeProvider();
apkProvider.Mappings[".apk"] = "application/vnd.android.package-archive";

app.UseStaticFiles(new StaticFileOptions {
    ContentTypeProvider = apkProvider
});

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();