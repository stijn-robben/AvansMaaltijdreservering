using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Students;

public class StudentReservationsViewModel
{
    public Student Student { get; set; } = new();
    public List<Package> Reservations { get; set; } = new();
}