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

            // US_07: First-come-first-served with user-friendly error message  
            if (package.IsReserved)
                throw new ReservationException("😔 Sorry! This package has already been reserved by another student. Please check our other available packages.", packageId, studentId);

            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new ReservationException("Student not found", packageId, studentId);

            // Provide specific error messages for each business rule violation
            if (student.IsBlocked())
                throw new StudentBlockedException(studentId, student.NoShowCount);

            // US_04: Age restriction with clear message  
            if (package.ContainsAlcohol() && !student.IsAdultOnDate(package.PickupTime.Date))
            {
                var ageOnPickup = student.GetAgeOnDate(package.PickupTime.Date);
                throw new ReservationException($"🚫 This package contains alcohol and requires you to be 18+ on pickup date. You will be {ageOnPickup} years old on {package.PickupTime.Date:dd-MM-yyyy}.", packageId, studentId);
            }

            // US_05: One package per day with helpful message
            if (student.HasReservationOnDate(package.PickupTime.Date))
            {
                var existingReservation = student.Reservations?.FirstOrDefault(r => r.PickupTime.Date == package.PickupTime.Date);
                var conflictDate = existingReservation?.PickupTime.Date ?? package.PickupTime.Date;
                throw new ReservationException($"📅 You already have a package reservation for {conflictDate:dd-MM-yyyy}. You can only reserve one package per day.", packageId, studentId);
            }

            // General eligibility check (shouldn't happen if above checks pass)
            var canReserve = await _studentService.CanReservePackageAsync(studentId, package);
            if (!canReserve)
                throw new ReservationException("You are not eligible to reserve this package", packageId, studentId);

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
        return await _packageRepository.GetPackagesByStudentIdAsync(studentId);
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
            
            try
            {
                // Cancel all future reservations for this blocked student
                var futureReservations = await _packageRepository.GetPackagesByStudentIdAsync(student.Id);
                
                if (futureReservations != null)
                {
                    var futurePendingReservations = futureReservations
                        .Where(p => p != null && p.PickupTime > DateTime.Now && p.ReservedByStudentId == student.Id)
                        .ToList();
                    
                    foreach (var reservation in futurePendingReservations)
                    {
                        try
                        {
                            reservation.ReservedByStudentId = null;
                            reservation.ReservedByStudent = null;
                            await _packageRepository.UpdateAsync(reservation);
                            
                            _logger.LogInfo($"Cancelled future reservation for package {reservation.Id} (pickup: {reservation.PickupTime}) due to student blocking");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Failed to cancel reservation for package {reservation.Id}: {ex.Message}");
                        }
                    }
                    
                    if (futurePendingReservations.Any())
                    {
                        _logger.LogWarning($"Cancelled {futurePendingReservations.Count} future reservations for blocked student {student.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error while cancelling future reservations for blocked student {student.Id}: {ex.Message}");
                // Don't rethrow - the main no-show registration should still succeed
            }
        }
    }

    public async Task<bool> IsStudentEligibleForPackageAsync(int studentId, int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
        {
            _logger.LogInfo($"Package {packageId} not found for eligibility check");
            return false;
        }
        
        var isEligible = await _studentService.IsStudentEligibleForPackageAsync(studentId, package);
        _logger.LogInfo($"Eligibility check for student {studentId} and package {packageId}: {isEligible}");
        return isEligible;
    }

    public async Task<bool> IsPackageAvailableAsync(int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        return package != null && !package.IsReserved && package.PickupTime > DateTime.Now;
    }

    public async Task<bool> CanStudentReservePackageAsync(int studentId, int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
        {
            _logger.LogInfo($"Package {packageId} not found for reservation check");
            return false;
        }
        
        var canReserve = await _studentService.CanReservePackageAsync(studentId, package);
        _logger.LogInfo($"Can reserve check for student {studentId} and package {packageId}: {canReserve}");
        return canReserve;
    }
}