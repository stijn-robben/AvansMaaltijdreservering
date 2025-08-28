using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.API.DTOs;

public class StudentDashboardDto
{
    public StudentProfileDto Student { get; set; } = new();
    public IEnumerable<PackageResponseDto> AvailablePackages { get; set; } = new List<PackageResponseDto>();
    public IEnumerable<PackageResponseDto> MyReservations { get; set; } = new List<PackageResponseDto>();
    public DashboardStats Stats { get; set; } = new();
}

public class StudentProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsAdult { get; set; }
    public City StudyCity { get; set; }
    public string Email { get; set; } = string.Empty;
    public int NoShowCount { get; set; }
    public bool IsBlocked { get; set; }
}

public class DashboardStats
{
    public int TotalAvailablePackages { get; set; }
    public int AvailablePackagesInMyCity { get; set; }
    public int MyActiveReservations { get; set; }
    public int PackagesICanReserve { get; set; } // Available + age eligible + not blocked
}