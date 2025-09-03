using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Employees;

public class CreatePackageViewModel
{
    [Required(ErrorMessage = "Package name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Package name must be between 3 and 100 characters")]
    [Display(Name = "Package Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "City")]
    public City City { get; set; }

    [Display(Name = "Canteen Location")]
    public CanteenLocation CanteenLocation { get; set; }

    [Required(ErrorMessage = "Pickup time is required")]
    [FutureDate(allowToday: true)]
    [MaxDaysAhead(2)]
    [Display(Name = "Pickup Time")]
    public DateTime PickupTime { get; set; }

    [Required(ErrorMessage = "Latest pickup time is required")]
    [FutureDate(allowToday: true)]
    [MaxDaysAhead(2)]
    [Display(Name = "Latest Pickup Time")]
    public DateTime LatestPickupTime { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Display(Name = "Price (€)")]
    [RegularExpression(@"^[0-9]+([,][0-9]{1,2})?$", ErrorMessage = "Please enter a valid price (e.g., 2,50)")]
    public string PriceString { get; set; } = string.Empty;
    
    // This will be set by the controller after parsing PriceString
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Meal type is required")]
    [Display(Name = "Meal Type")]
    [WarmMealLocation]
    public MealType MealType { get; set; }

    [Display(Name = "Select Products")]
    public List<int> SelectedProductIds { get; set; } = new();

    // Navigation properties (not bound) - exclude from validation
    [ValidateNever]
    public CanteenEmployee Employee { get; set; } = new();
    
    [ValidateNever]
    public Canteen? EmployeeCanteen { get; set; }
    
    [ValidateNever]
    public List<Product> AvailableProducts { get; set; } = new();
}