using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Students;

public class PackageDetailsViewModel
{
    public Package Package { get; set; } = new();
    public Student Student { get; set; } = new();
    public bool CanReserve { get; set; }
    public bool IsAvailable { get; set; }
}