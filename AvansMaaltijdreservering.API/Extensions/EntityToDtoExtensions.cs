using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.API.DTOs;

namespace AvansMaaltijdreservering.API.Extensions;

public static class EntityToDtoExtensions
{
    public static PackageResponseDto ToResponseDto(this Package package)
    {
        return new PackageResponseDto
        {
            Id = package.Id,
            Name = package.Name,
            City = package.City,
            CanteenLocation = package.CanteenLocation,
            PickupTime = package.PickupTime,
            LatestPickupTime = package.LatestPickupTime,
            Is18Plus = package.Is18Plus,
            Price = package.Price,
            MealType = package.MealType,
            IsReserved = package.IsReserved,
            ReservedByStudentId = package.ReservedByStudentId,
            Products = package.Products?.Select(p => p.ToResponseDto()).ToList() ?? new List<ProductResponseDto>()
        };
    }

    public static ProductResponseDto ToResponseDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            ContainsAlcohol = product.ContainsAlcohol,
            PhotoUrl = product.PhotoUrl
        };
    }

    public static StudentResponseDto ToResponseDto(this Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            DateOfBirth = student.DateOfBirth,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            StudyCity = student.StudyCity,
            PhoneNumber = student.PhoneNumber,
            NoShowCount = student.NoShowCount,
            Age = student.GetAge(),
            IsAdult = student.IsAdult(),
            IsBlocked = student.IsBlocked()
        };
    }
}