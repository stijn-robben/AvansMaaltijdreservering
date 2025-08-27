using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

public class WarmMealLocationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        // This attribute should be applied to a class, not a single property
        return true;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        // We expect this to be applied to the CreatePackageDto
        var dto = value;
        var mealTypeProperty = dto.GetType().GetProperty("MealType");
        var canteenLocationProperty = dto.GetType().GetProperty("CanteenLocation");

        if (mealTypeProperty == null || canteenLocationProperty == null)
            return ValidationResult.Success;

        var mealType = (MealType)mealTypeProperty.GetValue(dto)!;
        var canteenLocation = (CanteenLocation)canteenLocationProperty.GetValue(dto)!;

        // Check if trying to create warm meal at location that doesn't serve them
        if (mealType == MealType.WarmEveningMeal)
        {
            // Locations that don't serve warm meals:
            // BREDA_LD_BUILDING (1), BREDA_HA_BUILDING (2)
            if (canteenLocation == CanteenLocation.BREDA_LD_BUILDING || 
                canteenLocation == CanteenLocation.BREDA_HA_BUILDING)
            {
                return new ValidationResult("This canteen location does not serve warm meals.");
            }
        }

        return ValidationResult.Success;
    }
}