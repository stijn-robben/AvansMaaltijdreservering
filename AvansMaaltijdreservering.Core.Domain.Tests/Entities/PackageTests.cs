using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Entities;

public class PackageTests
{
    [Fact]
    public void ContainsAlcohol_WhenPackageHasAlcoholicProduct_ShouldReturnTrue()
    {
        // Arrange
        var alcoholProduct = new Product { Name = "Beer", ContainsAlcohol = true };
        var regularProduct = new Product { Name = "Sandwich", ContainsAlcohol = false };
        
        var package = new Package
        {
            Products = new List<Product> { regularProduct, alcoholProduct }
        };

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeTrue();
    }

    [Fact]
    public void ContainsAlcohol_WhenPackageHasNoAlcoholicProducts_ShouldReturnFalse()
    {
        // Arrange
        var product1 = new Product { Name = "Sandwich", ContainsAlcohol = false };
        var product2 = new Product { Name = "Apple", ContainsAlcohol = false };
        
        var package = new Package
        {
            Products = new List<Product> { product1, product2 }
        };

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeFalse();
    }

    [Fact]
    public void ContainsAlcohol_WhenPackageHasNoProducts_ShouldReturnFalse()
    {
        // Arrange
        var package = new Package
        {
            Products = new List<Product>()
        };

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeFalse();
    }

    [Fact]
    public void IsReserved_WhenPackageIsNotReserved_ShouldReturnFalse()
    {
        // Arrange
        var package = new Package
        {
            ReservedByStudentId = null
        };

        // Act
        var isReserved = package.IsReserved;

        // Assert
        isReserved.Should().BeFalse();
    }

    [Fact]
    public void IsReserved_WhenPackageIsReserved_ShouldReturnTrue()
    {
        // Arrange
        var package = new Package
        {
            ReservedByStudentId = 123
        };

        // Act
        var isReserved = package.IsReserved;

        // Assert
        isReserved.Should().BeTrue();
    }

    [Fact]
    public void CanBeModified_WhenPackageIsNotReserved_ShouldReturnTrue()
    {
        // Arrange
        var package = new Package
        {
            ReservedByStudentId = null
        };

        // Act
        var canBeModified = package.CanBeModified;

        // Assert
        canBeModified.Should().BeTrue();
    }

    [Fact]
    public void CanBeModified_WhenPackageIsReserved_ShouldReturnFalse()
    {
        // Arrange
        var package = new Package
        {
            ReservedByStudentId = 123
        };

        // Act
        var canBeModified = package.CanBeModified;

        // Assert
        canBeModified.Should().BeFalse();
    }

    [Fact]
    public void IsValidPickupTime_WhenPickupTimeIsWithin2DaysAndInFuture_ShouldReturnTrue()
    {
        // Arrange
        var package = new Package
        {
            PickupTime = DateTime.Today.AddDays(1).AddHours(12) // Tomorrow at noon
        };

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidPickupTime_WhenPickupTimeIsMoreThan2DaysAhead_ShouldReturnFalse()
    {
        // Arrange
        var package = new Package
        {
            PickupTime = DateTime.Today.AddDays(3).AddHours(12) // 3 days from now
        };

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValidPickupTime_WhenPickupTimeIsInPast_ShouldReturnFalse()
    {
        // Arrange
        var package = new Package
        {
            PickupTime = DateTime.Now.AddHours(-1) // 1 hour ago
        };

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValidPickupTime_WhenPickupTimeIsExactly2DaysAhead_ShouldReturnTrue()
    {
        // Arrange
        var package = new Package
        {
            PickupTime = DateTime.Today.AddDays(2).AddHours(12) // Exactly 2 days ahead
        };

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeTrue();
    }
}