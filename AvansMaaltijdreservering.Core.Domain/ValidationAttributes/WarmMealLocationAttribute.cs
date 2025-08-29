using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

public class WarmMealLocationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) 
            return ValidationResult.Success;

        // Get the object instance (the DTO)
        var objectInstance = validationContext.ObjectInstance;
        if (objectInstance == null) 
            return ValidationResult.Success;

        // Get properties from the object instance
        var mealTypeProperty = objectInstance.GetType().GetProperty("MealType");
        var canteenLocationProperty = objectInstance.GetType().GetProperty("CanteenLocation");

        if (mealTypeProperty == null || canteenLocationProperty == null)
            return ValidationResult.Success;

        var mealTypeValue = mealTypeProperty.GetValue(objectInstance);
        var canteenLocationValue = canteenLocationProperty.GetValue(objectInstance);

        if (mealTypeValue == null || canteenLocationValue == null)
            return ValidationResult.Success;

        var mealType = (MealType)mealTypeValue;
        var canteenLocation = (CanteenLocation)canteenLocationValue;

        // Check if trying to create warm meal at location that doesn't serve them
        if (mealType == MealType.WarmEveningMeal)
        {
            // Locations that don't serve warm meals:
            // BREDA_LD_BUILDING, BREDA_HA_BUILDING, TILBURG_BUILDING
            if (canteenLocation == CanteenLocation.BREDA_LD_BUILDING || 
                canteenLocation == CanteenLocation.BREDA_HA_BUILDING ||
                canteenLocation == CanteenLocation.TILBURG_BUILDING)
            {
                return new ValidationResult(
                    "This canteen location does not serve warm meals.",
                    new[] { validationContext.MemberName ?? "MealType" });
            }
        }

        return ValidationResult.Success;
    }
}