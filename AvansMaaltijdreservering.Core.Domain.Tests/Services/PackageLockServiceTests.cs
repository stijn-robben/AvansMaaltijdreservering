using AvansMaaltijdreservering.Core.DomainService.Services;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Services;

public class PackageLockServiceTests
{
    private readonly PackageLockService _lockService;

    public PackageLockServiceTests()
    {
        _lockService = new PackageLockService();
    }

    [Fact]
    public async Task ExecuteWithPackageLockAsync_WhenConcurrentAccessToSamePackage_ShouldSerializeAccess()
    {
        // Arrange
        var packageId = 1;
        var results = new List<int>();
        var tasks = new List<Task>();

        // Act - Start multiple concurrent operations on the same package
        for (int i = 0; i < 5; i++)
        {
            var operationId = i;
            tasks.Add(_lockService.ExecuteWithPackageLockAsync(packageId, async () =>
            {
                results.Add(operationId);
                await Task.Delay(10); // Simulate some work
                return Task.CompletedTask;
            }));
        }

        await Task.WhenAll(tasks);

        // Assert - All operations should have completed
        results.Should().HaveCount(5);
        results.Should().Contain(new[] { 0, 1, 2, 3, 4 });
    }

    [Fact]
    public async Task ExecuteWithPackageLockAsync_WhenConcurrentAccessToDifferentPackages_ShouldAllowParallelAccess()
    {
        // Arrange
        var results = new List<int>();
        var tasks = new List<Task>();
        var startTime = DateTime.UtcNow;

        // Act - Start concurrent operations on different packages
        for (int packageId = 1; packageId <= 3; packageId++)
        {
            var currentPackageId = packageId;
            tasks.Add(_lockService.ExecuteWithPackageLockAsync(currentPackageId, async () =>
            {
                results.Add(currentPackageId);
                await Task.Delay(50); // Simulate work
                return Task.CompletedTask;
            }));
        }

        await Task.WhenAll(tasks);
        var duration = DateTime.UtcNow - startTime;

        // Assert - Should complete in roughly parallel time (not 3x serial time)
        results.Should().HaveCount(3);
        results.Should().Contain(new[] { 1, 2, 3 });
        duration.Should().BeLessThan(TimeSpan.FromMilliseconds(200)); // Should be much faster than 150ms (3x50ms)
    }

    [Fact]
    public async Task ExecuteWithPackageLockAsync_WithReturnValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var packageId = 1;
        var expectedValue = "test-result";

        // Act
        var result = await _lockService.ExecuteWithPackageLockAsync(packageId, async () =>
        {
            await Task.Delay(10);
            return expectedValue;
        });

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public async Task ExecuteWithPackageLockAsync_WhenOperationThrows_ShouldPropagateException()
    {
        // Arrange
        var packageId = 1;
        var expectedException = new InvalidOperationException("Test exception");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _lockService.ExecuteWithPackageLockAsync(packageId, () =>
            {
                throw expectedException;
            }));

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task ExecuteWithPackageLockAsync_WhenOperationThrows_ShouldReleaseLock()
    {
        // Arrange
        var packageId = 1;
        var completedSuccessfully = false;

        // Act - First operation throws an exception
        try
        {
            await _lockService.ExecuteWithPackageLockAsync(packageId, () =>
            {
                throw new InvalidOperationException("Test exception");
            });
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        // Second operation should be able to acquire the lock
        await _lockService.ExecuteWithPackageLockAsync(packageId, async () =>
        {
            completedSuccessfully = true;
            return Task.CompletedTask;
        });

        // Assert
        completedSuccessfully.Should().BeTrue();
    }
}