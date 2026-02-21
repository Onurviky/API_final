using API_final.Data;
using API_final.Entities;
using API_final.Repository.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace API_final.Repository.Implementatios;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllRestaurantsAsync()
    {
        // TRUCO PARA LA DEFENSA: AsNoTracking()
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // Usamos SingleOrDefault porque por regla de negocio el email debe ser único.
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    //public async Task IncrementVisitsAsync(int userId)
    //{
    //    // Otra vez usamos ExecuteUpdateAsync para no traer el usuario a memoria solo para sumar 1.
    //    // Hacemos un UPDATE directo en SQL.
    //    await _context.Users
    //        .Where(u => u.Id == userId)
    //        .ExecuteUpdateAsync(s => s
    //            .SetProperty(u => u.Visits, u => u.Visits + 1));
    //}
}