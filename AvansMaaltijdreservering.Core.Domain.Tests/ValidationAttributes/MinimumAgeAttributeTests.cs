using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.ValidationAttributes;

[Trait("Category", "Unit")]
[Trait("Component", "Validation")]
public class MinimumAgeAttributeTests
{
    #region Student Registration Tests (Minimum Age 16)

    [Fact]
    [Trait("UserStory", "StudentRegistration")]
    public void IsValid_ShouldReturnTrue_WhenStudentIsExactly16YearsOld()
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var birthDate = DateTime.Today.AddYears(-16); // Exactly 16 today
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(birthDate, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because student is exactly 16 years old");
    }

    [Fact]
    [Trait("UserStory", "StudentRegistration")]
    public void IsValid_ShouldReturnTrue_WhenStudentIsOlderThan16()
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var birthDate = DateTime.Today.AddYears(-25); // 25 years old
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(birthDate, context);

        // Assert
        result.Should().Be(ValidationResult.Success, "because student is older than 16");
    }

    [Fact]
    [Trait("UserStory", "StudentRegistration")]
    public void IsValid_ShouldReturnError_WhenStudentIsYoungerThan16()
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var birthDate = DateTime.Today.AddYears(-15); // Only 15 years old
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(birthDate, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because student is younger than 16");
        result?.ErrorMessage.Should().Contain("16", "error message should mention the minimum age");
    }

    [Fact]
    [Trait("UserStory", "StudentRegistration")]
    public void IsValid_ShouldReturnError_WhenStudentWillBe16Tomorrow()
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var birthDate = DateTime.Today.AddYears(-16).AddDays(1); // 16th birthday is tomorrow
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(birthDate, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because student is not yet 16 today");
    }

    [Fact]
    [Trait("UserStory", "StudentRegistration")]
    public void IsValid_ShouldReturnError_WhenValueIsNull()
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success, "because validation attribute returns false for null values");
        result?.ErrorMessage.Should().Contain("16", "error message should mention the minimum age");
    }

    #endregion

    #region Edge Cases

    [Fact]
    [Trait("UserStory", "StudentRegistration")]
    public void IsValid_ShouldHandleLeapYearBirthdays()
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var leapYearBirthDate = new DateTime(2008, 2, 29); // Born on leap day
        var context = new ValidationContext(new object());

        // Note: This test uses DateTime.Today which may be past the 16th birthday
        // The actual result depends on when the test is run

        // Act
        var result = attribute.GetValidationResult(leapYearBirthDate, context);

        // Assert - Student born in 2008 would be 16+ in 2024
        result.Should().Be(ValidationResult.Success, "because student born in 2008 is now 16+ years old");
    }

    [Theory]
    [Trait("UserStory", "StudentRegistration")]
    [InlineData(15, false)] // Too young
    [InlineData(16, true)]  // Minimum age
    [InlineData(17, true)]  // Above minimum
    [InlineData(25, true)]  // Adult
    public void IsValid_ShouldValidateCorrectly_ForDifferentAges(int ageYears, bool expectedValid)
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(16);
        var birthDate = DateTime.Today.AddYears(-ageYears);
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(birthDate, context);

        // Assert
        if (expectedValid)
        {
            result.Should().Be(ValidationResult.Success, $"because {ageYears} years old should be valid");
        }
        else
        {
            result.Should().NotBe(ValidationResult.Success, $"because {ageYears} years old should be invalid");
        }
    }

    #endregion

    #region Different Minimum Ages

    [Theory]
    [Trait("UserStory", "Validation")]
    [InlineData(18, 17, false)] // 18+ validation, 17 years old
    [InlineData(18, 18, true)]  // 18+ validation, 18 years old
    [InlineData(21, 20, false)] // 21+ validation, 20 years old
    [InlineData(21, 21, true)]  // 21+ validation, 21 years old
    public void IsValid_ShouldRespectDifferentMinimumAges(int minimumAge, int actualAge, bool expectedValid)
    {
        // Arrange
        var attribute = new MinimumAgeAttribute(minimumAge);
        var birthDate = DateTime.Today.AddYears(-actualAge);
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(birthDate, context);

        // Assert
        if (expectedValid)
        {
            result.Should().Be(ValidationResult.Success, $"because {actualAge} meets minimum age {minimumAge}");
        }
        else
        {
            result.Should().NotBe(ValidationResult.Success, $"because {actualAge} does not meet minimum age {minimumAge}");
        }
    }

    #endregion
}