namespace API_final.Services.Interfaces
{
    public interface IProductService
    {
        Task IncreaseAllPricesAsync(int userId, decimal percentage);
    }
}
