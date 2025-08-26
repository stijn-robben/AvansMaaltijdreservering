using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Exceptions;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class ReservationService : IReservationService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICanteenEmployeeRepository _employeeRepository;
    private readonly IStudentService _studentService;
    private readonly IPackageLockService _lockService;
    private readonly ILoggerService _logger;

    public ReservationService(
        IPackageRepository packageRepository,
        IStudentRepository studentRepository,
        ICanteenEmployeeRepository employeeRepository,
        IStudentService studentService,
        IPackageLockService lockService,
        ILoggerService logger)
    {
        _packageRepository = packageRepository;
        _studentRepository = studentRepository;
        _employeeRepository = employeeRepository;
        _studentService = studentService;
        _lockService = lockService;
        _logger = logger;
    }

    public async Task<Package> ReservePackageAsync(int packageId, int studentId)
    {
        if (!await _lockService.TryLockPackageAsync(packageId))
            throw new ReservationException("Package is currently being reserved by another user", packageId, studentId);

        try
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
                throw new ReservationException("Package not found", packageId, studentId);

            if (package.IsReserved)
                throw new ReservationException("Package is already reserved", packageId, studentId);

            if (!await _studentService.CanReservePackageAsync(studentId, package))
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student?.IsBlocked() == true)
                    throw new StudentBlockedException(studentId, student.NoShowCount);
                    
                throw new ReservationException("Student cannot reserve this package", packageId, studentId);
            }

            package.ReservedByStudentId = studentId;
            var updatedPackage = await _packageRepository.UpdateAsync(package);

            _logger.LogInfo($"Package {packageId} reserved by student {studentId}");
            return updatedPackage;
        }
        finally
        {
            await _lockService.ReleasePackageLockAsync(packageId);
        }
    }

    public async Task MakeReservationAsync(int packageId, int studentId)
    {
        await ReservePackageAsync(packageId, studentId);
    }

    public async Task CancelReservationAsync(int packageId, int studentId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            throw new ArgumentException("Package not found");

        if (package.ReservedByStudentId != studentId)
            throw new ArgumentException("Cannot cancel another student's reservation");

        package.ReservedByStudentId = null;
        package.ReservedByStudent = null;
        
        await _packageRepository.UpdateAsync(package);
        _logger.LogInfo($"Reservation cancelled for package {packageId} by student {studentId}");
    }

    public async Task<IEnumerable<Package>> GetStudentReservationsAsync(int studentId)
    {
        var packages = await _packageRepository.GetAllAsync();
        return packages.Where(p => p.ReservedByStudentId == studentId);
    }

    public async Task RegisterNoShowAsync(int packageId, int employeeId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            throw new ArgumentException("Package not found");

        if (package.ReservedByStudentId == null)
            throw new ArgumentException("Package has no reservation");

        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            throw new UnauthorizedAccessException("Employee not found");

        var student = await _studentRepository.GetByIdAsync(package.ReservedByStudentId.Value);
        if (student == null)
            throw new ArgumentException("Reserved student not found");

        student.NoShowCount++;
        await _studentRepository.UpdateAsync(student);

        package.ReservedByStudentId = null;
        package.ReservedByStudent = null;
        await _packageRepository.UpdateAsync(package);

        _logger.LogWarning($"No-show registered for student {student.Id} on package {packageId}. Total no-shows: {student.NoShowCount}");
        
        if (student.IsBlocked())
        {
            _logger.LogWarning($"Student {student.Id} is now blocked due to excessive no-shows");
        }
    }

    public async Task<bool> IsStudentEligibleForPackageAsync(int studentId, int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            return false;
            
        return await _studentService.CanReservePackageAsync(studentId, package);
    }

    public async Task<bool> IsPackageAvailableAsync(int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        return package != null && !package.IsReserved && package.PickupTime > DateTime.Now;
    }
}