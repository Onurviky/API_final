using API_final.Entities;

namespace API_final.Repository.Interfaces
{
    public interface IProductRepository
    {
        // Métodos para el Invitado
        Task<List<Product>> GetProductsByRestaurantAsync(int userId);
        Task<List<Product>> GetProductsByCategoryAsync(int restaurantId,int categoryId);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetFavoritesAsync(int userId);
        Task<List<Product>> GetDiscountedOrHappyHourAsync(int userId);

        // Métodos para el Dueño
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task IncreasePricesByPercentageAsync(int userId, decimal percentage);
    }
}
