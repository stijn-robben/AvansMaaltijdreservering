using System.ComponentModel.DataAnnotations;

namespace AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

public class FutureDateAttribute : ValidationAttribute
{
    private readonly bool _allowToday;

    public FutureDateAttribute(bool allowToday = false)
    {
        _allowToday = allowToday;
        ErrorMessage = allowToday ? "Date must be today or in the future" : "Date must be in the future";
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            return _allowToday ? dateTime.Date >= DateTime.Today : dateTime.Date > DateTime.Today;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be " + (_allowToday ? "today or in the future." : "in the future.");
    }
}