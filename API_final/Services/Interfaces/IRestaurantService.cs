using API_final.DTOs;

namespace API_final.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<List<RestaurantDto>> GetAllRestaurantsAsync();
    }
}
