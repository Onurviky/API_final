using API_final.DTOs;
using API_final.DTOs.Requests;
using API_final.Entities;
using API_final.Repository.Interfaces;
using API_final.Services.Interfaces;


namespace API_final.Services.Implementatios;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // ==================== INVITADO (LECTURA) ====================

    public async Task<List<ProductDto>> GetProductsByRestaurantAsync(int restaurantId)
    {
        var products = await _productRepository.GetProductsByRestaurantAsync(restaurantId);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetProductsByCategoryAsync(int restaurantId, int categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);

        if (category == null)
            throw new KeyNotFoundException("La categoria no existe.");

        if (category.UserId != restaurantId)
            throw new ArgumentException("Esta categoria no pertenece al restaurante seleccionado.");
        var products = await _productRepository.GetProductsByCategoryAsync(restaurantId,categoryId);
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
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null) {
            throw new KeyNotFoundException("la categoria especificada no existe");
        }
        if (category.UserId != userId) {
            throw new UnauthorizedAccessException("No puedes asignar uan categoria que no te pertenece");
        }
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            UserId = userId, // Asignamos el dueño de forma obligatoria
            Category = category

        };

        var created = await _productRepository.AddAsync(product);
        return MapToDto(created);
    }
    public async Task<ProductDto> UpdateProductAsync(int id, int userId, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Producto no encontrado.");

        // PROTECCIÓN IDOR: Validamos que el producto sea de este dueño
        if (product.UserId != userId)
            throw new UnauthorizedAccessException("No podés editar un producto que no te pertenece.");

        // Si el usuario mandó un CategoryId nuevo, validamos que esa categoría también sea suya
        if (dto.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
            if (category == null) throw new KeyNotFoundException("La nueva categoría no existe.");
            if (category.UserId != userId) throw new UnauthorizedAccessException("La categoría no te pertenece.");

            product.CategoryId = dto.CategoryId.Value;
            product.Category = category; // Actualizamos para el mapeo
        }

        // Actualizamos solo los campos que vengan con datos
        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.DiscountPercent.HasValue) product.DiscountPercent = dto.DiscountPercent.Value;
        if (dto.HappyHourEnabled.HasValue) product.HappyHourEnabled = dto.HappyHourEnabled.Value;

        await _productRepository.UpdateAsync(product);
        return MapToDto(product);
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
    public async Task DeleteProductAsync(int id, int userId) 
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Producto no encontrado");
        if (product.UserId != userId) throw new UnauthorizedAccessException("No puedes eliminar un producto que no es tuyo");
        await _productRepository.DeleteAsync(product);
    }

    public async Task ToggleFavouriteAsync(int id, int userId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Producto no encontrado.");

        // 🔥 PROTECCIÓN IDOR
        if (product.UserId != userId)
            throw new UnauthorizedAccessException("No podés editar este producto.");

        // Invertimos el valor (si era true pasa a false, y viceversa)
        product.IsFavorite = !product.IsFavorite;
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