using API_final.DTOs.Requests;
using API_final.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ==================== INVITADO ====================

        // Obtener menú completo de un restaurante
        [HttpGet("restaurant/{restaurantId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRestaurant(int restaurantId)
        {
            // Nota: Asumimos que tu ProductService tiene este método implementado
            var products = await _productService.GetProductsByRestaurantAsync(restaurantId);
            return Ok(products);
        }

        // Obtener filtrado por categoría
        [HttpGet("category/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        // Obtener destacados/favoritos
        [HttpGet("restaurant/{restaurantId:int}/favorites")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFavorites(int restaurantId)
        {
            var products = await _productService.GetFavoritesAsync(restaurantId);
            return Ok(products);
        }

        // Obtener promos (Descuento o Happy Hour)
        [HttpGet("restaurant/{restaurantId:int}/promotions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotions(int restaurantId)
        {
            var products = await _productService.GetDiscountedOrHappyHourAsync(restaurantId);
            return Ok(products);
        }

        // ==================== DUEÑO ====================

        // Crear producto
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var created = await _productService.CreateProductAsync(userId, dto);
            return Ok(created);
        }

        // Modificar descuento (Endpoint propio como pide el TP)
        [HttpPatch("{id:int}/discount")]
        [Authorize]
        public async Task<IActionResult> SetDiscount(int id, [FromBody] int discountPercent)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _productService.SetDiscountAsync(id, userId, discountPercent);
            return NoContent();
        }

        // Modificar Happy Hour (Endpoint propio como pide el TP)
        [HttpPatch("{id:int}/happy-hour")]
        [Authorize]
        public async Task<IActionResult> ToggleHappyHour(int id, [FromBody] bool isEnabled)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _productService.ToggleHappyHourAsync(id, userId, isEnabled);
            return NoContent();
        }

        // 🔥 REQUERIMIENTO EXTRA: Aumento Masivo 🔥
        [HttpPost("increase-prices")]
        [Authorize]
        public async Task<IActionResult> IncreasePrices([FromBody] decimal percentage)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                await _productService.IncreaseAllPricesAsync(userId, percentage);
                return Ok(new { message = $"Los precios aumentaron un {percentage}% exitosamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}