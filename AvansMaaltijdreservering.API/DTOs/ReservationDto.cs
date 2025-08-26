using System.ComponentModel.DataAnnotations;

namespace AvansMaaltijdreservering.API.DTOs;

public class ReservationDto
{
    [Required(ErrorMessage = "Package ID is required")]
    public int PackageId { get; set; }
}

public class NoShowDto
{
    [Required(ErrorMessage = "Package ID is required")]
    public int PackageId { get; set; }
}