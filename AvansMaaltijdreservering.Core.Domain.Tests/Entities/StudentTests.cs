using AvansMaaltijdreservering.Core.Domain.Tests.TestDataBuilders;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Component", "Domain")]
public class StudentTests
{
    #region Age Calculation Tests (US_04)

    [Fact]
    [Trait("UserStory", "US_04")]
    public void GetAge_ShouldReturnCorrectAge_WhenBirthdayNotPassedThisYear()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-25).AddDays(1); // Birthday tomorrow, 25 years ago
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var age = student.GetAge();

        // Assert
        age.Should().Be(24, "because birthday hasn't occurred this year yet");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void GetAge_ShouldReturnCorrectAge_WhenBirthdayAlreadyPassedThisYear()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-25).AddDays(-1); // Birthday yesterday, 25 years ago
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var age = student.GetAge();

        // Assert
        age.Should().Be(25, "because birthday has already occurred this year");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void GetAge_ShouldReturnCorrectAge_WhenBirthdayIsToday()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-18); // 18th birthday today
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var age = student.GetAge();

        // Assert
        age.Should().Be(18, "because today is the 18th birthday");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void GetAgeOnDate_ShouldReturnCorrectAge_ForSpecificDate()
    {
        // Arrange
        var birthDate = new DateTime(2000, 6, 15); // Born June 15, 2000
        var checkDate = new DateTime(2023, 6, 14); // Day before 23rd birthday
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var age = student.GetAgeOnDate(checkDate);

        // Assert
        age.Should().Be(22, "because the 23rd birthday hasn't occurred yet on the check date");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void GetAgeOnDate_ShouldReturnCorrectAge_OnExactBirthdayDate()
    {
        // Arrange
        var birthDate = new DateTime(2000, 6, 15); // Born June 15, 2000
        var checkDate = new DateTime(2023, 6, 15); // Exact 23rd birthday
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var age = student.GetAgeOnDate(checkDate);

        // Assert
        age.Should().Be(23, "because it's exactly the 23rd birthday");
    }

    #endregion

    #region Adult Validation Tests (US_04)

    [Fact]
    [Trait("UserStory", "US_04")]
    public void IsAdult_ShouldReturnTrue_WhenStudentIs18OrOlder()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent();

        // Act
        var isAdult = student.IsAdult();

        // Assert
        isAdult.Should().BeTrue("because student is 25 years old");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void IsAdult_ShouldReturnFalse_WhenStudentIsUnder18()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateMinorStudent();

        // Act
        var isAdult = student.IsAdult();

        // Assert
        isAdult.Should().BeFalse("because student is 17 years old");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void IsAdult_ShouldReturnTrue_WhenStudentIsExactly18()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-18); // Exactly 18 today
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var isAdult = student.IsAdult();

        // Assert
        isAdult.Should().BeTrue("because student is exactly 18 years old");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void IsAdultOnDate_ShouldReturnFalse_WhenUnder18OnSpecificDate()
    {
        // Arrange
        var birthDate = new DateTime(2006, 12, 25); // Born Dec 25, 2006
        var checkDate = new DateTime(2024, 12, 24); // Day before 18th birthday
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var isAdultOnDate = student.IsAdultOnDate(checkDate);

        // Assert
        isAdultOnDate.Should().BeFalse("because student is still 17 on the check date");
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public void IsAdultOnDate_ShouldReturnTrue_OnExact18thBirthday()
    {
        // Arrange
        var birthDate = new DateTime(2006, 12, 25); // Born Dec 25, 2006
        var checkDate = new DateTime(2024, 12, 25); // Exact 18th birthday
        var student = StudentTestDataBuilder.CreateStudentBornOn(birthDate);

        // Act
        var isAdultOnDate = student.IsAdultOnDate(checkDate);

        // Assert
        isAdultOnDate.Should().BeTrue("because it's exactly the 18th birthday");
    }

    #endregion

    #region Blocking Tests (US_10)

    [Fact]
    [Trait("UserStory", "US_10")]
    public void IsBlocked_ShouldReturnFalse_WhenNoShowCountIsZero()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent().WithNoShowCount(0);

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeFalse("because student has no no-shows");
    }

    [Fact]
    [Trait("UserStory", "US_10")]
    public void IsBlocked_ShouldReturnFalse_WhenNoShowCountIsOne()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent().WithNoShowCount(1);

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeFalse("because student has only 1 no-show (threshold is 2)");
    }

    [Fact]
    [Trait("UserStory", "US_10")]
    public void IsBlocked_ShouldReturnTrue_WhenNoShowCountIsExactlyTwo()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent().WithNoShowCount(2);

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeTrue("because student has reached the 2 no-show threshold");
    }

    [Fact]
    [Trait("UserStory", "US_10")]
    public void IsBlocked_ShouldReturnTrue_WhenNoShowCountIsMoreThanTwo()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent().WithNoShowCount(5);

        // Act
        var isBlocked = student.IsBlocked();

        // Assert
        isBlocked.Should().BeTrue("because student has exceeded the 2 no-show threshold");
    }

    #endregion

    #region Reservation Management Tests (US_05)

    [Fact]
    [Trait("UserStory", "US_05")]
    public void HasReservationOnDate_ShouldReturnFalse_WhenNoReservations()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent();
        var checkDate = DateTime.Today.AddDays(1);

        // Act
        var hasReservation = student.HasReservationOnDate(checkDate);

        // Assert
        hasReservation.Should().BeFalse("because student has no reservations");
    }

    [Fact]
    [Trait("UserStory", "US_05")]
    public void HasReservationOnDate_ShouldReturnFalse_WhenReservationsCollectionIsNull()
    {
        // Arrange
        var student = StudentTestDataBuilder.CreateAdultStudent();
        student.Reservations = null!; // Simulate null collection
        var checkDate = DateTime.Today.AddDays(1);

        // Act
        var hasReservation = student.HasReservationOnDate(checkDate);

        // Assert
        hasReservation.Should().BeFalse("because reservations collection is null");
    }

    [Fact]
    [Trait("UserStory", "US_05")]
    public void HasReservationOnDate_ShouldReturnTrue_WhenHasReservationOnSameDate()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(1);
        var package = PackageTestDataBuilder.CreateRegularPackage().WithPickupTime(reservationDate.AddHours(12));
        var student = StudentTestDataBuilder.CreateAdultStudent().WithReservation(package);

        // Act
        var hasReservation = student.HasReservationOnDate(reservationDate);

        // Assert
        hasReservation.Should().BeTrue("because student has a reservation on that date");
    }

    [Fact]
    [Trait("UserStory", "US_05")]
    public void HasReservationOnDate_ShouldReturnFalse_WhenReservationOnDifferentDate()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(1);
        var checkDate = DateTime.Today.AddDays(2);
        var package = PackageTestDataBuilder.CreateRegularPackage().WithPickupTime(reservationDate.AddHours(12));
        var student = StudentTestDataBuilder.CreateAdultStudent().WithReservation(package);

        // Act
        var hasReservation = student.HasReservationOnDate(checkDate);

        // Assert
        hasReservation.Should().BeFalse("because reservation is on a different date");
    }

    [Fact]
    [Trait("UserStory", "US_05")]
    public void HasReservationOnDate_ShouldIgnoreTime_WhenCheckingDate()
    {
        // Arrange
        var reservationDateTime = new DateTime(2024, 6, 15, 18, 30, 0); // 6:30 PM
        var checkDate = new DateTime(2024, 6, 15, 12, 0, 0); // 12:00 PM same day
        var package = PackageTestDataBuilder.CreateRegularPackage().WithPickupTime(reservationDateTime);
        var student = StudentTestDataBuilder.CreateAdultStudent().WithReservation(package);

        // Act
        var hasReservation = student.HasReservationOnDate(checkDate);

        // Assert
        hasReservation.Should().BeTrue("because reservation is on the same date (time should be ignored)");
    }

    [Fact]
    [Trait("UserStory", "US_05")]
    public void HasReservationOnDate_ShouldReturnTrue_WhenHasMultipleReservationsIncludingOnDate()
    {
        // Arrange
        var targetDate = DateTime.Today.AddDays(1);
        var otherDate = DateTime.Today.AddDays(2);

        var packageOnTarget = PackageTestDataBuilder.CreateRegularPackage("Package 1")
            .WithPickupTime(targetDate.AddHours(12));
        var packageOnOther = PackageTestDataBuilder.CreateRegularPackage("Package 2")
            .WithPickupTime(otherDate.AddHours(12));

        var student = StudentTestDataBuilder.CreateAdultStudent()
            .WithReservation(packageOnTarget)
            .WithReservation(packageOnOther);

        // Act
        var hasReservation = student.HasReservationOnDate(targetDate);

        // Assert
        hasReservation.Should().BeTrue("because one of the reservations is on the target date");
    }

    #endregion
}