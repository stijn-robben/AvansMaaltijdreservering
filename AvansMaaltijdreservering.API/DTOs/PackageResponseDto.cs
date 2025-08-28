using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.API.DTOs;

public class PackageResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public City City { get; set; }
    public CanteenLocation CanteenLocation { get; set; }
    public DateTime PickupTime { get; set; }
    public DateTime LatestPickupTime { get; set; }
    public bool Is18Plus { get; set; }
    public decimal Price { get; set; }
    public MealType MealType { get; set; }
    public bool IsReserved { get; set; }
    public List<ProductResponseDto> Products { get; set; } = new();
    public int? ReservedByStudentId { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool ContainsAlcohol { get; set; }
    public string? PhotoUrl { get; set; }
}

public class StudentResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public City StudyCity { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public int NoShowCount { get; set; }
    public int Age { get; set; }
    public bool IsAdult { get; set; }
    public bool IsBlocked { get; set; }
}