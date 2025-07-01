using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

namespace AvansMaaltijdreservering.API.DTOs;

public class StudentRegistrationDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    [MinimumAge(16)]
    public DateTime DateOfBirth { get; set; }
    
    [Required(ErrorMessage = "Student number is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Student number must be between 1 and 20 characters")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Student number can only contain letters and numbers")]
    public string StudentNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Study city is required")]
    public City StudyCity { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 20 characters")]
    public string PhoneNumber { get; set; } = string.Empty;
}