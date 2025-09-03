using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Exceptions;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Tests.TestDataBuilders;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;
using FluentAssertions;
using Moq;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Services;

[Trait("Category", "Unit")]
[Trait("Component", "DomainService")]
public class ReservationServiceTests
{
    private readonly Mock<IPackageRepository> _mockPackageRepository;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ICanteenEmployeeRepository> _mockEmployeeRepository;
    private readonly Mock<IStudentService> _mockStudentService;
    private readonly Mock<IPackageLockService> _mockLockService;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly ReservationService _reservationService;

    public ReservationServiceTests()
    {
        _mockPackageRepository = new Mock<IPackageRepository>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockEmployeeRepository = new Mock<ICanteenEmployeeRepository>();
        _mockStudentService = new Mock<IStudentService>();
        _mockLockService = new Mock<IPackageLockService>();
        _mockLogger = new Mock<ILoggerService>();

        _reservationService = new ReservationService(
            _mockPackageRepository.Object,
            _mockStudentRepository.Object,
            _mockEmployeeRepository.Object,
            _mockStudentService.Object,
            _mockLockService.Object,
            _mockLogger.Object
        );
    }

    #region US_07: First-Come-First-Served with Locking

    [Fact]
    [Trait("UserStory", "US_07")]
    public async Task ReservePackageAsync_ShouldThrowReservationException_WhenPackageLockFails()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        
        _mockLockService.Setup(x => x.TryLockPackageAsync(It.IsAny<int>(), It.IsAny<TimeSpan>()))
                       .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReservationException>(
            () => _reservationService.ReservePackageAsync(packageId, studentId)
        );

        exception.Message.Should().Contain("currently being reserved by another user");
        _mockPackageRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("UserStory", "US_07")]
    public async Task ReservePackageAsync_ShouldReleaseLock_EvenWhenExceptionOccurs()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;

        _mockLockService.Setup(x => x.TryLockPackageAsync(It.IsAny<int>(), It.IsAny<TimeSpan>()))
                       .ReturnsAsync(true);
        _mockPackageRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                             .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _reservationService.ReservePackageAsync(packageId, studentId)
        );

        _mockLockService.Verify(x => x.ReleasePackageLockAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    [Trait("UserStory", "US_07")]
    public async Task ReservePackageAsync_ShouldThrowUserFriendlyException_WhenPackageAlreadyReserved()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        var reservedPackage = PackageTestDataBuilder.CreateReservedPackage("Already Reserved", 999);

        SetupSuccessfulLocking(packageId);
        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(reservedPackage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReservationException>(
            () => _reservationService.ReservePackageAsync(packageId, studentId)
        );

        exception.Message.Should().Contain("😔 Sorry!");
        exception.Message.Should().Contain("already been reserved by another student");
        exception.Message.Should().Contain("check our other available packages");
    }

    #endregion

    #region US_04: Age Restrictions for Alcohol Packages

    [Fact]
    [Trait("UserStory", "US_04")]
    public async Task ReservePackageAsync_ShouldThrowException_WhenMinorTriesToReserveAlcoholPackage()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        var alcoholPackage = PackageTestDataBuilder.CreateAlcoholPackage();
        var minorStudent = StudentTestDataBuilder.CreateMinorStudent();

        SetupSuccessfulReservationAttempt(packageId, studentId, alcoholPackage, minorStudent);
        
        // Student will be 17 on pickup date (minor)
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReservationException>(
            () => _reservationService.ReservePackageAsync(packageId, studentId)
        );

        exception.Message.Should().Contain("🚫");
        exception.Message.Should().Contain("contains alcohol");
        exception.Message.Should().Contain("18+");
        exception.Message.Should().Contain("17 years old"); // Age on pickup date
    }

    [Fact]
    [Trait("UserStory", "US_04")]
    public async Task ReservePackageAsync_ShouldSucceed_WhenAdultReservesAlcoholPackage()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        var alcoholPackage = PackageTestDataBuilder.CreateAlcoholPackage();
        var adultStudent = StudentTestDataBuilder.CreateAdultStudent();

        SetupSuccessfulReservationAttempt(packageId, studentId, alcoholPackage, adultStudent);
        _mockStudentService.Setup(x => x.CanReservePackageAsync(studentId, alcoholPackage))
                          .ReturnsAsync(true);
        _mockPackageRepository.Setup(x => x.UpdateAsync(It.IsAny<Package>()))
                             .ReturnsAsync(alcoholPackage);

        // Act
        var result = await _reservationService.ReservePackageAsync(packageId, studentId);

        // Assert
        result.Should().NotBeNull();
        result.ReservedByStudentId.Should().Be(studentId);
        _mockPackageRepository.Verify(x => x.UpdateAsync(It.Is<Package>(p => p.ReservedByStudentId == studentId)), Times.Once);
    }

    #endregion

    #region US_05: One Package Per Day Restriction

    [Fact]
    [Trait("UserStory", "US_05")]
    public async Task ReservePackageAsync_ShouldThrowException_WhenStudentAlreadyHasReservationOnSameDay()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        var package = PackageTestDataBuilder.CreateRegularPackage();
        var existingReservation = PackageTestDataBuilder.CreateRegularPackage("Existing Reservation");
        var student = StudentTestDataBuilder.CreateAdultStudent()
                                           .WithReservation(existingReservation);

        SetupSuccessfulReservationAttempt(packageId, studentId, package, student);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReservationException>(
            () => _reservationService.ReservePackageAsync(packageId, studentId)
        );

        exception.Message.Should().Contain("📅");
        exception.Message.Should().Contain("already have a package reservation");
        exception.Message.Should().Contain("one package per day");
    }

    [Fact]
    [Trait("UserStory", "US_05")]
    public async Task ReservePackageAsync_ShouldSucceed_WhenStudentReservesForDifferentDay()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        
        var tomorrowPackage = PackageTestDataBuilder.CreateFuturePackage(1, "Tomorrow Package");
        var dayAfterPackage = PackageTestDataBuilder.CreateFuturePackage(2, "Day After Package");
        var student = StudentTestDataBuilder.CreateAdultStudent()
                                           .WithReservation(tomorrowPackage);

        SetupSuccessfulReservationAttempt(packageId, studentId, dayAfterPackage, student);
        _mockStudentService.Setup(x => x.CanReservePackageAsync(studentId, dayAfterPackage))
                          .ReturnsAsync(true);
        _mockPackageRepository.Setup(x => x.UpdateAsync(It.IsAny<Package>()))
                             .ReturnsAsync(dayAfterPackage);

        // Act
        var result = await _reservationService.ReservePackageAsync(packageId, studentId);

        // Assert
        result.Should().NotBeNull();
        result.ReservedByStudentId.Should().Be(studentId);
    }

    #endregion

    #region US_10: No-Show Management

    [Fact]
    [Trait("UserStory", "US_10")]
    public async Task RegisterNoShowAsync_ShouldIncrementNoShowCount_AndReleasePackage()
    {
        // Arrange
        const int packageId = 1;
        const int employeeId = 1;
        const int studentId = 1;

        var package = PackageTestDataBuilder.CreateReservedPackage("No Show Package", studentId);
        var student = StudentTestDataBuilder.CreateAdultStudent();
        var employee = new CanteenEmployee { Id = employeeId };

        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(package);
        _mockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId))
                              .ReturnsAsync(employee);
        _mockStudentRepository.Setup(x => x.GetByIdAsync(studentId))
                             .ReturnsAsync(student);

        // Act
        await _reservationService.RegisterNoShowAsync(packageId, employeeId);

        // Assert
        student.NoShowCount.Should().Be(1, "because no-show count should be incremented");
        package.ReservedByStudentId.Should().BeNull("because package should be released");
        
        _mockStudentRepository.Verify(x => x.UpdateAsync(student), Times.Once);
        _mockPackageRepository.Verify(x => x.UpdateAsync(package), Times.Once);
    }

    [Fact]
    [Trait("UserStory", "US_10")]
    public async Task RegisterNoShowAsync_ShouldBlockStudent_WhenReachingTwoNoShows()
    {
        // Arrange
        const int packageId = 1;
        const int employeeId = 1;
        const int studentId = 1;

        var package = PackageTestDataBuilder.CreateReservedPackage("No Show Package", studentId);
        var student = StudentTestDataBuilder.CreateAdultStudent().WithNoShowCount(1); // Already has 1 no-show
        var employee = new CanteenEmployee { Id = employeeId };

        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(package);
        _mockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId))
                              .ReturnsAsync(employee);
        _mockStudentRepository.Setup(x => x.GetByIdAsync(studentId))
                             .ReturnsAsync(student);
        
        // Setup for future reservations cancellation
        _mockPackageRepository.Setup(x => x.GetPackagesByStudentIdAsync(studentId))
                             .ReturnsAsync(new List<Package>()); // No future reservations for simplicity

        // Act
        await _reservationService.RegisterNoShowAsync(packageId, employeeId);

        // Assert
        student.NoShowCount.Should().Be(2, "because second no-show should be registered");
        student.IsBlocked().Should().BeTrue("because student should be blocked with 2 no-shows");
        
        // Verify logging occurred for blocking
        _mockLogger.Verify(
            x => x.LogWarning(It.Is<string>(s => s.Contains("blocked") && s.Contains("excessive no-shows"))),
            Times.Once
        );
        
        // Verify that future reservations query was called when student becomes blocked
        _mockPackageRepository.Verify(x => x.GetPackagesByStudentIdAsync(studentId), Times.Once);
    }

    [Fact]
    [Trait("UserStory", "US_10")]
    public async Task RegisterNoShowAsync_ShouldCancelFutureReservations_WhenStudentBecomesBlocked()
    {
        // Arrange
        const int packageId = 1;
        const int employeeId = 1;
        const int studentId = 1;

        var currentPackage = PackageTestDataBuilder.CreateReservedPackage("Current No Show Package", studentId);
        var futurePackage1 = PackageTestDataBuilder.CreateFuturePackage(1, "Future Package 1");
        futurePackage1.ReservedByStudentId = studentId;
        var futurePackage2 = PackageTestDataBuilder.CreateFuturePackage(2, "Future Package 2");
        futurePackage2.ReservedByStudentId = studentId;
        var pastPackage = PackageTestDataBuilder.CreatePastPackage("Past Package");
        pastPackage.ReservedByStudentId = studentId;

        var student = StudentTestDataBuilder.CreateAdultStudent().WithNoShowCount(1); // Already has 1 no-show
        var employee = new CanteenEmployee { Id = employeeId };

        var allStudentPackages = new List<Package> { futurePackage1, futurePackage2, pastPackage };

        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(currentPackage);
        _mockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId))
                              .ReturnsAsync(employee);
        _mockStudentRepository.Setup(x => x.GetByIdAsync(studentId))
                             .ReturnsAsync(student);
        _mockPackageRepository.Setup(x => x.GetPackagesByStudentIdAsync(studentId))
                             .ReturnsAsync(allStudentPackages);

        // Act
        await _reservationService.RegisterNoShowAsync(packageId, employeeId);

        // Assert
        student.IsBlocked().Should().BeTrue("because student should be blocked with 2 no-shows");
        
        // Verify that future packages were unreserved (past packages should not be affected)
        futurePackage1.ReservedByStudentId.Should().BeNull("because future reservation should be cancelled");
        futurePackage2.ReservedByStudentId.Should().BeNull("because future reservation should be cancelled");
        pastPackage.ReservedByStudentId.Should().Be(studentId, "because past packages should not be affected");
        
        // Verify that UpdateAsync was called for each future package
        _mockPackageRepository.Verify(x => x.UpdateAsync(futurePackage1), Times.Once);
        _mockPackageRepository.Verify(x => x.UpdateAsync(futurePackage2), Times.Once);
        _mockPackageRepository.Verify(x => x.UpdateAsync(pastPackage), Times.Never, 
            "because past packages should not be updated");
        
        // Verify logging for cancelled reservations
        _mockLogger.Verify(
            x => x.LogWarning(It.Is<string>(s => s.Contains("Cancelled 2 future reservations"))),
            Times.Once
        );
    }

    [Fact]
    [Trait("UserStory", "US_10")]
    public async Task ReservePackageAsync_ShouldThrowException_WhenStudentIsBlocked()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        var package = PackageTestDataBuilder.CreateRegularPackage();
        var blockedStudent = StudentTestDataBuilder.CreateBlockedStudent();

        SetupSuccessfulReservationAttempt(packageId, studentId, package, blockedStudent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StudentBlockedException>(
            () => _reservationService.ReservePackageAsync(packageId, studentId)
        );

        exception.StudentId.Should().Be(studentId);
        exception.NoShowCount.Should().Be(blockedStudent.NoShowCount);
    }

    #endregion

    #region Get Student Reservations (Performance Fixed)

    [Fact]
    [Trait("UserStory", "US_01")]
    public async Task GetStudentReservationsAsync_ShouldUseOptimizedQuery()
    {
        // Arrange
        const int studentId = 1;
        var expectedReservations = new List<Package>
        {
            PackageTestDataBuilder.CreateReservedPackage("Package 1", studentId),
            PackageTestDataBuilder.CreateReservedPackage("Package 2", studentId)
        };

        _mockPackageRepository.Setup(x => x.GetPackagesByStudentIdAsync(studentId))
                             .ReturnsAsync(expectedReservations);

        // Act
        var result = await _reservationService.GetStudentReservationsAsync(studentId);

        // Assert
        result.Should().BeEquivalentTo(expectedReservations);
        
        // Verify it uses the optimized method, not GetAllAsync
        _mockPackageRepository.Verify(x => x.GetPackagesByStudentIdAsync(studentId), Times.Once);
        _mockPackageRepository.Verify(x => x.GetAllAsync(), Times.Never, 
            "because this would be a performance issue");
    }

    #endregion

    #region Helper Methods

    private void SetupSuccessfulLocking(int packageId)
    {
        _mockLockService.Setup(x => x.TryLockPackageAsync(It.IsAny<int>(), It.IsAny<TimeSpan>()))
                       .ReturnsAsync(true);
        _mockLockService.Setup(x => x.ReleasePackageLockAsync(It.IsAny<int>()))
                       .Returns(Task.CompletedTask);
    }

    private void SetupSuccessfulReservationAttempt(int packageId, int studentId, Package package, Student student)
    {
        SetupSuccessfulLocking(packageId);
        
        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(package);
        _mockStudentRepository.Setup(x => x.GetByIdAsync(studentId))
                             .ReturnsAsync(student);
    }

    #endregion

    #region Cancel Reservation Tests

    [Fact]
    [Trait("UserStory", "US_01")]
    public async Task CancelReservationAsync_ShouldReleasePackage_WhenStudentCancelsOwnReservation()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        var reservedPackage = PackageTestDataBuilder.CreateReservedPackage("Student Package", studentId);

        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(reservedPackage);

        // Act
        await _reservationService.CancelReservationAsync(packageId, studentId);

        // Assert
        reservedPackage.ReservedByStudentId.Should().BeNull("because reservation should be cancelled");
        reservedPackage.ReservedByStudent.Should().BeNull("because student reference should be cleared");
        
        _mockPackageRepository.Verify(x => x.UpdateAsync(reservedPackage), Times.Once);
        _mockLogger.Verify(
            x => x.LogInfo(It.Is<string>(s => s.Contains("cancelled") && s.Contains(packageId.ToString()))),
            Times.Once
        );
    }

    [Fact]
    [Trait("UserStory", "US_01")]
    public async Task CancelReservationAsync_ShouldThrowException_WhenStudentTriesToCancelOthersReservation()
    {
        // Arrange
        const int packageId = 1;
        const int studentId = 1;
        const int otherStudentId = 999;
        var othersPackage = PackageTestDataBuilder.CreateReservedPackage("Others Package", otherStudentId);

        _mockPackageRepository.Setup(x => x.GetByIdAsync(packageId))
                             .ReturnsAsync(othersPackage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _reservationService.CancelReservationAsync(packageId, studentId)
        );

        exception.Message.Should().Contain("Cannot cancel another student's reservation");
    }

    #endregion
}