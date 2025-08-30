using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Students;

public class StudentDashboardViewModel
{
    public Student Student { get; set; } = new();
    public IEnumerable<PackageEligibilityViewModel> AvailablePackages { get; set; } = new List<PackageEligibilityViewModel>();
    public City FilterCity { get; set; }
    public MealType? FilterMealType { get; set; }
}

public class PackageEligibilityViewModel
{
    public Package Package { get; set; } = new();
    public bool IsEligible { get; set; }
    public bool CanReserve { get; set; }
    public string? IneligibilityReason { get; set; }
}