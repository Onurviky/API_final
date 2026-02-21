using API_final.DTOs;
using API_final.DTOs.Requests;
using API_final.Entities;
using API_final.Repository.Interfaces;
using API_final.Services.Interfaces;


namespace API_final.Services.Implementatios;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // ==================== INVITADO (LECTURA) ====================

    public async Task<List<ProductDto>> GetProductsByRestaurantAsync(int restaurantId)
    {
        var products = await _productRepository.GetProductsByRestaurantAsync(restaurantId);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetFavoritesAsync(int restaurantId)
    {
        var products = await _productRepository.GetFavoritesAsync(restaurantId);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetDiscountedOrHappyHourAsync(int restaurantId)
    {
        var products = await _productRepository.GetDiscountedOrHappyHourAsync(restaurantId);
        return products.Select(MapToDto).ToList();
    }

    // ==================== DUEÑO (ESCRITURA) ====================

    public async Task<ProductDto> CreateProductAsync(int userId, CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            UserId = userId // Asignamos el dueño de forma obligatoria
        };

        var created = await _productRepository.AddAsync(product);
        return MapToDto(created);
    }

    public async Task SetDiscountAsync(int id, int userId, int discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException("El descuento debe estar entre 0 y 100.");

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Producto no encontrado.");

        // 🔥 VALIDACIÓN CRÍTICA (Defensa contra IDOR) 🔥
        if (product.UserId != userId)
            throw new UnauthorizedAccessException("No tenés permiso para editar este producto.");

        product.DiscountPercent = discountPercent;
        await _productRepository.UpdateAsync(product);
    }

    public async Task ToggleHappyHourAsync(int id, int userId, bool isEnabled)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Producto no encontrado.");

        // 🔥 VALIDACIÓN CRÍTICA (Defensa contra IDOR) 🔥
        if (product.UserId != userId)
            throw new UnauthorizedAccessException("No tenés permiso para editar este producto.");

        product.HappyHourEnabled = isEnabled;
        await _productRepository.UpdateAsync(product);
    }

    // AUMENTO MASIVO (REQUERIMIENTO EXTRA)
    public async Task IncreaseAllPricesAsync(int userId, decimal percentage)
    {
        if (percentage <= 0)
        {
            throw new ArgumentException("El porcentaje de aumento debe ser mayor a cero.");
        }

        // Delegamos la operación ultra optimizada al repositorio
        await _productRepository.IncreasePricesByPercentageAsync(userId, percentage);
    }

    // ==================== HELPER PRIVADO ====================
    // Centralizamos el mapeo para no repetir código en cada método de lectura
    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountPercent = product.DiscountPercent,
            HappyHourEnabled = product.HappyHourEnabled,
            IsFavorite = product.IsFavorite,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty
        };
    }
}