using API_final.DTOs;
using API_final.Entities;
using API_final.Repository.Interfaces;
using Menu_Restaurante_API.Services.Interfaces;

namespace API_final.Services.Implementatios
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetCategoriesByRestaurantAsync(int userId)
        {
            var categories = await _categoryRepository.GetByRestaurantAsync(userId);

            // Mapeo manual de Entidad a DTO (se podría usar AutoMapper, pero así es más explícito)
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Categoría no encontrada.");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(int userId, CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                UserId = userId // Asignamos el dueño de forma obligatoria
            };

            var createdCategory = await _categoryRepository.AddAsync(category);

            return new CategoryDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name
            };
        }

        public async Task UpdateCategoryAsync(int id, int userId, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Categoría no encontrada.");

            // 🔥 VALIDACIÓN CRÍTICA (Defensa contra IDOR) 🔥
            if (category.UserId != userId)
                throw new UnauthorizedAccessException("No tenés permiso para editar esta categoría.");

            category.Name = dto.Name;
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id, int userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Categoría no encontrada.");

            // 🔥 VALIDACIÓN CRÍTICA (Defensa contra IDOR) 🔥
            if (category.UserId != userId)
                throw new UnauthorizedAccessException("No tenés permiso para eliminar esta categoría.");

            await _categoryRepository.DeleteAsync(category);
        }
    }
}
