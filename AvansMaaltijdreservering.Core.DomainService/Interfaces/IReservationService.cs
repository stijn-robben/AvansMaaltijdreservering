using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IReservationService
{
    Task<Package> ReservePackageAsync(int packageId, int studentId);
    Task MakeReservationAsync(int packageId, int studentId);
    Task CancelReservationAsync(int packageId, int studentId);
    Task<IEnumerable<Package>> GetStudentReservationsAsync(int studentId);
    Task RegisterNoShowAsync(int packageId, int employeeId);
    Task<bool> IsStudentEligibleForPackageAsync(int studentId, int packageId);
    Task<bool> IsPackageAvailableAsync(int packageId);
}