using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Data;

namespace AvansMaaltijdreservering.Infrastructure.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly ApplicationDbContext _context;

    public PackageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Package>> GetAllAsync()
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Include(p => p.ReservedByStudent)
            .Include(p => p.Canteen)
            .ToListAsync();
    }

    public async Task<Package?> GetByIdAsync(int id)
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Include(p => p.ReservedByStudent)
            .Include(p => p.Canteen)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Package>> GetAvailablePackagesAsync()
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Where(p => p.ReservedByStudentId == null && p.PickupTime > DateTime.Now)
            .OrderBy(p => p.PickupTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetPackagesByCanteenAsync(CanteenLocation canteenLocation)
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Include(p => p.ReservedByStudent)
            .Include(p => p.Canteen)
            .Where(p => p.CanteenLocation == canteenLocation)
            .OrderBy(p => p.PickupTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetPackagesByCanteenIdAsync(int canteenId)
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Include(p => p.ReservedByStudent)
            .Where(p => p.CanteenId == canteenId)
            .OrderBy(p => p.PickupTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetPackagesByCityAsync(City city)
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Include(p => p.ReservedByStudent)
            .Include(p => p.Canteen)
            .Where(p => p.City == city)
            .OrderBy(p => p.PickupTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetPackagesByMealTypeAsync(MealType mealType)
    {
        return await _context.Packages
            .Include(p => p.Products)
            .Include(p => p.ReservedByStudent)
            .Include(p => p.Canteen)
            .Where(p => p.MealType == mealType)
            .OrderBy(p => p.PickupTime)
            .ToListAsync();
    }

    public async Task<Package> AddAsync(Package package)
    {
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task<Package> UpdateAsync(Package package)
    {
        _context.Entry(package).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task DeleteAsync(int id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package != null)
        {
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Packages.AnyAsync(p => p.Id == id);
    }
}
