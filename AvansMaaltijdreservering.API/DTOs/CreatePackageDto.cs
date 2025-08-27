using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

namespace AvansMaaltijdreservering.API.DTOs;

[WarmMealLocation]
public class CreatePackageDto
{
    [Required(ErrorMessage = "Package name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Package name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "City is required")]
    public City City { get; set; }
    
    [Required(ErrorMessage = "Canteen location is required")]
    public CanteenLocation CanteenLocation { get; set; }
    
    [Required(ErrorMessage = "Pickup time is required")]
    [DataType(DataType.DateTime)]
    [FutureDate(allowToday: true)]
    [MaxDaysAhead(2)]
    public DateTime PickupTime { get; set; }
    
    [Required(ErrorMessage = "Latest pickup time is required")]
    [DataType(DataType.DateTime)]
    [FutureDate(allowToday: true)]
    [MaxDaysAhead(2)]
    public DateTime LatestPickupTime { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999.99, ErrorMessage = "Price must be between €0.01 and €999.99")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Meal type is required")]
    public MealType MealType { get; set; }
    
    public List<int> ProductIds { get; set; } = new List<int>();
}