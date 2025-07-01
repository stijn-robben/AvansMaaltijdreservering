using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IReservationService
{
    Task<Package> MakeReservationAsync(int packageId, int studentId);
    Task CancelReservationAsync(int packageId, int studentId);
    Task<IEnumerable<Package>> GetStudentReservationsAsync(int studentId);
    Task ValidateReservationRulesAsync(int packageId, int studentId);
    Task<bool> IsPackageAvailableAsync(int packageId);
    Task<bool> HasStudentReservationOnDateAsync(int studentId, DateTime date);
    Task<bool> IsStudentEligibleForPackageAsync(int studentId, int packageId);
}
