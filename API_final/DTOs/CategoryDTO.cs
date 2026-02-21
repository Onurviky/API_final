using System.ComponentModel.DataAnnotations;

namespace API_final.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}