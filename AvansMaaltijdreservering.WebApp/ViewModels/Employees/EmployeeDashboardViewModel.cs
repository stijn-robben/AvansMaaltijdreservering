using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Employees;

public class EmployeeDashboardViewModel
{
    public CanteenEmployee Employee { get; set; } = new();
    public Canteen? EmployeeCanteen { get; set; }
    public List<Package> OwnCanteenPackages { get; set; } = new();
    public List<Package> AllPackages { get; set; } = new();
    
    public string? StatusFilter { get; set; }
    public City? CityFilter { get; set; }
}