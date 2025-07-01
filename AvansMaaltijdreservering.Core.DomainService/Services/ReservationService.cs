using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Exceptions;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class ReservationService : IReservationService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentService _studentService;
    private readonly IPackageLockService _lockService;
    private readonly ILoggerService _logger;

    public ReservationService(
        IPackageRepository packageRepository,
        IStudentRepository studentRepository,
        IStudentService studentService,
        IPackageLockService lockService,
        ILoggerService logger)
    {
        _packageRepository = packageRepository;
        _studentRepository = studentRepository;
        _studentService = studentService;
        _lockService = lockService;
        _logger = logger;
    }

    public async Task<Package> MakeReservationAsync(int packageId, int studentId)
    {
        _logger.LogInformation("Starting reservation process for Package {PackageId} by Student {StudentId}", packageId, studentId);

        try
        {
            return await _lockService.ExecuteWithPackageLockAsync(packageId, async () =>
            {
                _logger.LogDebug("Acquired lock for Package {PackageId}", packageId);

                // Re-validate inside the lock to ensure package is still available
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    _logger.LogWarning("Package {PackageId} not found during reservation attempt by Student {StudentId}", packageId, studentId);
                    throw new ArgumentException("Package not found");
                }

                // Check if package was reserved by another thread while we were waiting for the lock
                if (package.ReservedByStudentId != null)
                {
                    _logger.LogInformation("Package {PackageId} already reserved by Student {ExistingStudentId} when Student {StudentId} tried to reserve", 
                        packageId, package.ReservedByStudentId, studentId);
                    throw new ReservationException("Package is already reserved", packageId, studentId);
                }

                // Validate all business rules inside the lock
                await ValidateReservationRulesAsync(packageId, studentId);

                // Atomically reserve the package
                package.ReservedByStudentId = studentId;
                var updatedPackage = await _packageRepository.UpdateAsync(package);

                _logger.LogInformation("Successfully reserved Package {PackageId} for Student {StudentId}", packageId, studentId);
                return updatedPackage;
            });
        }
        catch (Exception ex) when (!(ex is ReservationException || ex is BusinessRuleException))
        {
            _logger.LogError(ex, "Unexpected error during reservation of Package {PackageId} by Student {StudentId}", packageId, studentId);
            throw;
        }
    }

    public async Task CancelReservationAsync(int packageId, int studentId)
    {
        await _lockService.ExecuteWithPackageLockAsync(packageId, async () =>
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
                throw new ArgumentException("Package not found");

            if (package.ReservedByStudentId != studentId)
                throw new UnauthorizedAccessException("Package is not reserved by you");

            // Atomically cancel the reservation
            package.ReservedByStudentId = null;
            package.ReservedByStudent = null;
            await _packageRepository.UpdateAsync(package);
        });
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
