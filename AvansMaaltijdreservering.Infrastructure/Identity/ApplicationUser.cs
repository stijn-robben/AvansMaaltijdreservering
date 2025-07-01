using Microsoft.AspNetCore.Identity;

namespace AvansMaaltijdreservering.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? StudentNumber { get; set; }
    public string? EmployeeNumber { get; set; }
    public int? StudentId { get; set; }
    public int? CanteenEmployeeId { get; set; }
}