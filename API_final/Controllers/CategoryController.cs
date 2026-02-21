using API_final.DTOs;
using API_final.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_final.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // ==================== INVITADO ====================

    [HttpGet("restaurant/{restaurantId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByRestaurant(int restaurantId)
    {
        var categories = await _categoryService.GetCategoriesByRestaurantAsync(restaurantId);
        return Ok(categories);
    }

    // ==================== DUEÑO ====================

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        // 🔐 EXTRAEMOS EL ID DIRECTAMENTE DEL TOKEN
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var created = await _categoryService.CreateCategoryAsync(userId, dto);
        return CreatedAtAction(nameof(GetByRestaurant), new { restaurantId = userId }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            await _categoryService.UpdateCategoryAsync(id, userId, dto);
            return NoContent(); // 204 No Content es el estándar REST para un PUT exitoso
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            await _categoryService.DeleteCategoryAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}