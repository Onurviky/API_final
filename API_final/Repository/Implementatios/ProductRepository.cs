using API_final.Data;
using API_final.Entities;
using API_final.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_final.Repository.Implementatios
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsByRestaurantAsync(int userId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetFavoritesAsync(int userId)
        {
            return await _context.Products
                .Where(p => p.UserId == userId && p.IsFavorite)
                .ToListAsync();
        }

        public async Task<List<Product>> GetDiscountedOrHappyHourAsync(int userId)
        {
            return await _context.Products
                .Where(p => p.UserId == userId && (p.DiscountPercent > 0 || p.HappyHourEnabled))
                .ToListAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // 🔥 EL REQUERIMIENTO EXTRA: AUMENTO MASIVO OPTIMIZADO PARA EF 8 🔥
        public async Task IncreasePricesByPercentageAsync(int userId, decimal percentage)
        {
            // Esto genera un único comando UPDATE en SQL sin traer los datos a la memoria RAM.
            decimal multiplier = 1 + (percentage / 100m);

            await _context.Products
                .Where(p => p.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Price, p => p.Price * multiplier));
        }
    }
}
