
using API_final.DTOs;
using API_final.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API_final.Services.Interfaces;
using API_final.Services.Implementatios;

namespace API_final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var users = await _restaurantService.GetAllRestaurantsAsync();

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
