using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Data;

namespace AvansMaaltijdreservering.Infrastructure.Repositories;

public class CanteenEmployeeRepository : ICanteenEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public CanteenEmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CanteenEmployee>> GetAllAsync()
    {
        return await _context.CanteenEmployees
            .Include(ce => ce.Canteen)
            .ToListAsync();
    }

    public async Task<CanteenEmployee?> GetByIdAsync(int id)
    {
        return await _context.CanteenEmployees
            .Include(ce => ce.Canteen)
            .FirstOrDefaultAsync(ce => ce.Id == id);
    }

    public async Task<CanteenEmployee?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        return await _context.CanteenEmployees
            .Include(ce => ce.Canteen)
            .FirstOrDefaultAsync(ce => ce.EmployeeNumber == employeeNumber);
    }

    public async Task<IEnumerable<CanteenEmployee>> GetByCanteenIdAsync(int canteenId)
    {
        return await _context.CanteenEmployees
            .Include(ce => ce.Canteen)
            .Where(ce => ce.CanteenId == canteenId)
            .ToListAsync();
    }

    public async Task<CanteenEmployee> AddAsync(CanteenEmployee employee)
    {
        _context.CanteenEmployees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<CanteenEmployee> UpdateAsync(CanteenEmployee employee)
    {
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _context.CanteenEmployees.FindAsync(id);
        if (employee != null)
        {
            _context.CanteenEmployees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.CanteenEmployees.AnyAsync(ce => ce.Id == id);
    }

    public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber)
    {
        return await _context.CanteenEmployees.AnyAsync(ce => ce.EmployeeNumber == employeeNumber);
    }
}