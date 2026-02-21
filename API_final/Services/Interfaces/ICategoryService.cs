using API_final.DTOs;

namespace Menu_Restaurante_API.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesByRestaurantAsync(int userId);
    Task<CategoryDto> GetCategoryByIdAsync(int id);

    // Métodos de escritura (requieren el userId para saber quién lo crea/modifica)
    Task<CategoryDto> CreateCategoryAsync(int userId, CreateCategoryDto dto);
    Task UpdateCategoryAsync(int id, int userId, UpdateCategoryDto dto);
    Task DeleteCategoryAsync(int id, int userId);
}