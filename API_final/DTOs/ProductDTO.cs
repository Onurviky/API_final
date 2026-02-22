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
public class UpdateProductDto
{
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal? Price { get; set; }

    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
    public int? DiscountPercent { get; set; }

    public bool? HappyHourEnabled { get; set; }

    public int? CategoryId { get; set; }

}