using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.ValidationAttributes;

[Trait("Category", "Unit")]
[Trait("Component", "Validation")]
[Trait("UserStory", "US_03")]
public class MaxDaysAheadAttributeTests
{
    #region Package Creation Time Limits (US_03)

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenDateIsToday()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var dateTime = DateTime.Today.AddHours(14); // Today at 2 PM
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(dateTime, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because today is within 2 days ahead");
    }

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenDateIsTomorrow()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var dateTime = DateTime.Today.AddDays(1).AddHours(12); // Tomorrow at noon
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(dateTime, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because tomorrow is within 2 days ahead");
    }

    [Fact]
    public void IsValid_ShouldReturnSuccess_WhenDateIsExactly2DaysAhead()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var dateTime = DateTime.Today.AddDays(2).AddHours(12); // Day after tomorrow
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(dateTime, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because 2 days ahead is exactly the limit");
    }

    [Fact]
    public void IsValid_ShouldReturnError_WhenDateIs3DaysAhead()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var dateTime = DateTime.Today.AddDays(3).AddHours(12); // 3 days from now
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(dateTime, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because 3 days ahead exceeds the 2-day limit");
        result?.ErrorMessage.Should().Contain("2 days", "error message should mention the limit");
    }

    [Fact]
    public void IsValid_ShouldReturnError_WhenDateIsMoreThan2DaysAhead()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var dateTime = DateTime.Today.AddDays(7).AddHours(12); // 1 week from now
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(dateTime, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because 7 days ahead far exceeds the limit");
    }

    #endregion

    #region Time Within Day Tests

    [Fact]
    public void IsValid_ShouldIgnoreTimeOfDay_WhenValidatingDayLimit()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var earlyMorning = DateTime.Today.AddDays(2).AddHours(1); // 1 AM on day 2
        var lateMight = DateTime.Today.AddDays(2).AddHours(23); // 11 PM on day 2
        var context = new ValidationContext(new object());

        // Act
        var earlyResult = attribute.GetValidationResult(earlyMorning, context);
        var lateResult = attribute.GetValidationResult(lateMight, context);

        // Assert
        earlyResult.Should().Be(ValidationResult.Success, "because time of day should not matter");
        lateResult.Should().Be(ValidationResult.Success, "because time of day should not matter");
    }

    #endregion

    #region Different Limits

    [Theory]
    [InlineData(1, 0, true)]   // 1-day limit, today
    [InlineData(1, 1, true)]   // 1-day limit, tomorrow
    [InlineData(1, 2, false)]  // 1-day limit, day after tomorrow
    [InlineData(3, 2, true)]   // 3-day limit, 2 days ahead
    [InlineData(3, 3, true)]   // 3-day limit, exactly 3 days
    [InlineData(3, 4, false)]  // 3-day limit, 4 days ahead
    [InlineData(7, 6, true)]   // 7-day limit, 6 days ahead
    [InlineData(7, 8, false)]  // 7-day limit, 8 days ahead
    public void IsValid_ShouldRespectDifferentDayLimits(int maxDays, int daysAhead, bool expectedValid)
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(maxDays);
        var dateTime = DateTime.Today.AddDays(daysAhead).AddHours(12);
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(dateTime, context);

        // Assert
        if (expectedValid)
        {
            result.Should().Be(ValidationResult.Success, $"because {daysAhead} days should be within {maxDays} day limit");
        }
        else
        {
            result.Should().NotBe(ValidationResult.Success, $"because {daysAhead} days should exceed {maxDays} day limit");
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void IsValid_ShouldReturnError_WhenValueIsNull()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because validation attribute returns false for null values");
        result?.ErrorMessage.Should().Contain("2 days", "error message should mention the day limit");
    }

    [Fact]
    public void IsValid_ShouldHandlePastDates_Appropriately()
    {
        // Arrange
        var attribute = new MaxDaysAheadAttribute(2);
        var pastDate = DateTime.Today.AddDays(-1); // Yesterday
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(pastDate, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because past dates are within any future limit (handled by FutureDate attribute)");
    }

    #endregion

    #region Business Rule Integration

    [Fact]
    public void IsValid_ShouldEnforcePackageBusinessRule_For2DayLimit()
    {
        // Arrange - This matches the business requirement from US_03
        var attribute = new MaxDaysAheadAttribute(2); // Exact business rule
        
        var validTomorrow = DateTime.Today.AddDays(1).AddHours(12);
        var validDayAfter = DateTime.Today.AddDays(2).AddHours(18);
        var invalid3Days = DateTime.Today.AddDays(3).AddHours(12);
        
        var context = new ValidationContext(new object());

        // Act
        var tomorrowResult = attribute.GetValidationResult(validTomorrow, context);
        var dayAfterResult = attribute.GetValidationResult(validDayAfter, context);
        var threeDaysResult = attribute.GetValidationResult(invalid3Days, context);

        // Assert
        tomorrowResult.Should().Be(ValidationResult.Success, "because employees should be able to plan packages for tomorrow");
        dayAfterResult.Should().Be(ValidationResult.Success, "because employees should be able to plan packages for day after tomorrow");
        threeDaysResult.Should().NotBe(ValidationResult.Success, "because packages cannot be planned more than 2 days ahead (US_03)");
    }

    #endregion
}