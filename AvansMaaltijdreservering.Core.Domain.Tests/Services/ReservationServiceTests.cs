using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;
using FluentAssertions;
using Moq;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Services;

public class ReservationServiceTests
{
    private readonly Mock<IPackageRepository> _packageRepositoryMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<IPackageLockService> _lockServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly ReservationService _reservationService;

    public ReservationServiceTests()
    {
        _packageRepositoryMock = new Mock<IPackageRepository>();
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _studentServiceMock = new Mock<IStudentService>();
        _lockServiceMock = new Mock<IPackageLockService>();
        _loggerMock = new Mock<ILoggerService>();
        
        // Setup lock service to execute operations directly (no actual locking in tests)
        _lockServiceMock.Setup(x => x.ExecuteWithPackageLockAsync(It.IsAny<int>(), It.IsAny<Func<Task<Package>>>()))
            .Returns<int, Func<Task<Package>>>((packageId, operation) => operation());
        _lockServiceMock.Setup(x => x.ExecuteWithPackageLockAsync(It.IsAny<int>(), It.IsAny<Func<Task>>()))
            .Returns<int, Func<Task>>((packageId, operation) => operation());
        
        _reservationService = new ReservationService(_packageRepositoryMock.Object, _studentRepositoryMock.Object, _studentServiceMock.Object, _lockServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task MakeReservationAsync_WhenPackageNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync((Package?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _reservationService.MakeReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("Package not found");
    }

    [Fact]
    public async Task MakeReservationAsync_WhenStudentNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var package = new Package { Id = packageId };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _reservationService.MakeReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("Student not found");
    }

    [Fact]
    public async Task MakeReservationAsync_WhenPackageAlreadyReserved_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var package = new Package 
        { 
            Id = packageId,
            ReservedByStudentId = 999 // Already reserved by someone else
        };
        var student = new Student { Id = studentId };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _reservationService.MakeReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("Package is already reserved");
    }

    [Fact]
    public async Task MakeReservationAsync_WhenStudentIsBlocked_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var package = new Package { Id = packageId, ReservedByStudentId = null };
        var blockedStudent = new Student 
        { 
            Id = studentId,
            NoShowCount = 3, // Blocked (>= 2)
            Reservations = new List<Package>()
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(blockedStudent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _reservationService.MakeReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("Student is blocked due to no-shows");
    }

    [Fact]
    public async Task MakeReservationAsync_WhenStudentAlreadyHasReservationOnSameDate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var pickupDate = DateTime.Today.AddDays(1);
        
        var package = new Package 
        { 
            Id = packageId,
            PickupTime = pickupDate.AddHours(12),
            ReservedByStudentId = null
        };
        
        var existingReservation = new Package
        {
            Id = 2,
            PickupTime = pickupDate.AddHours(18), // Same date, different time
            ReservedByStudentId = studentId
        };
        
        var student = new Student 
        { 
            Id = studentId,
            NoShowCount = 0,
            DateOfBirth = DateTime.Today.AddYears(-20), // Adult
            Reservations = new List<Package> { existingReservation }
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _reservationService.MakeReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("already have a reservation on this date");
    }

    [Fact]
    public async Task MakeReservationAsync_WhenMinorTriesToReserveAdultOnlyPackage_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var pickupDate = DateTime.Today.AddDays(1);
        
        var alcoholProduct = new Product { Name = "Beer", ContainsAlcohol = true };
        var package = new Package 
        { 
            Id = packageId,
            PickupTime = pickupDate.AddHours(12),
            ReservedByStudentId = null,
            Is18Plus = true,
            Products = new List<Product> { alcoholProduct }
        };
        
        var minorStudent = new Student 
        { 
            Id = studentId,
            NoShowCount = 0,
            DateOfBirth = DateTime.Today.AddYears(-16), // 16 years old (minor)
            Reservations = new List<Package>()
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(minorStudent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _reservationService.MakeReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("must be 18+ to reserve this package");
    }

    [Fact]
    public async Task MakeReservationAsync_WhenValidReservation_ShouldSucceed()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var pickupDate = DateTime.Today.AddDays(1);
        
        var package = new Package 
        { 
            Id = packageId,
            PickupTime = pickupDate.AddHours(12),
            ReservedByStudentId = null,
            Is18Plus = false
        };
        
        var student = new Student 
        { 
            Id = studentId,
            NoShowCount = 0,
            DateOfBirth = DateTime.Today.AddYears(-20), // Adult
            Reservations = new List<Package>()
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        await _reservationService.MakeReservationAsync(packageId, studentId);

        // Assert
        package.ReservedByStudentId.Should().Be(studentId);
        _packageRepositoryMock.Verify(x => x.UpdateAsync(package), Times.Once);
    }

    [Fact]
    public async Task CancelReservationAsync_WhenPackageNotReservedByStudent_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var package = new Package 
        { 
            Id = packageId,
            ReservedByStudentId = 999 // Reserved by different student
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _reservationService.CancelReservationAsync(packageId, studentId));
        
        exception.Message.Should().Contain("not reserved by you");
    }

    [Fact]
    public async Task CancelReservationAsync_WhenValidCancellation_ShouldSucceed()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var package = new Package 
        { 
            Id = packageId,
            ReservedByStudentId = studentId
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);

        // Act
        await _reservationService.CancelReservationAsync(packageId, studentId);

        // Assert
        package.ReservedByStudentId.Should().BeNull();
        _packageRepositoryMock.Verify(x => x.UpdateAsync(package), Times.Once);
    }

    [Fact]
    public async Task IsStudentEligibleForPackageAsync_WhenStudentIsEligible_ShouldReturnTrue()
    {
        // Arrange
        var packageId = 1;
        var studentId = 1;
        var package = new Package 
        { 
            Id = packageId,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12),
            Is18Plus = false
        };
        var student = new Student 
        { 
            Id = studentId,
            NoShowCount = 0,
            DateOfBirth = DateTime.Today.AddYears(-20),
            Reservations = new List<Package>()
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var isEligible = await _reservationService.IsStudentEligibleForPackageAsync(studentId, packageId);

        // Assert
        isEligible.Should().BeTrue();
    }

    [Fact]
    public async Task IsPackageAvailableAsync_WhenPackageIsAvailable_ShouldReturnTrue()
    {
        // Arrange
        var packageId = 1;
        var package = new Package 
        { 
            Id = packageId,
            ReservedByStudentId = null,
            PickupTime = DateTime.Now.AddHours(2) // Not expired
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);

        // Act
        var isAvailable = await _reservationService.IsPackageAvailableAsync(packageId);

        // Assert
        isAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task IsPackageAvailableAsync_WhenPackageIsReserved_ShouldReturnFalse()
    {
        // Arrange
        var packageId = 1;
        var package = new Package 
        { 
            Id = packageId,
            ReservedByStudentId = 123,
            PickupTime = DateTime.Now.AddHours(2)
        };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(package);

        // Act
        var isAvailable = await _reservationService.IsPackageAvailableAsync(packageId);

        // Assert
        isAvailable.Should().BeFalse();
    }
}