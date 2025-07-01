using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Entities;

public class StudentTests
{
    [Fact]
    public void GetAge_WhenBirthdayNotPassedThisYear_ShouldReturnCorrectAge()
    {
        // Arrange
        var birthDate = new DateTime(2000, 12, 31); // Birthday later this year
        var student = new Student { DateOfBirth = birthDate };

        // Act
        var age = student.GetAge();

        // Assert
        var expectedAge = DateTime.Today.Year - 2000 - 1; // Birthday hasn't passed
        age.Should().Be(expectedAge);
    }

    [Fact]
    public void GetAge_WhenBirthdayPassedThisYear_ShouldReturnCorrectAge()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1); // Birthday earlier this year
        var student = new Student { DateOfBirth = birthDate };

        // Act
        var age = student.GetAge();

        // Assert
        var expectedAge = DateTime.Today.Year - 2000; // Birthday has passed
        age.Should().Be(expectedAge);
    }

    [Fact]
    public void IsAdult_WhenStudentIs18OrOlder_ShouldReturnTrue()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-20); // 20 years old
        var student = new Student { DateOfBirth = birthDate };

        // Act
        var isAdult = student.IsAdult();

        // Assert
        isAdult.Should().BeTrue();
    }

    [Fact]
    public void IsAdult_WhenStudentIsUnder18_ShouldReturnFalse()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-16); // 16 years old
        var student = new Student { DateOfBirth = birthDate };

        // Act
        var isAdult = student.IsAdult();

        // Assert
        isAdult.Should().BeFalse();
    }

    [Fact]
    public void IsAdult_WhenStudentIsExactly18_ShouldReturnTrue()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-18); // Exactly 18 today
        var student = new Student { DateOfBirth = birthDate };

        // Act
        var isAdult = student.IsAdult();

        // Assert
        isAdult.Should().BeTrue();
    }

    [Fact]
    public void IsBlocked_WhenNoShowCountIsLessThan2_ShouldReturnFalse()
    {
        // Arrange
        var student = new Student { NoShowCount = 1 };

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeFalse();
    }

    [Fact]
    public void IsBlocked_WhenNoShowCountIs2OrMore_ShouldReturnTrue()
    {
        // Arrange
        var student = new Student { NoShowCount = 2 };

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void IsBlocked_WhenNoShowCountIsMoreThan2_ShouldReturnTrue(int noShowCount)
    {
        // Arrange
        var student = new Student { NoShowCount = noShowCount };

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeTrue();
    }

    [Fact]
    public void HasReservationOnDate_WhenStudentHasReservationOnDate_ShouldReturnTrue()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(1);
        var package = new Package 
        { 
            Id = 1,
            PickupTime = reservationDate.AddHours(12),
            ReservedByStudentId = 1
        };
        
        var student = new Student 
        { 
            Id = 1,
            Reservations = new List<Package> { package }
        };

        // Act
        var hasReservation = student.HasReservationOnDate(reservationDate);

        // Assert
        hasReservation.Should().BeTrue();
    }

    [Fact]
    public void HasReservationOnDate_WhenStudentHasNoReservationOnDate_ShouldReturnFalse()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(1);
        var differentDate = DateTime.Today.AddDays(2);
        var package = new Package 
        { 
            Id = 1,
            PickupTime = differentDate.AddHours(12),
            ReservedByStudentId = 1
        };
        
        var student = new Student 
        { 
            Id = 1,
            Reservations = new List<Package> { package }
        };

        // Act
        var hasReservation = student.HasReservationOnDate(reservationDate);

        // Assert
        hasReservation.Should().BeFalse();
    }

    [Fact]
    public void HasReservationOnDate_WhenStudentHasNoReservations_ShouldReturnFalse()
    {
        // Arrange
        var student = new Student { Id = 1, Reservations = new List<Package>() };
        var checkDate = DateTime.Today.AddDays(1);

        // Act
        var hasReservation = student.HasReservationOnDate(checkDate);

        // Assert
        hasReservation.Should().BeFalse();
    }
}