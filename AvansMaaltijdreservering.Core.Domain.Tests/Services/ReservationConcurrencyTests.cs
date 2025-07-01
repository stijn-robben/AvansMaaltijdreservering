using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;
using FluentAssertions;
using Moq;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Services;

public class ReservationConcurrencyTests
{
    private readonly Mock<IPackageRepository> _packageRepositoryMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly PackageLockService _lockService;
    private readonly ReservationService _reservationService;

    public ReservationConcurrencyTests()
    {
        _packageRepositoryMock = new Mock<IPackageRepository>();
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _studentServiceMock = new Mock<IStudentService>();
        _loggerMock = new Mock<ILoggerService>();
        _lockService = new PackageLockService(); // Use real lock service for concurrency testing
        _reservationService = new ReservationService(_packageRepositoryMock.Object, _studentRepositoryMock.Object, _studentServiceMock.Object, _lockService, _loggerMock.Object);
    }

    [Fact]
    public async Task MakeReservationAsync_WhenMultipleStudentsTryToReserveSamePackage_ShouldOnlyAllowOneReservation()
    {
        // Arrange
        var packageId = 1;
        var package = new Package
        {
            Id = packageId,
            Name = "Test Package",
            PickupTime = DateTime.Today.AddDays(1),
            ReservedByStudentId = null,
            Is18Plus = false
        };

        var student1 = new Student { Id = 1, DateOfBirth = DateTime.Today.AddYears(-20), NoShowCount = 0, Reservations = new List<Package>() };
        var student2 = new Student { Id = 2, DateOfBirth = DateTime.Today.AddYears(-20), NoShowCount = 0, Reservations = new List<Package>() };
        var student3 = new Student { Id = 3, DateOfBirth = DateTime.Today.AddYears(-20), NoShowCount = 0, Reservations = new List<Package>() };

        // Track how many times package is updated
        var updateCount = 0;
        Package? lastUpdatedPackage = null;

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(packageId))
            .ReturnsAsync(() => package); // Return the current state of package

        _packageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Package>()))
            .Callback<Package>(p => 
            {
                // Simulate the database update
                package.ReservedByStudentId = p.ReservedByStudentId;
                lastUpdatedPackage = p;
                updateCount++;
            })
            .ReturnsAsync((Package p) => p);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(student1);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(student2);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(student3);

        _studentServiceMock.Setup(x => x.IsStudentBlockedAsync(It.IsAny<int>())).ReturnsAsync(false);

        var results = new List<Task<Package>>();
        var exceptions = new List<Exception>();

        // Act - Multiple students try to reserve the same package concurrently
        var tasks = new[]
        {
            Task.Run(async () =>
            {
                try
                {
                    return await _reservationService.MakeReservationAsync(packageId, 1);
                }
                catch (Exception ex)
                {
                    lock (exceptions) exceptions.Add(ex);
                    throw;
                }
            }),
            Task.Run(async () =>
            {
                try
                {
                    return await _reservationService.MakeReservationAsync(packageId, 2);
                }
                catch (Exception ex)
                {
                    lock (exceptions) exceptions.Add(ex);
                    throw;
                }
            }),
            Task.Run(async () =>
            {
                try
                {
                    return await _reservationService.MakeReservationAsync(packageId, 3);
                }
                catch (Exception ex)
                {
                    lock (exceptions) exceptions.Add(ex);
                    throw;
                }
            })
        };

        var completedTasks = 0;
        var successfulReservations = new List<Package>();

        foreach (var task in tasks)
        {
            try
            {
                var result = await task;
                successfulReservations.Add(result);
                completedTasks++;
            }
            catch
            {
                // Expected for the students who couldn't reserve
                completedTasks++;
            }
        }

        // Assert
        completedTasks.Should().Be(3, "all tasks should complete");
        successfulReservations.Should().HaveCount(1, "only one student should successfully reserve");
        exceptions.Should().HaveCount(2, "two students should get exceptions");
        
        // The package should be reserved by exactly one student
        lastUpdatedPackage.Should().NotBeNull();
        lastUpdatedPackage!.ReservedByStudentId.Should().BeOneOf(1, 2, 3);
        
        // Should have only the successful reservation
        package.ReservedByStudentId.Should().BeOneOf(1, 2, 3);
        
        exceptions.Should().AllSatisfy(ex => 
            ex.Should().BeOfType<InvalidOperationException>()
            .Which.Message.Should().Contain("already reserved"));
    }

    [Fact]
    public async Task MakeReservationAsync_WhenDifferentPackagesReservedConcurrently_ShouldAllowAllReservations()
    {
        // Arrange
        var package1 = new Package { Id = 1, ReservedByStudentId = null, Is18Plus = false, PickupTime = DateTime.Today.AddDays(1) };
        var package2 = new Package { Id = 2, ReservedByStudentId = null, Is18Plus = false, PickupTime = DateTime.Today.AddDays(1) };
        
        var student1 = new Student { Id = 1, DateOfBirth = DateTime.Today.AddYears(-20), NoShowCount = 0, Reservations = new List<Package>() };
        var student2 = new Student { Id = 2, DateOfBirth = DateTime.Today.AddYears(-20), NoShowCount = 0, Reservations = new List<Package>() };

        _packageRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(package1);
        _packageRepositoryMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(package2);
        _packageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Package>())).ReturnsAsync((Package p) => p);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(student1);
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(student2);
        _studentServiceMock.Setup(x => x.IsStudentBlockedAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act - Different students reserve different packages
        var tasks = new[]
        {
            _reservationService.MakeReservationAsync(1, 1),
            _reservationService.MakeReservationAsync(2, 2)
        };

        var results = await Task.WhenAll(tasks);

        // Assert - Both reservations should succeed
        results.Should().HaveCount(2);
        results[0].ReservedByStudentId.Should().Be(1);
        results[1].ReservedByStudentId.Should().Be(2);
    }
}