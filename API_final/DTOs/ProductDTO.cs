using System.ComponentModel.DataAnnotations;

namespace API_final.DTOs.Requests;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }
}