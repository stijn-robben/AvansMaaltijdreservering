using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Interfaces;

namespace AvansMaaltijdreservering.API.GraphQL;

public class Query
{
    // Get all available packages with optional filtering
    public async Task<IEnumerable<Package>> GetPackages(
        [Service] IPackageService packageService,
        City? city = null,
        MealType? mealType = null,
        bool? isAvailable = null)
    {
        var packages = await packageService.GetAvailablePackagesAsync();

        if (city.HasValue)
            packages = packages.Where(p => p.City == city.Value);

        if (mealType.HasValue)
            packages = packages.Where(p => p.MealType == mealType.Value);

        if (isAvailable.HasValue && isAvailable.Value)
            packages = packages.Where(p => p.ReservedById == null);

        return packages;
    }

    // Get a specific package by ID
    public async Task<Package?> GetPackage(
        [Service] IPackageService packageService,
        int id)
    {
        return await packageService.GetPackageByIdAsync(id);
    }

    // Get all products
    public async Task<IEnumerable<Product>> GetProducts(
        [Service] IProductRepository productRepository)
    {
        return await productRepository.GetAllAsync();
    }

    // Get a specific product by ID
    public async Task<Product?> GetProduct(
        [Service] IProductRepository productRepository,
        int id)
    {
        return await productRepository.GetByIdAsync(id);
    }

    // Get products for a specific package
    public async Task<IEnumerable<Product>> GetProductsByPackage(
        [Service] IProductRepository productRepository,
        int packageId)
    {
        return await productRepository.GetProductsByPackageIdAsync(packageId);
    }

    // Get packages by canteen location
    public async Task<IEnumerable<Package>> GetPackagesByCanteen(
        [Service] IPackageService packageService,
        CanteenLocation location)
    {
        return await packageService.GetPackagesByCanteenAsync(location);
    }

    // Get all canteens
    public async Task<IEnumerable<Canteen>> GetCanteens(
        [Service] ICanteenRepository canteenRepository)
    {
        return await canteenRepository.GetAllAsync();
    }

    // Search packages by name or description
    public async Task<IEnumerable<Package>> SearchPackages(
        [Service] IPackageService packageService,
        string searchTerm)
    {
        var packages = await packageService.GetAvailablePackagesAsync();
        
        return packages.Where(p => 
            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (p.Products?.Any(prod => prod.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ?? false));
    }
}