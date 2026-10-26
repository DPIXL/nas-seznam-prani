using Microsoft.EntityFrameworkCore;
using darkovy_kosik.Models;

namespace darkovy_kosik;


public class MyContext : DbContext {
    
    public DbSet<Item> Items { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DbSet<Family> Families { get; set; }
    
    public MyContext(DbContextOptions<MyContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Families)
            .WithMany(f => f.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserFamilies",
                j => j.HasOne<Family>().WithMany().HasForeignKey("FamilyId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
            );
    }
    
}