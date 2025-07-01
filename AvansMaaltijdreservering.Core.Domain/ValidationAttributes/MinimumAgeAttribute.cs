using System.ComponentModel.DataAnnotations;

namespace AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
        ErrorMessage = $"Must be at least {minimumAge} years old";
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            return age >= _minimumAge;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must indicate an age of at least {_minimumAge} years old.";
    }
}