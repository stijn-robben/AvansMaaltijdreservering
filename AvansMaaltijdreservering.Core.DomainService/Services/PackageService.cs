using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly ICanteenRepository _canteenRepository;
    private readonly ICanteenEmployeeRepository _canteenEmployeeRepository;

    public PackageService(
        IPackageRepository packageRepository,
        ICanteenRepository canteenRepository,
        ICanteenEmployeeRepository canteenEmployeeRepository)
    {
        _packageRepository = packageRepository;
        _canteenRepository = canteenRepository;
        _canteenEmployeeRepository = canteenEmployeeRepository;
    }

    public async Task<IEnumerable<Package>> GetAvailablePackagesAsync()
    {
        var allPackages = await _packageRepository.GetAvailablePackagesAsync();
        return allPackages.Where(p => p.PickupTime > DateTime.Now && !p.IsReserved);
    }

    public async Task<IEnumerable<Package>> GetPackagesByCanteenAsync(CanteenLocation canteenLocation)
    {
        return await _packageRepository.GetPackagesByCanteenAsync(canteenLocation);
    }

    public async Task<IEnumerable<Package>> GetPackagesByCityAsync(City city)
    {
        return await _packageRepository.GetPackagesByCityAsync(city);
    }

    public async Task<IEnumerable<Package>> GetPackagesByMealTypeAsync(MealType mealType)
    {
        return await _packageRepository.GetPackagesByMealTypeAsync(mealType);
    }

    public async Task<Package?> GetPackageByIdAsync(int packageId)
    {
        return await _packageRepository.GetByIdAsync(packageId);
    }

    public async Task<Package> CreatePackageAsync(Package package, int canteenEmployeeId)
    {
        var employee = await _canteenEmployeeRepository.GetByIdAsync(canteenEmployeeId);
        if (employee == null)
            throw new ArgumentException("Canteen employee not found");

        var canteen = await _canteenRepository.GetByLocationAsync(package.CanteenLocation);
        if (canteen == null || !employee.WorksAtCanteen(canteen.Id))
            throw new InvalidOperationException("Employee cannot create packages for this location");

        await ValidatePackageBusinessRulesAsync(package);

        if (package.ContainsAlcohol())
            package.Is18Plus = true;

        return await _packageRepository.AddAsync(package);
    }

    public async Task<Package> UpdatePackageAsync(Package package, int canteenEmployeeId)
    {
        if (!await CanEmployeeModifyPackageAsync(package.Id, canteenEmployeeId))
            throw new InvalidOperationException("Employee cannot modify this package");

        if (package.IsReserved)
            throw new InvalidOperationException("Cannot modify reserved package");

        await ValidatePackageBusinessRulesAsync(package);

        if (package.ContainsAlcohol())
            package.Is18Plus = true;

        return await _packageRepository.UpdateAsync(package);
    }

    public async Task DeletePackageAsync(int packageId, int canteenEmployeeId)
    {
        if (!await CanEmployeeModifyPackageAsync(packageId, canteenEmployeeId))
            throw new InvalidOperationException("Employee cannot delete this package");

        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package?.IsReserved == true)
            throw new InvalidOperationException("Cannot delete reserved package");

        await _packageRepository.DeleteAsync(packageId);
    }

    public async Task ValidatePackageBusinessRulesAsync(Package package)
    {
        if (string.IsNullOrWhiteSpace(package.Name))
            throw new ArgumentException("Package name is required");

        if (package.Price <= 0)
            throw new ArgumentException("Package price must be greater than 0");

        if (!package.IsValidPickupTime())
            throw new ArgumentException("Package can only be planned up to 2 days in advance");

        if (package.PickupTime <= DateTime.Now)
            throw new ArgumentException("Pickup time must be in the future");

        if (package.LatestPickupTime < package.PickupTime)
            throw new ArgumentException("Latest pickup time must be after pickup time");

        var canteen = await _canteenRepository.GetByLocationAsync(package.CanteenLocation);
        if (canteen == null)
            throw new ArgumentException("Invalid canteen location");

        if (package.MealType == MealType.WarmEveningMeal && !canteen.ServesWarmMeals)
            throw new InvalidOperationException("This canteen does not serve warm meals");
    }

    public async Task<bool> CanEmployeeModifyPackageAsync(int packageId, int canteenEmployeeId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        var employee = await _canteenEmployeeRepository.GetByIdAsync(canteenEmployeeId);
        
        if (package == null || employee == null)
            return false;

        var canteen = await _canteenRepository.GetByLocationAsync(package.CanteenLocation);
        return canteen != null && employee.WorksAtCanteen(canteen.Id);
    }
}
