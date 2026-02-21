using API_final.Entities;

namespace API_final.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetByRestaurantAsync(int userId);
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
