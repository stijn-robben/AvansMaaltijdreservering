using System.ComponentModel.DataAnnotations;

namespace AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

public class MaxDaysAheadAttribute : ValidationAttribute
{
    private readonly int _maxDays;

    public MaxDaysAheadAttribute(int maxDays)
    {
        _maxDays = maxDays;
        ErrorMessage = $"Date cannot be more than {maxDays} days in the future";
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            var maxDate = DateTime.Today.AddDays(_maxDays);
            return dateTime.Date <= maxDate;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field cannot be more than {_maxDays} days in the future.";
    }
}