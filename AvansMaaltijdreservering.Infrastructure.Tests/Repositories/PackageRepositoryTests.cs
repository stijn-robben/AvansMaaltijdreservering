using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Infrastructure.Data;
using AvansMaaltijdreservering.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AvansMaaltijdreservering.Infrastructure.Tests.Repositories;

[Trait("Category", "Integration")]
[Trait("Component", "Repository")]
public class PackageRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PackageRepository _repository;

    public PackageRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options);
        _repository = new PackageRepository(_context);
        
        SeedTestData();
    }

    #region Repository Performance Tests

    [Fact]
    [Trait("UserStory", "Performance")]
    public async Task GetPackagesByStudentIdAsync_ShouldOnlyQueryRelevantPackages()
    {
        // Arrange
        const int studentId = 1;
        await SeedPackagesForStudentTest(studentId);

        // Act
        var result = await _repository.GetPackagesByStudentIdAsync(studentId);

        // Assert
        result.Should().HaveCount(2, "because student 1 has exactly 2 reservations");
        result.Should().AllSatisfy(p => p.ReservedByStudentId.Should().Be(studentId));
        result.Should().AllSatisfy(p => p.Products.Should().NotBeNull("because packages should include products"));
        result.Should().BeInAscendingOrder(p => p.PickupTime, "because packages should be ordered by pickup time");
    }

    [Fact]
    [Trait("UserStory", "US_01")]
    public async Task GetAvailablePackagesAsync_ShouldExcludeReservedAndPastPackages()
    {
        // Arrange - test data already seeded with mix of available/reserved/past packages

        // Act
        var result = await _repository.GetAvailablePackagesAsync();

        // Assert
        result.Should().NotBeEmpty("because there should be available packages");
        result.Should().OnlyContain(p => !p.IsReserved, "because available packages should not be reserved");
        result.Should().OnlyContain(p => p.PickupTime > DateTime.Now, "because available packages should be in the future");
        result.Should().BeInAscendingOrder(p => p.PickupTime, "because packages should be ordered by pickup time");
    }

    #endregion

    #region Entity Framework Integration Tests

    [Fact]
    [Trait("UserStory", "US_06")]
    public async Task GetByIdAsync_ShouldIncludeProductsAndRelatedEntities()
    {
        // Arrange
        var packageId = await CreatePackageWithProducts();

        // Act
        var result = await _repository.GetByIdAsync(packageId);

        // Assert
        result.Should().NotBeNull();
        result!.Products.Should().NotBeEmpty("because package should have products loaded");
        result.Products.Should().HaveCountGreaterThan(0, "because we added products to this package");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var package = new Package
        {
            Name = "Test Package for Update",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14),
            Price = 7.50m,
            MealType = MealType.Lunch
        };

        await _repository.AddAsync(package);
        var originalName = package.Name;
        
        // Act - Update the package
        package.Name = "Updated Package Name";
        package.Price = 9.99m;
        await _repository.UpdateAsync(package);

        // Assert - Retrieve and verify changes
        var updatedPackage = await _repository.GetByIdAsync(package.Id);
        updatedPackage.Should().NotBeNull();
        updatedPackage!.Name.Should().Be("Updated Package Name");
        updatedPackage.Price.Should().Be(9.99m);
        updatedPackage.Name.Should().NotBe(originalName);
    }

    #endregion

    #region Business Logic Integration Tests

    [Fact]
    [Trait("UserStory", "US_08")]
    public async Task GetPackagesByCityAsync_ShouldFilterCorrectly()
    {
        // Arrange - Seed packages for different cities
        await SeedPackagesForDifferentCities();

        // Act
        var bredaPackages = await _repository.GetPackagesByCityAsync(City.BREDA);
        var tilburgPackages = await _repository.GetPackagesByCityAsync(City.TILBURG);

        // Assert
        bredaPackages.Should().NotBeEmpty("because there should be packages in Breda");
        bredaPackages.Should().OnlyContain(p => p.City == City.BREDA);
        
        tilburgPackages.Should().NotBeEmpty("because there should be packages in Tilburg");
        tilburgPackages.Should().OnlyContain(p => p.City == City.TILBURG);
    }

    [Fact]
    [Trait("UserStory", "US_08")]
    public async Task GetPackagesByMealTypeAsync_ShouldFilterCorrectly()
    {
        // Arrange - Use existing seeded data

        // Act
        var lunchPackages = await _repository.GetPackagesByMealTypeAsync(MealType.Lunch);
        var dinnerPackages = await _repository.GetPackagesByMealTypeAsync(MealType.WarmEveningMeal);

        // Assert
        if (lunchPackages.Any())
        {
            lunchPackages.Should().OnlyContain(p => p.MealType == MealType.Lunch);
        }
        
        if (dinnerPackages.Any())
        {
            dinnerPackages.Should().OnlyContain(p => p.MealType == MealType.WarmEveningMeal);
        }
    }

    #endregion

    #region Test Data Setup

    private void SeedTestData()
    {
        // Create basic canteens
        var laCanteen = new Canteen 
        { 
            Location = CanteenLocation.BREDA_LA_BUILDING,
            City = City.BREDA,
            ServesWarmMeals = true
        };
        
        _context.Canteens.Add(laCanteen);
        _context.SaveChanges();

        // Create some basic products
        var sandwich = new Product { Name = "Sandwich", ContainsAlcohol = false };
        var juice = new Product { Name = "Orange Juice", ContainsAlcohol = false };
        var wine = new Product { Name = "Wine", ContainsAlcohol = true };
        
        _context.Products.AddRange(sandwich, juice, wine);
        _context.SaveChanges();

        // Create some basic packages
        var availablePackage = new Package
        {
            Name = "Available Package",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14),
            Price = 5.95m,
            MealType = MealType.Lunch,
            CanteenId = laCanteen.Id,
            Products = new List<Product> { sandwich, juice }
        };

        var pastPackage = new Package
        {
            Name = "Past Package",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(-1).AddHours(12), // Yesterday
            LatestPickupTime = DateTime.Today.AddDays(-1).AddHours(14),
            Price = 4.50m,
            MealType = MealType.Lunch,
            CanteenId = laCanteen.Id
        };

        _context.Packages.AddRange(availablePackage, pastPackage);
        _context.SaveChanges();
    }

    private async Task SeedPackagesForStudentTest(int studentId)
    {
        var student = new Student
        {
            Id = studentId,
            Name = "Test Student",
            DateOfBirth = DateTime.Today.AddYears(-20),
            StudentNumber = "STU001",
            Email = "test@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "+31612345678"
        };

        _context.Students.Add(student);

        var reservedPackage1 = new Package
        {
            Name = "Student Reserved 1",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14),
            Price = 5.95m,
            MealType = MealType.Lunch,
            ReservedByStudentId = studentId
        };

        var reservedPackage2 = new Package
        {
            Name = "Student Reserved 2",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(2).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(2).AddHours(14),
            Price = 7.50m,
            MealType = MealType.Lunch,
            ReservedByStudentId = studentId
        };

        // Package reserved by someone else
        var otherStudentPackage = new Package
        {
            Name = "Other Student Package",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(18),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(20),
            Price = 12.95m,
            MealType = MealType.WarmEveningMeal,
            ReservedByStudentId = 999 // Different student
        };

        _context.Packages.AddRange(reservedPackage1, reservedPackage2, otherStudentPackage);
        await _context.SaveChangesAsync();
    }

    private async Task<int> CreatePackageWithProducts()
    {
        var product1 = new Product { Name = "Test Product 1", ContainsAlcohol = false };
        var product2 = new Product { Name = "Test Product 2", ContainsAlcohol = true };
        
        _context.Products.AddRange(product1, product2);
        await _context.SaveChangesAsync();

        var package = new Package
        {
            Name = "Package With Products",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14),
            Price = 8.95m,
            MealType = MealType.Lunch,
            Products = new List<Product> { product1, product2 }
        };

        _context.Packages.Add(package);
        await _context.SaveChangesAsync();
        
        return package.Id;
    }

    private async Task SeedPackagesForDifferentCities()
    {
        var bredaPackage = new Package
        {
            Name = "Breda Package",
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14),
            Price = 5.95m,
            MealType = MealType.Lunch
        };

        var tilburgPackage = new Package
        {
            Name = "Tilburg Package",
            City = City.TILBURG,
            CanteenLocation = CanteenLocation.TILBURG_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14),
            Price = 6.95m,
            MealType = MealType.WarmEveningMeal
        };

        _context.Packages.AddRange(bredaPackage, tilburgPackage);
        await _context.SaveChangesAsync();
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}