using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Employees;

public class EmployeeDashboardViewModel
{
    public CanteenEmployee Employee { get; set; } = new();
    public Canteen? EmployeeCanteen { get; set; }
    public List<Package> OwnCanteenPackages { get; set; } = new();
    public List<Package> AllPackages { get; set; } = new();
}