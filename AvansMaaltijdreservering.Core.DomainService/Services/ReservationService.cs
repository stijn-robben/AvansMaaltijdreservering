using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class ReservationService : IReservationService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentService _studentService;

    public ReservationService(
        IPackageRepository packageRepository,
        IStudentRepository studentRepository,
        IStudentService studentService)
    {
        _packageRepository = packageRepository;
        _studentRepository = studentRepository;
        _studentService = studentService;
    }

    public async Task<Package> MakeReservationAsync(int packageId, int studentId)
    {
        await ValidateReservationRulesAsync(packageId, studentId);

        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            throw new ArgumentException("Package not found");

        package.ReservedByStudentId = studentId;
        return await _packageRepository.UpdateAsync(package);
    }

    public async Task CancelReservationAsync(int packageId, int studentId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            throw new ArgumentException("Package not found");

        if (package.ReservedByStudentId != studentId)
            throw new InvalidOperationException("Package is not reserved by this student");

        package.ReservedByStudentId = null;
        package.ReservedByStudent = null;
        await _packageRepository.UpdateAsync(package);
    }

    public async Task<IEnumerable<Package>> GetStudentReservationsAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            return Enumerable.Empty<Package>();

        var allPackages = await _packageRepository.GetAllAsync();
        return allPackages.Where(p => p.ReservedByStudentId == studentId);
    }

    public async Task ValidateReservationRulesAsync(int packageId, int studentId)
    {
        // Check if package exists and is available
        if (!await IsPackageAvailableAsync(packageId))
            throw new InvalidOperationException("Package is not available for reservation");

        var package = await _packageRepository.GetByIdAsync(packageId);
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (package == null)
            throw new ArgumentException("Package not found");
        if (student == null)
            throw new ArgumentException("Student not found");

        // Check if student is eligible for this package
        if (!await IsStudentEligibleForPackageAsync(studentId, packageId))
            throw new InvalidOperationException("Student is not eligible for this package");

        // Check if student already has reservation on this date
        if (await HasStudentReservationOnDateAsync(studentId, package.PickupTime.Date))
            throw new InvalidOperationException("Student already has a reservation on this pickup date");

        // Check if student is blocked
        if (await _studentService.IsStudentBlockedAsync(studentId))
            throw new InvalidOperationException("Student is blocked from making reservations due to no-shows");
    }

    public async Task<bool> IsPackageAvailableAsync(int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        return package != null && 
               !package.IsReserved && 
               package.PickupTime > DateTime.Now;
    }

    public async Task<bool> HasStudentReservationOnDateAsync(int studentId, DateTime date)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        return student?.HasReservationOnDate(date) ?? false;
    }

    public async Task<bool> IsStudentEligibleForPackageAsync(int studentId, int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (package == null || student == null)
            return false;

        // Check 18+ restriction
        if (package.Is18Plus && !student.IsAdult())
            return false;

        // Check if pickup time is relative to student's current age
        var ageAtPickup = DateTime.Today.Year - student.DateOfBirth.Year;
        if (package.PickupTime.Date < DateTime.Today.AddYears(ageAtPickup - student.GetAge()))
        {
            ageAtPickup = student.GetAge();
        }

        if (package.Is18Plus && ageAtPickup < 18)
            return false;

        return true;
    }
}
