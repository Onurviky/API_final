using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_final.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Relación con el Dueño (Restaurante)
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Relación con los productos
    public ICollection<Product> Products { get; set; } = new List<Product>();
}