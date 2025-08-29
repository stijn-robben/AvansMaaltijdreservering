using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Interfaces;

public interface IPackageRepository
{
    Task<IEnumerable<Package>> GetAllAsync();
    Task<Package?> GetByIdAsync(int id);
    Task<IEnumerable<Package>> GetAvailablePackagesAsync();
    Task<IEnumerable<Package>> GetPackagesByCanteenAsync(CanteenLocation canteenLocation);
    Task<IEnumerable<Package>> GetPackagesByCanteenIdAsync(int canteenId);
    Task<IEnumerable<Package>> GetPackagesByCityAsync(City city);
    Task<IEnumerable<Package>> GetPackagesByMealTypeAsync(MealType mealType);
    Task<IEnumerable<Package>> GetPackagesByStudentIdAsync(int studentId);
    Task<Package> AddAsync(Package package);
    Task<Package> UpdateAsync(Package package);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
