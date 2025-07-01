using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;
using FluentAssertions;
using Moq;

namespace AvansMaaltijdreservering.Core.Domain.Tests.Services;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _studentService = new StudentService(_studentRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterStudentAsync_WhenStudentIsUnder16_ShouldThrowArgumentException()
    {
        // Arrange
        var youngStudent = new Student
        {
            Name = "Young Student",
            DateOfBirth = DateTime.Today.AddYears(-15), // 15 years old
            StudentNumber = "12345",
            Email = "young@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.RegisterStudentAsync(youngStudent));
        
        exception.Message.Should().Contain("must be at least 16 years old");
    }

    [Fact]
    public async Task RegisterStudentAsync_WhenDateOfBirthIsInFuture_ShouldThrowArgumentException()
    {
        // Arrange
        var futureStudent = new Student
        {
            Name = "Future Student",
            DateOfBirth = DateTime.Today.AddDays(1), // Future date
            StudentNumber = "12345",
            Email = "future@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.RegisterStudentAsync(futureStudent));
        
        exception.Message.Should().Contain("Date of birth cannot be in the future");
    }

    [Fact]
    public async Task RegisterStudentAsync_WhenStudentNumberAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        var existingStudent = new Student
        {
            Id = 1,
            StudentNumber = "12345"
        };
        
        var newStudent = new Student
        {
            Name = "New Student",
            DateOfBirth = DateTime.Today.AddYears(-20),
            StudentNumber = "12345", // Same student number
            Email = "new@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        _studentRepositoryMock.Setup(x => x.GetByStudentNumberAsync("12345"))
            .ReturnsAsync(existingStudent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.RegisterStudentAsync(newStudent));
        
        exception.Message.Should().Contain("Student number already exists");
    }

    [Fact]
    public async Task RegisterStudentAsync_WhenEmailAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        var existingStudent = new Student
        {
            Id = 1,
            Email = "existing@avans.nl"
        };
        
        var newStudent = new Student
        {
            Name = "New Student",
            DateOfBirth = DateTime.Today.AddYears(-20),
            StudentNumber = "12345",
            Email = "existing@avans.nl", // Same email
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        _studentRepositoryMock.Setup(x => x.GetByStudentNumberAsync("12345"))
            .ReturnsAsync((Student?)null);
        _studentRepositoryMock.Setup(x => x.GetByEmailAsync("existing@avans.nl"))
            .ReturnsAsync(existingStudent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.RegisterStudentAsync(newStudent));
        
        exception.Message.Should().Contain("Email already exists");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task RegisterStudentAsync_WhenNameIsEmptyOrWhitespace_ShouldThrowArgumentException(string? name)
    {
        // Arrange
        var student = new Student
        {
            Name = name!,
            DateOfBirth = DateTime.Today.AddYears(-20),
            StudentNumber = "12345",
            Email = "test@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.RegisterStudentAsync(student));
        
        exception.Message.Should().Contain("Name cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task RegisterStudentAsync_WhenStudentNumberIsEmptyOrWhitespace_ShouldThrowArgumentException(string? studentNumber)
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            DateOfBirth = DateTime.Today.AddYears(-20),
            StudentNumber = studentNumber!,
            Email = "test@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.RegisterStudentAsync(student));
        
        exception.Message.Should().Contain("Student number cannot be empty");
    }

    [Fact]
    public async Task RegisterStudentAsync_WhenValidStudent_ShouldRegisterSuccessfully()
    {
        // Arrange
        var validStudent = new Student
        {
            Name = "Valid Student",
            DateOfBirth = DateTime.Today.AddYears(-20), // 20 years old
            StudentNumber = "12345",
            Email = "valid@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "0612345678"
        };

        _studentRepositoryMock.Setup(x => x.GetByStudentNumberAsync("12345"))
            .ReturnsAsync((Student?)null);
        _studentRepositoryMock.Setup(x => x.GetByEmailAsync("valid@avans.nl"))
            .ReturnsAsync((Student?)null);

        // Act
        await _studentService.RegisterStudentAsync(validStudent);

        // Assert
        validStudent.NoShowCount.Should().Be(0); // Should initialize to 0
        _studentRepositoryMock.Verify(x => x.AddAsync(validStudent), Times.Once);
    }

    [Fact]
    public async Task RegisterStudentAsync_WhenStudentIsExactly16_ShouldRegisterSuccessfully()
    {
        // Arrange
        var sixteenYearOld = new Student
        {
            Name = "Sixteen Student",
            DateOfBirth = DateTime.Today.AddYears(-16), // Exactly 16
            StudentNumber = "16789",
            Email = "sixteen@avans.nl",
            StudyCity = City.TILBURG,
            PhoneNumber = "0612345678"
        };

        _studentRepositoryMock.Setup(x => x.GetByStudentNumberAsync("16789"))
            .ReturnsAsync((Student?)null);
        _studentRepositoryMock.Setup(x => x.GetByEmailAsync("sixteen@avans.nl"))
            .ReturnsAsync((Student?)null);

        // Act
        await _studentService.RegisterStudentAsync(sixteenYearOld);

        // Assert
        _studentRepositoryMock.Verify(x => x.AddAsync(sixteenYearOld), Times.Once);
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenStudentNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var student = new Student { Id = 1, Name = "Updated Student" };

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Student?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.UpdateStudentAsync(student));
        
        exception.Message.Should().Contain("Student not found");
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenValidUpdate_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingStudent = new Student 
        { 
            Id = 1, 
            Name = "Original Name",
            Email = "original@avans.nl"
        };
        
        var updatedStudent = new Student 
        { 
            Id = 1, 
            Name = "Updated Name",
            Email = "updated@avans.nl",
            DateOfBirth = DateTime.Today.AddYears(-25),
            StudyCity = City.DENBOSCH
        };

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingStudent);

        // Act
        await _studentService.UpdateStudentAsync(updatedStudent);

        // Assert
        _studentRepositoryMock.Verify(x => x.UpdateAsync(updatedStudent), Times.Once);
    }
}