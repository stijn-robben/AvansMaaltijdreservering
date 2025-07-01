using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Data;

namespace AvansMaaltijdreservering.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Packages)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Packages)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByPackageIdAsync(int packageId)
    {
        return await _context.Products
            .Include(p => p.Packages)
            .Where(p => p.Packages.Any(pkg => pkg.Id == packageId))
            .ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}
