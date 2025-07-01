using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Package
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
    
    public int? ReservedByStudentId { get; set; }
    public virtual Student? ReservedByStudent { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    
    public bool IsReserved => ReservedByStudentId.HasValue;
    
    public bool CanBeModified => !IsReserved;
    
    public bool IsValidPickupTime()
    {
        var maxDaysAhead = DateTime.Today.AddDays(2);
        return PickupTime.Date <= maxDaysAhead && PickupTime > DateTime.Now;
    }
    
    public bool ContainsAlcohol()
    {
        return Products.Any(p => p.ContainsAlcohol);
    }
}
