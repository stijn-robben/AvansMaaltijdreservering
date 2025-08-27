using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly ICanteenEmployeeRepository _employeeRepository;
    private readonly ICanteenRepository _canteenRepository;
    private readonly ILoggerService _logger;

    public PackageService(
        IPackageRepository packageRepository,
        ICanteenEmployeeRepository employeeRepository,
        ICanteenRepository canteenRepository,
        ILoggerService logger)
    {
        _packageRepository = packageRepository;
        _employeeRepository = employeeRepository;
        _canteenRepository = canteenRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Package>> GetAvailablePackagesAsync()
    {
        return await _packageRepository.GetAvailablePackagesAsync();
    }

    public async Task<Package?> GetPackageByIdAsync(int id)
    {
        return await _packageRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Package>> GetPackagesByCanteenAsync(CanteenLocation location)
    {
        return await _packageRepository.GetPackagesByCanteenAsync(location);
    }

    public async Task<IEnumerable<Package>> GetPackagesByCityAsync(City city)
    {
        return await _packageRepository.GetPackagesByCityAsync(city);
    }

    public async Task<IEnumerable<Package>> GetPackagesByMealTypeAsync(MealType mealType)
    {
        return await _packageRepository.GetPackagesByMealTypeAsync(mealType);
    }

    public async Task<Package> CreatePackageAsync(Package package, int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            throw new UnauthorizedAccessException("Employee not found");

        await ValidatePackageForEmployeeAsync(package, employee);
        
        // Set the canteen relationship
        package.CanteenId = employee.CanteenId;
        
        // Is18Plus is now automatically calculated from ContainsAlcohol()

        _logger.LogInfo($"Creating package: {package.Name} by employee {employeeId}");
        return await _packageRepository.AddAsync(package);
    }

    public async Task<Package> UpdatePackageAsync(Package package, int employeeId)
    {
        var existing = await _packageRepository.GetByIdAsync(package.Id);
        if (existing == null)
            throw new ArgumentException("Package not found");

        if (!existing.CanBeModified)
            throw new ArgumentException("Cannot modify package with existing reservations");

        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            throw new UnauthorizedAccessException("Employee not found");

        await ValidatePackageForEmployeeAsync(package, employee);

        // Is18Plus is now automatically calculated from ContainsAlcohol()

        _logger.LogInfo($"Updating package: {package.Id} by employee {employeeId}");
        return await _packageRepository.UpdateAsync(package);
    }

    public async Task DeletePackageAsync(int packageId, int employeeId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            throw new ArgumentException("Package not found");

        if (!package.CanBeModified)
            throw new ArgumentException("Cannot delete package with existing reservations");

        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            throw new UnauthorizedAccessException("Employee not found");

        await ValidatePackageForEmployeeAsync(package, employee);

        _logger.LogInfo($"Deleting package: {packageId} by employee {employeeId}");
        await _packageRepository.DeleteAsync(packageId);
    }

    private async Task ValidatePackageForEmployeeAsync(Package package, CanteenEmployee employee)
    {
        var employeeCanteen = await _canteenRepository.GetByIdAsync(employee.CanteenId);
        
        if (employeeCanteen == null)
            throw new UnauthorizedAccessException("Employee canteen not found");

        if (employeeCanteen.Location != package.CanteenLocation)
            throw new UnauthorizedAccessException("Employee can only manage packages for their own canteen");

        if (package.MealType == MealType.WarmEveningMeal && !employeeCanteen.ServesWarmMeals)
            throw new ArgumentException("This canteen does not serve warm meals");

        if (!package.IsValidPickupTime())
            throw new ArgumentException("Package pickup time must be within 2 days");
    }
}