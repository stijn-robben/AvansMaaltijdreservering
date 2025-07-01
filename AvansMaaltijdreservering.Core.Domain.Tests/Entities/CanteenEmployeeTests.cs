using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using FluentAssertions;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Entities;

public class CanteenEmployeeTests
{
    [Fact]
    public void WorksAtCanteen_WhenEmployeeWorksAtSpecifiedCanteen_ShouldReturnTrue()
    {
        // Arrange
        var canteenId = 5;
        var employee = new CanteenEmployee
        {
            Id = 1,
            Name = "John Doe",
            EmployeeNumber = "EMP001",
            CanteenId = canteenId
        };

        // Act
        var worksAtCanteen = employee.WorksAtCanteen(canteenId);

        // Assert
        worksAtCanteen.Should().BeTrue();
    }

    [Fact]
    public void WorksAtCanteen_WhenEmployeeDoesNotWorkAtSpecifiedCanteen_ShouldReturnFalse()
    {
        // Arrange
        var employeeCanteenId = 5;
        var otherCanteenId = 3;
        var employee = new CanteenEmployee
        {
            Id = 1,
            Name = "John Doe", 
            EmployeeNumber = "EMP001",
            CanteenId = employeeCanteenId
        };

        // Act
        var worksAtCanteen = employee.WorksAtCanteen(otherCanteenId);

        // Assert
        worksAtCanteen.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(999)]
    public void WorksAtCanteen_WithDifferentCanteenIds_ShouldWorkCorrectly(int canteenId)
    {
        // Arrange
        var employee = new CanteenEmployee
        {
            CanteenId = canteenId
        };

        // Act & Assert
        employee.WorksAtCanteen(canteenId).Should().BeTrue();
        employee.WorksAtCanteen(canteenId + 1).Should().BeFalse();
        employee.WorksAtCanteen(canteenId - 1).Should().BeFalse();
    }
}