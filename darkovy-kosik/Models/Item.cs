using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darkovy_kosik.Models;

public class Item {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Název dárku je povinný!")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Odkaz je povinný!")]
    [MaxLength(2083)]
    [Url]
    public string Link { get; set; } = string.Empty;
    
    [MaxLength(2083)]
    [Url]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Cena je povinná!")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    public int UserId { get; set; }
    
    public bool IsReserved { get; set; } = false;
    
    public string? ReservedBy { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; } = null!;
}