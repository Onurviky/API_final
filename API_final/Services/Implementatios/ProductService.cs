using API_final.Repository.Interfaces;
using API_final.Services.Interfaces;


namespace API_final.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
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
}