using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IPackageService
{
    Task<IEnumerable<Package>> GetAvailablePackagesAsync();
    Task<IEnumerable<Package>> GetPackagesByCanteenAsync(CanteenLocation canteenLocation);
    Task<IEnumerable<Package>> GetPackagesByCityAsync(City city);
    Task<IEnumerable<Package>> GetPackagesByMealTypeAsync(MealType mealType);
    Task<Package?> GetPackageByIdAsync(int packageId);
    Task<Package> CreatePackageAsync(Package package, int canteenEmployeeId);
    Task<Package> UpdatePackageAsync(Package package, int canteenEmployeeId);
    Task DeletePackageAsync(int packageId, int canteenEmployeeId);
    Task ValidatePackageBusinessRulesAsync(Package package);
    Task<bool> CanEmployeeModifyPackageAsync(int packageId, int canteenEmployeeId);
}
