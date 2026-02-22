using API_final.Services.Interfaces;
using API_final.DTOs;
using API_final.Repository.Interfaces;

namespace API_final.Services.Implementatios
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUserRepository _userRepository;
        public RestaurantService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<RestaurantDto>> GetAllRestaurantsAsync()
        {
            var users = await _userRepository.GetAllRestaurantsAsync();
            return users.Select(u => new RestaurantDto
            {
                Id = u.Id,
                RestaurantName = u.RestaurantName,
                Address = u.Address
            }).ToList();
        }
    }
}
