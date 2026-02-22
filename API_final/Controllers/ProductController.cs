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
        [HttpGet("restaurant/{restaurantId:int}/category/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int restaurantId, int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(restaurantId, categoryId);
                return Ok(products);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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

        // Modificar Happy Hour 
        [HttpPatch("{id:int}/happy-hour")]
        [Authorize]
        public async Task<IActionResult> ToggleHappyHour(int id, [FromBody] bool isEnabled)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _productService.ToggleHappyHourAsync(id, userId, isEnabled);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var updated = await _productService.UpdateProductAsync(id, userId, dto);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException) { return Forbid(); } // 403 Forbidden
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); } // 404
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                await _productService.DeleteProductAsync(id, userId);
                return NoContent(); // Estándar REST para borrado exitoso
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpPatch("{id:int}/favorite")]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                await _productService.ToggleFavouriteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        }

        // REQUERIMIENTO EXTRA 
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