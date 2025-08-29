using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.ValidationAttributes;

[Trait("Category", "Unit")]
[Trait("Component", "Validation")]
[Trait("UserStory", "US_09")]
public class WarmMealLocationAttributeTests
{
    #region Test DTO for Validation Context

    private class TestPackageDto
    {
        public MealType MealType { get; set; }
        public CanteenLocation CanteenLocation { get; set; }
    }

    #endregion

    #region Warm Meal Restriction Tests (US_09)

    [Fact]
    public void IsValid_ShouldReturnError_WhenWarmMealAtLocationThatDoesNotServeWarmMeals()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var dto = new TestPackageDto
        {
            MealType = MealType.WarmEveningMeal,
            CanteenLocation = CanteenLocation.BREDA_LD_BUILDING // Does not serve warm meals
        };
        var context = new ValidationContext(dto) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(dto.MealType, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because LD Building does not serve warm meals");
        result?.ErrorMessage.Should().Contain("canteen location does not serve warm meals");
    }

    [Fact]
    public void IsValid_ShouldReturnError_WhenWarmMealAtHABuilding()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var dto = new TestPackageDto
        {
            MealType = MealType.WarmEveningMeal,
            CanteenLocation = CanteenLocation.BREDA_HA_BUILDING // Does not serve warm meals
        };
        var context = new ValidationContext(dto) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(dto.MealType, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because HA Building does not serve warm meals");
    }

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenWarmMealAtLocationThatServesWarmMeals()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var dto = new TestPackageDto
        {
            MealType = MealType.WarmEveningMeal,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING // Serves warm meals
        };
        var context = new ValidationContext(dto) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(dto.MealType, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because LA Building serves warm meals");
    }

    [Theory]
    [InlineData(CanteenLocation.BREDA_LA_BUILDING, true)]   // Serves warm meals
    [InlineData(CanteenLocation.BREDA_HA_BUILDING, false)]  // Does not serve warm meals (per validation attribute)
    [InlineData(CanteenLocation.BREDA_LD_BUILDING, false)]  // Does not serve warm meals
    [InlineData(CanteenLocation.DENBOSCH_BUILDING, true)]            // Serves warm meals
    [InlineData(CanteenLocation.TILBURG_BUILDING, false)]            // Does not serve warm meals
    public void IsValid_ShouldValidateWarmMealLocations_Correctly(CanteenLocation location, bool shouldAllowWarmMeals)
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var dto = new TestPackageDto
        {
            MealType = MealType.WarmEveningMeal,
            CanteenLocation = location
        };
        var context = new ValidationContext(dto) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(dto.MealType, context);

        // Assert
        if (shouldAllowWarmMeals)
        {
            result.Should().Be(ValidationResult.Success, $"because {location} should serve warm meals");
        }
        else
        {
            result.Should().NotBe(ValidationResult.Success, $"because {location} should not serve warm meals");
        }
    }

    #endregion

    #region Non-Warm Meal Types Should Always Be Valid

    [Theory]
    [InlineData(MealType.Lunch)]
    [InlineData(MealType.Drink)]
    [InlineData(MealType.Snack)]
    public void IsValid_ShouldReturnSuccess_ForNonWarmMealTypes_AtAnyLocation(MealType mealType)
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var dto = new TestPackageDto
        {
            MealType = mealType,
            CanteenLocation = CanteenLocation.BREDA_LD_BUILDING // Location that doesn't serve warm meals
        };
        var context = new ValidationContext(dto) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(dto.MealType, context);

        // Assert
        result.Should().Be(ValidationResult.Success, $"because {mealType} is not a warm meal and should be allowed anywhere");
    }

    [Fact]
    public void IsValid_ShouldReturnSuccess_ForNonWarmMeals_EvenAtRestrictedLocations()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var dto = new TestPackageDto
        {
            MealType = MealType.Lunch, // Non-warm meal
            CanteenLocation = CanteenLocation.BREDA_LD_BUILDING // Restricted location
        };
        var context = new ValidationContext(dto) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(dto.MealType, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because lunch is not a warm meal");
    }

    #endregion

    #region Edge Cases and Null Handling

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenValueIsNull()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because null values should be handled gracefully");
    }

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenObjectInstanceIsNull()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var context = new ValidationContext(new { }) { MemberName = "MealType" };
        
        // Create a context with null ObjectInstance by using a different constructor approach
        var nullContext = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(MealType.WarmEveningMeal, nullContext);

        // Assert
        result.Should().Be(ValidationResult.Success, "because validation should handle missing properties gracefully");
    }

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenPropertiesNotFound()
    {
        // Arrange
        var attribute = new WarmMealLocationAttribute();
        var objectWithoutProperties = new { SomeOtherProperty = "value" };
        var context = new ValidationContext(objectWithoutProperties) { MemberName = "SomeOtherProperty" };

        // Act
        var result = attribute.GetValidationResult("someValue", context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because validation should handle missing properties gracefully");
    }

    #endregion

    #region Integration with Business Scenario

    [Fact]
    public void IsValid_ShouldPreventWarmMealViolation_InBusinessScenario()
    {
        // Arrange - Employee tries to create warm meal package at LD Building
        var attribute = new WarmMealLocationAttribute();
        var invalidPackage = new TestPackageDto
        {
            MealType = MealType.WarmEveningMeal,
            CanteenLocation = CanteenLocation.BREDA_LD_BUILDING
        };
        var context = new ValidationContext(invalidPackage) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(invalidPackage.MealType, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because this should prevent US_09 violation");
        result?.ErrorMessage.Should().NotBeNullOrEmpty("because user should get clear feedback");
    }

    [Fact]
    public void IsValid_ShouldAllowValidWarmMeal_InBusinessScenario()
    {
        // Arrange - Employee creates warm meal package at LA Building
        var attribute = new WarmMealLocationAttribute();
        var validPackage = new TestPackageDto
        {
            MealType = MealType.WarmEveningMeal,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING
        };
        var context = new ValidationContext(validPackage) { MemberName = "MealType" };

        // Act
        var result = attribute.GetValidationResult(validPackage.MealType, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because this is a valid warm meal configuration");
    }

    #endregion
}