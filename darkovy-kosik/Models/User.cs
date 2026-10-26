using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace darkovy_kosik.Models;

[Index(nameof(Username), IsUnique = true)]
public class User {
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    public ICollection<Item> Items { get; set; } = new List<Item>();

    // Navigation property for N:N
    public ICollection<Family> Families { get; set; } = new List<Family>();
}