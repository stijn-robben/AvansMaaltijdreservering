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
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= _minimumAge;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must indicate an age of at least {_minimumAge} years old.";
    }
}