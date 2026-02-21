using API_final.DTOs;
using API_final.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public RestaurantController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllRestaurantsAsync();

            var response = users.Select(u => new RestaurantDto
            {
                Id = u.Id,
                RestaurantName = u.RestaurantName,
                Address = u.Address
            });

            return Ok(response);
        }
    }
}
