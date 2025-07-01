using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public City StudyCity { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public int NoShowCount { get; set; } = 0;
    
    public virtual ICollection<Package> Reservations { get; set; } = new List<Package>();
    
    public int GetAge()
    {
        return DateTime.Today.Year - DateOfBirth.Year - 
               (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
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
