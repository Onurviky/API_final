using API_final.DTOs;
using API_final.DTOs.Requests;

namespace API_final.Services.Interfaces
{
    public interface IProductService
    {
        // Lectura (Invitado)
        Task<List<ProductDto>> GetProductsByRestaurantAsync(int restaurantId);
        Task<List<ProductDto>> GetProductsByCategoryAsync(int restaurantId, int categoryId);
        Task<List<ProductDto>> GetFavoritesAsync(int restaurantId);
        Task<List<ProductDto>> GetDiscountedOrHappyHourAsync(int restaurantId);

        // Escritura (Dueño)
        Task<ProductDto> CreateProductAsync(int userId, CreateProductDto dto);
        Task SetDiscountAsync(int id, int userId, int discountPercent);
        Task ToggleHappyHourAsync(int id, int userId, bool isEnabled);
        Task IncreaseAllPricesAsync(int userId, decimal percentage);
        Task<ProductDto> UpdateProductAsync(int id, int userId, UpdateProductDto dto);
        Task DeleteProductAsync(int id, int userId);
        Task ToggleFavouriteAsync(int id, int userId);
    }
}
