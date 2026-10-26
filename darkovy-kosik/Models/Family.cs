using System.ComponentModel.DataAnnotations;

namespace darkovy_kosik.Models;

public class Family {
    
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FamilyName { get; set; } = string.Empty;
    
    public int AdminUserId { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}