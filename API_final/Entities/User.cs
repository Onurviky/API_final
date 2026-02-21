using System.ComponentModel.DataAnnotations;

namespace API_final.Entities;
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string RestaurantName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty; // Acá atacamos el requerimiento extra del Hasheo

    public string Address { get; set; } = string.Empty;

    // Relaciones (Un restaurante tiene muchas categorías y muchos productos)
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

