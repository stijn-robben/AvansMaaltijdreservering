using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.Domain.Interfaces;

public interface ICanteenEmployeeRepository
{
    Task<IEnumerable<CanteenEmployee>> GetAllAsync();
    Task<CanteenEmployee?> GetByIdAsync(int id);
    Task<CanteenEmployee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<IEnumerable<CanteenEmployee>> GetByCanteenIdAsync(int canteenId);
    Task<CanteenEmployee> AddAsync(CanteenEmployee employee);
    Task<CanteenEmployee> UpdateAsync(CanteenEmployee employee);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmployeeNumberExistsAsync(string employeeNumber);
}