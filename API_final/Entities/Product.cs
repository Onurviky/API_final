using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_final.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")] // Fundamental para plata
    public decimal Price { get; set; }

    // Requerimientos específicos del TP
    public int DiscountPercent { get; set; } = 0;
    public bool HappyHourEnabled { get; set; } = false;
    public bool IsFavorite { get; set; } = false; // Para el endpoint de "favoritos"

    // Relaciones
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}