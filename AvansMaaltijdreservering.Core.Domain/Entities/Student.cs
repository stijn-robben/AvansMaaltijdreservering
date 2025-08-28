using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Date of birth is required")]
    [MinimumAge(16)]
    public DateTime DateOfBirth { get; set; }
    
    [Required(ErrorMessage = "Student number is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Student number must be between 1 and 20 characters")]
    public string StudentNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    
    public City StudyCity { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "No-show count cannot be negative")]
    public int NoShowCount { get; set; } = 0;
    
    [JsonIgnore]
    public virtual ICollection<Package> Reservations { get; set; } = new List<Package>();
    
    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        
        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }
        
        return age;
    }
    
    public bool IsAdult()
    {
        return GetAge() >= 18;
    }
    
    public bool IsBlocked()
    {
        return NoShowCount >= 2;
    }
    
    public bool HasReservationOnDate(DateTime date)
    {
        return Reservations.Any(r => r.PickupTime.Date == date.Date);
    }
}
