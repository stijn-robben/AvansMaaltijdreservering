using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;
using FluentAssertions;
using Moq;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Services;

public class PackageServiceTests
{
    private readonly Mock<IPackageRepository> _packageRepositoryMock;
    private readonly Mock<ICanteenRepository> _canteenRepositoryMock;
    private readonly Mock<ICanteenEmployeeRepository> _canteenEmployeeRepositoryMock;
    private readonly PackageService _packageService;

    public PackageServiceTests()
    {
        _packageRepositoryMock = new Mock<IPackageRepository>();
        _canteenRepositoryMock = new Mock<ICanteenRepository>();
        _canteenEmployeeRepositoryMock = new Mock<ICanteenEmployeeRepository>();
        _packageService = new PackageService(_packageRepositoryMock.Object, _canteenRepositoryMock.Object, _canteenEmployeeRepositoryMock.Object);
    }

    [Fact]
    public async Task CreatePackageAsync_WhenEmployeeDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        var package = new Package { Name = "Test Package" };
        var employeeId = 1;

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync((CanteenEmployee?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _packageService.CreatePackageAsync(package, employeeId));
        
        exception.Message.Should().Contain("Employee not found");
    }

    [Fact]
    public async Task CreatePackageAsync_WhenPackageHasInvalidPickupTime_ShouldThrowArgumentException()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Id = 1, Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        
        var package = new Package 
        { 
            Name = "Test Package",
            PickupTime = DateTime.Today.AddDays(5), // More than 2 days ahead
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _packageService.CreatePackageAsync(package, 1));
        
        exception.Message.Should().Contain("Pickup time must be within 2 days");
    }

    [Fact]
    public async Task CreatePackageAsync_WhenEmployeeTriesToCreatePackageForDifferentCanteen_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Id = 1, Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        
        var package = new Package 
        { 
            Name = "Test Package",
            PickupTime = DateTime.Today.AddDays(1),
            CanteenLocation = CanteenLocation.TILBURG_BUILDING // Different canteen!
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _packageService.CreatePackageAsync(package, 1));
        
        exception.Message.Should().Contain("only create packages for your own canteen");
    }

    [Fact]
    public async Task CreatePackageAsync_WhenPackageContainsAlcohol_ShouldSetIs18PlusToTrue()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Id = 1, Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        
        var alcoholProduct = new Product { Name = "Beer", ContainsAlcohol = true };
        var package = new Package 
        { 
            Name = "Adult Package",
            PickupTime = DateTime.Today.AddDays(1),
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            Products = new List<Product> { alcoholProduct }
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);

        // Act
        await _packageService.CreatePackageAsync(package, 1);

        // Assert
        package.Is18Plus.Should().BeTrue();
        _packageRepositoryMock.Verify(x => x.AddAsync(package), Times.Once);
    }

    [Fact]
    public async Task CreatePackageAsync_WhenValidPackage_ShouldCreateSuccessfully()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Id = 1, Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        
        var package = new Package 
        { 
            Name = "Valid Package",
            PickupTime = DateTime.Today.AddDays(1),
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            Products = new List<Product> { new Product { Name = "Sandwich", ContainsAlcohol = false } }
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);

        // Act
        await _packageService.CreatePackageAsync(package, 1);

        // Assert
        package.Is18Plus.Should().BeFalse(); // No alcohol
        _packageRepositoryMock.Verify(x => x.AddAsync(package), Times.Once);
    }

    [Fact]
    public async Task UpdatePackageAsync_WhenPackageIsReserved_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var employee = new CanteenEmployee { Id = 1, CanteenId = 1 };
        var reservedPackage = new Package 
        { 
            Id = 1,
            ReservedByStudentId = 123, // Package is reserved
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);
        _packageRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(reservedPackage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _packageService.UpdatePackageAsync(reservedPackage, 1));
        
        exception.Message.Should().Contain("Cannot modify a reserved package");
    }

    [Fact]
    public async Task CanEmployeeModifyPackageAsync_WhenEmployeeCanModify_ShouldReturnTrue()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        var package = new Package 
        { 
            Id = 1,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            ReservedByStudentId = null // Not reserved
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);
        _packageRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(package);

        // Act
        var canModify = await _packageService.CanEmployeeModifyPackageAsync(1, 1);

        // Assert
        canModify.Should().BeTrue();
    }

    [Fact]
    public async Task CanEmployeeModifyPackageAsync_WhenPackageIsFromDifferentCanteen_ShouldReturnFalse()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        var package = new Package 
        { 
            Id = 1,
            CanteenLocation = CanteenLocation.TILBURG_BUILDING, // Different canteen
            ReservedByStudentId = null
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);
        _packageRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(package);

        // Act
        var canModify = await _packageService.CanEmployeeModifyPackageAsync(1, 1);

        // Assert
        canModify.Should().BeFalse();
    }

    [Fact]
    public async Task CanEmployeeModifyPackageAsync_WhenPackageIsReserved_ShouldReturnFalse()
    {
        // Arrange
        var employee = new CanteenEmployee 
        { 
            Id = 1, 
            CanteenId = 1,
            Canteen = new Canteen { Location = CanteenLocation.BREDA_LA_BUILDING }
        };
        var package = new Package 
        { 
            Id = 1,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            ReservedByStudentId = 123 // Package is reserved
        };

        _canteenEmployeeRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);
        _packageRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(package);

        // Act
        var canModify = await _packageService.CanEmployeeModifyPackageAsync(1, 1);

        // Assert
        canModify.Should().BeFalse();
    }
}