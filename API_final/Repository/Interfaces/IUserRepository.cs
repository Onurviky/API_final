using API_final.Entities;

namespace API_final.Repository.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllRestaurantsAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);

    // El otro requerimiento extra: trackear visitas
    Task IncrementVisitsAsync(int userId);
}