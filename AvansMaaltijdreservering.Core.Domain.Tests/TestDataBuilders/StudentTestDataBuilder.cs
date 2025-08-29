using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Tests.TestDataBuilders;

public static class StudentTestDataBuilder
{
    public static Student CreateAdultStudent(string name = "John Adult", int ageOffset = 0)
    {
        return new Student
        {
            Id = 1,
            Name = name,
            DateOfBirth = DateTime.Today.AddYears(-25 + ageOffset), // 25 years old by default
            StudentNumber = "STU001",
            Email = "john.adult@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "+31612345678",
            NoShowCount = 0,
            Reservations = new List<Package>()
        };
    }

    public static Student CreateMinorStudent(string name = "Jane Minor", int ageOffset = 0)
    {
        return new Student
        {
            Id = 2,
            Name = name,
            DateOfBirth = DateTime.Today.AddYears(-17 + ageOffset), // 17 years old by default
            StudentNumber = "STU002",
            Email = "jane.minor@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "+31687654321",
            NoShowCount = 0,
            Reservations = new List<Package>()
        };
    }

    public static Student CreateBlockedStudent(string name = "Bob Blocked")
    {
        return new Student
        {
            Id = 3,
            Name = name,
            DateOfBirth = DateTime.Today.AddYears(-20),
            StudentNumber = "STU003",
            Email = "bob.blocked@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "+31654321987",
            NoShowCount = 2, // Blocked due to no-shows
            Reservations = new List<Package>()
        };
    }

    public static Student CreateStudentWithReservation(Package reservation)
    {
        var student = CreateAdultStudent("Student With Reservation");
        student.Id = 4;
        student.Reservations.Add(reservation);
        return student;
    }

    public static Student CreateStudentBornOn(DateTime birthDate, string name = "Test Student")
    {
        return new Student
        {
            Id = 5,
            Name = name,
            DateOfBirth = birthDate,
            StudentNumber = "STU005",
            Email = "test.student@avans.nl",
            StudyCity = City.BREDA,
            PhoneNumber = "+31612345678",
            NoShowCount = 0,
            Reservations = new List<Package>()
        };
    }

    public static Student WithNoShowCount(this Student student, int noShowCount)
    {
        student.NoShowCount = noShowCount;
        return student;
    }

    public static Student WithReservation(this Student student, Package package)
    {
        student.Reservations.Add(package);
        return student;
    }
}