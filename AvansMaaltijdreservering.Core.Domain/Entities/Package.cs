using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Package
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Package name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Package name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    public City City { get; set; }
    public CanteenLocation CanteenLocation { get; set; }
    
    [FutureDate(allowToday: true)]
    [MaxDaysAhead(2)]
    public DateTime PickupTime { get; set; }
    
    [FutureDate(allowToday: true)]
    [MaxDaysAhead(2)]
    public DateTime LatestPickupTime { get; set; }
    
    // Is18Plus is calculated from products (if any contain alcohol)
    public bool Is18Plus => ContainsAlcohol();
    
    [Range(0.01, 999.99, ErrorMessage = "Price must be between €0.01 and €999.99")]
    public decimal Price { get; set; }
    
    public MealType MealType { get; set; }
    
    public int? ReservedByStudentId { get; set; }
    public virtual Student? ReservedByStudent { get; set; }
    
    public int? CanteenId { get; set; }
    public virtual Canteen? Canteen { get; set; }
    
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
