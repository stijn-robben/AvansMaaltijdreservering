using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IPackageService
{
    Task<IEnumerable<Package>> GetAvailablePackagesAsync();
    Task<IEnumerable<Package>> GetAllPackagesAsync();
    Task<Package?> GetPackageByIdAsync(int id);
    Task<IEnumerable<Package>> GetPackagesByCanteenAsync(CanteenLocation location);
    Task<IEnumerable<Package>> GetPackagesForEmployeeCanteenAsync(int employeeId);
    Task<IEnumerable<Package>> GetPackagesByCityAsync(City city);
    Task<IEnumerable<Package>> GetPackagesByMealTypeAsync(MealType mealType);
    
    Task<Package> CreatePackageAsync(Package package, int employeeId, List<int>? productIds = null);
    Task<Package> UpdatePackageAsync(Package package, int employeeId, List<int>? productIds = null);
    Task DeletePackageAsync(int packageId, int employeeId);
}