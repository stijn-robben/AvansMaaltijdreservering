using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Data;

namespace AvansMaaltijdreservering.Infrastructure.Repositories;

public class CanteenRepository : ICanteenRepository
{
    private readonly ApplicationDbContext _context;

    public CanteenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Canteen>> GetAllAsync()
    {
        return await _context.Canteens
            .Include(c => c.Employees)
            .Include(c => c.Packages)
            .ToListAsync();
    }

    public async Task<Canteen?> GetByIdAsync(int id)
    {
        return await _context.Canteens
            .Include(c => c.Employees)
            .Include(c => c.Packages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Canteen>> GetCanteensByCityAsync(City city)
    {
        return await _context.Canteens
            .Include(c => c.Employees)
            .Include(c => c.Packages)
            .Where(c => c.City == city)
            .ToListAsync();
    }

    public async Task<Canteen?> GetByLocationAsync(CanteenLocation location)
    {
        return await _context.Canteens
            .Include(c => c.Employees)
            .Include(c => c.Packages)
            .FirstOrDefaultAsync(c => c.Location == location);
    }

    public async Task<Canteen> AddAsync(Canteen canteen)
    {
        _context.Canteens.Add(canteen);
        await _context.SaveChangesAsync();
        return canteen;
    }

    public async Task<Canteen> UpdateAsync(Canteen canteen)
    {
        _context.Entry(canteen).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return canteen;
    }

    public async Task DeleteAsync(int id)
    {
        var canteen = await _context.Canteens.FindAsync(id);
        if (canteen != null)
        {
            _context.Canteens.Remove(canteen);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Canteens.AnyAsync(c => c.Id == id);
    }
}