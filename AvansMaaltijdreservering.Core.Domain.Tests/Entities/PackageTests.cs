using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Tests.TestDataBuilders;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Component", "Domain")]
public class PackageTests
{
    #region Alcohol Detection Tests (US_04)

    [Fact]
    [Trait("UserStory", "US_04")]
    public void ContainsAlcohol_ShouldReturnFalse_WhenNoProducts()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.Products.Clear(); // No products

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeFalse("because package has no products");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void ContainsAlcohol_ShouldReturnFalse_WhenProductsCollectionIsNull()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.Products = null!; // Null collection

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeFalse("because products collection is null");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void ContainsAlcohol_ShouldReturnFalse_WhenAllProductsAreNonAlcoholic()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage()
            .WithProducts(
                ProductTestDataBuilder.CreateRegularProduct("Sandwich"),
                ProductTestDataBuilder.CreateRegularProduct("Orange Juice"),
                ProductTestDataBuilder.CreateRegularProduct("Apple")
            );

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeFalse("because all products are non-alcoholic");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void ContainsAlcohol_ShouldReturnTrue_WhenAtLeastOneProductContainsAlcohol()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage()
            .WithProducts(
                ProductTestDataBuilder.CreateRegularProduct("Sandwich"),
                ProductTestDataBuilder.CreateAlcoholProduct("Wine"),
                ProductTestDataBuilder.CreateRegularProduct("Dessert")
            );

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeTrue("because at least one product contains alcohol");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void ContainsAlcohol_ShouldReturnTrue_WhenAllProductsContainAlcohol()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage()
            .WithProducts(
                ProductTestDataBuilder.CreateAlcoholProduct("Wine"),
                ProductTestDataBuilder.CreateAlcoholProduct("Beer"),
                ProductTestDataBuilder.CreateAlcoholProduct("Whiskey")
            );

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeTrue("because all products contain alcohol");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void ContainsAlcohol_ShouldHandleNullProducts_InProductsCollection()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.Products = new List<Product> 
        { 
            ProductTestDataBuilder.CreateRegularProduct("Sandwich"),
            null!, // Null product
            ProductTestDataBuilder.CreateAlcoholProduct("Wine")
        };

        // Act
        var containsAlcohol = package.ContainsAlcohol();

        // Assert
        containsAlcohol.Should().BeTrue("because one valid product contains alcohol, null products should be ignored");
    }

    #endregion

    #region Is18Plus Property Tests (US_04)

    [Fact]
    [Trait("UserStory", "US_04")]
    public void Is18Plus_ShouldReturnTrue_WhenPackageContainsAlcohol()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateAlcoholPackage();

        // Act
        var is18Plus = package.Is18Plus;

        // Assert
        is18Plus.Should().BeTrue("because package contains alcoholic products");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void Is18Plus_ShouldReturnFalse_WhenPackageDoesNotContainAlcohol()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();

        // Act
        var is18Plus = package.Is18Plus;

        // Assert
        is18Plus.Should().BeFalse("because package contains only non-alcoholic products");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void Is18Plus_ShouldReturnFalse_WhenPackageHasNoProducts()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.Products.Clear();

        // Act
        var is18Plus = package.Is18Plus;

        // Assert
        is18Plus.Should().BeFalse("because package has no products");
    }

    #endregion

    #region Reservation Status Tests (US_07)

    [Fact]
    [Trait("UserStory", "US_07")]
    public void IsReserved_ShouldReturnFalse_WhenReservedByStudentIdIsNull()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.ReservedByStudentId = null;

        // Act
        var isReserved = package.IsReserved;

        // Assert
        isReserved.Should().BeFalse("because package is not reserved by any student");
    }

    [Fact]
    [Trait("UserStory", "US_07")]
    public void IsReserved_ShouldReturnTrue_WhenReservedByStudentIdHasValue()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateReservedPackage();

        // Act
        var isReserved = package.IsReserved;

        // Assert
        isReserved.Should().BeTrue("because package is reserved by a student");
    }

    #endregion

    #region Modification Rules Tests (US_03)

    [Fact]
    [Trait("UserStory", "US_03")]
    public void CanBeModified_ShouldReturnTrue_WhenPackageIsNotReserved()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();

        // Act
        var canBeModified = package.CanBeModified;

        // Assert
        canBeModified.Should().BeTrue("because package is not reserved");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public void CanBeModified_ShouldReturnFalse_WhenPackageIsReserved()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateReservedPackage();

        // Act
        var canBeModified = package.CanBeModified;

        // Assert
        canBeModified.Should().BeFalse("because package is reserved and cannot be modified");
    }

    #endregion

    #region Pickup Time Validation Tests (US_03)

    [Fact]
    [Trait("UserStory", "US_03")]
    public void IsValidPickupTime_ShouldReturnTrue_WhenPickupTimeIsTomorrow()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateFuturePackage(1); // Tomorrow

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeTrue("because pickup time is tomorrow (within 2 days)");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public void IsValidPickupTime_ShouldReturnTrue_WhenPickupTimeIsDayAfterTomorrow()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateFuturePackage(2); // Day after tomorrow

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeTrue("because pickup time is day after tomorrow (exactly 2 days ahead)");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public void IsValidPickupTime_ShouldReturnFalse_WhenPickupTimeIsMoreThan2DaysAhead()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateFuturePackage(3); // 3 days from now

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeFalse("because pickup time is more than 2 days ahead");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public void IsValidPickupTime_ShouldReturnFalse_WhenPickupTimeIsInPast()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreatePastPackage();

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeFalse("because pickup time is in the past");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public void IsValidPickupTime_ShouldReturnTrue_WhenPickupTimeIsLaterToday()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.PickupTime = DateTime.Now.AddHours(2); // 2 hours from now

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeTrue("because pickup time is later today");
    }

    [Fact]
    [Trait("UserStory", "US_03")]
    public void IsValidPickupTime_ShouldReturnFalse_WhenPickupTimeIsEarlierToday()
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.PickupTime = DateTime.Now.AddHours(-1); // 1 hour ago

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeFalse("because pickup time is in the past");
    }

    [Theory]
    [Trait("UserStory", "US_03")]
    [InlineData(0)] // Today
    [InlineData(1)] // Tomorrow
    [InlineData(2)] // Day after tomorrow
    public void IsValidPickupTime_ShouldReturnTrue_ForValidDaysAhead(int daysAhead)
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.PickupTime = DateTime.Today.AddDays(daysAhead).AddHours(12); // Noon on the specified day

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeTrue($"because pickup time is {daysAhead} days ahead (within allowed range)");
    }

    [Theory]
    [Trait("UserStory", "US_03")]
    [InlineData(3)]  // 3 days ahead
    [InlineData(7)]  // 1 week ahead
    [InlineData(30)] // 1 month ahead
    public void IsValidPickupTime_ShouldReturnFalse_ForInvalidDaysAhead(int daysAhead)
    {
        // Arrange
        var package = PackageTestDataBuilder.CreateRegularPackage();
        package.PickupTime = DateTime.Today.AddDays(daysAhead).AddHours(12);

        // Act
        var isValid = package.IsValidPickupTime();

        // Assert
        isValid.Should().BeFalse($"because pickup time is {daysAhead} days ahead (exceeds 2-day limit)");
    }

    #endregion
}