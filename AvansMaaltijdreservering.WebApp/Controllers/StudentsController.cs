using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Exceptions;
using AvansMaaltijdreservering.WebApp.ViewModels.Students;

namespace AvansMaaltijdreservering.WebApp.Controllers;

[Authorize(Roles = "Student")]
public class StudentsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStudentService _studentService;
    private readonly IPackageService _packageService;
    private readonly IReservationService _reservationService;

    public StudentsController(
        UserManager<ApplicationUser> userManager,
        IStudentService studentService,
        IPackageService packageService,
        IReservationService reservationService)
    {
        _userManager = userManager;
        _studentService = studentService;
        _packageService = packageService;
        _reservationService = reservationService;
    }

    // US_01: Student Dashboard - Available Packages + Filtering (US_08)
    public async Task<IActionResult> Dashboard(City? filterCity = null, MealType? filterMealType = null)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var student = await GetCurrentStudentAsync(currentUser);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            // US_10: Check if student is blocked
            if (student.IsBlocked())
            {
                TempData["ErrorMessage"] = $"⚠️ Your account is temporarily blocked due to {student.NoShowCount} no-shows. Please contact student services for assistance.";
            }

            // Get available packages
            var availablePackages = await _packageService.GetAvailablePackagesAsync();

            // US_08: Apply filters
            if (filterCity.HasValue)
            {
                availablePackages = availablePackages.Where(p => p.City == filterCity.Value);
            }
            else
            {
                // Default to student's study city
                availablePackages = availablePackages.Where(p => p.City == student.StudyCity);
            }

            if (filterMealType.HasValue)
            {
                availablePackages = availablePackages.Where(p => p.MealType == filterMealType.Value);
            }

            // US_04: Check eligibility for each package
            var packagesWithEligibility = new List<PackageEligibilityViewModel>();
            
            foreach (var package in availablePackages.OrderBy(p => p.PickupTime))
            {
                var isEligible = await _reservationService.IsStudentEligibleForPackageAsync(student.Id, package.Id);
                var canReserve = await _reservationService.CanStudentReservePackageAsync(student.Id, package.Id);
                
                string? ineligibilityReason = null;
                if (!canReserve)
                {
                    if (package.ContainsAlcohol() && !student.IsAdultOnDate(package.PickupTime.Date))
                    {
                        var ageOnPickup = student.GetAgeOnDate(package.PickupTime.Date);
                        ineligibilityReason = $"🔞 Requires 18+ (you'll be {ageOnPickup} on pickup date)";
                    }
                    else if (student.HasReservationOnDate(package.PickupTime.Date))
                    {
                        ineligibilityReason = "📅 You already have a reservation this day";
                    }
                    else if (student.IsBlocked())
                    {
                        ineligibilityReason = "⚠️ Account blocked due to no-shows";
                    }
                }

                packagesWithEligibility.Add(new PackageEligibilityViewModel
                {
                    Package = package,
                    IsEligible = isEligible,
                    CanReserve = canReserve,
                    IneligibilityReason = ineligibilityReason
                });
            }

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                AvailablePackages = packagesWithEligibility,
                FilterCity = filterCity ?? student.StudyCity,
                FilterMealType = filterMealType
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred loading the dashboard: {ex.Message}";
            return View(new StudentDashboardViewModel());
        }
    }

    // US_01: Student Reservations Overview
    public async Task<IActionResult> Reservations()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var student = await GetCurrentStudentAsync(currentUser);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            var reservations = await _reservationService.GetStudentReservationsAsync(student.Id);

            var viewModel = new StudentReservationsViewModel
            {
                Student = student,
                Reservations = reservations.OrderBy(r => r.PickupTime).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred loading your reservations: {ex.Message}";
            return View(new StudentReservationsViewModel());
        }
    }

    // US_06: Package Details with Product Information
    public async Task<IActionResult> PackageDetails(int id)
    {
        try
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
                return NotFound("Package not found");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var student = await GetCurrentStudentAsync(currentUser);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            var canReserve = await _reservationService.CanStudentReservePackageAsync(student.Id, id);
            var isAvailable = await _reservationService.IsPackageAvailableAsync(id);

            var viewModel = new PackageDetailsViewModel
            {
                Package = package,
                Student = student,
                CanReserve = canReserve && isAvailable,
                IsAvailable = isAvailable
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading package details: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // US_05: Reserve Package
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReservePackage(int packageId)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var student = await GetCurrentStudentAsync(currentUser);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            // US_05 & US_07: Attempt reservation with business rule validation
            var reservedPackage = await _reservationService.ReservePackageAsync(packageId, student.Id);
            
            TempData["SuccessMessage"] = $"🎉 Successfully reserved '{reservedPackage.Name}'! Don't forget to pick it up on time.";
            return RedirectToAction("Reservations");
        }
        catch (ReservationException ex)
        {
            // US_07: User-friendly error messages are already in your service
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("PackageDetails", new { id = packageId });
        }
        catch (StudentBlockedException ex)
        {
            TempData["ErrorMessage"] = $"⚠️ Your account is blocked due to {ex.NoShowCount} no-shows. Please contact student services.";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
            return RedirectToAction("PackageDetails", new { id = packageId });
        }
    }

    // Cancel Reservation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int packageId)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var student = await GetCurrentStudentAsync(currentUser);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            await _reservationService.CancelReservationAsync(packageId, student.Id);
            
            TempData["SuccessMessage"] = "✅ Reservation cancelled successfully. The package is now available for other students.";
            return RedirectToAction("Reservations");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error cancelling reservation: {ex.Message}";
            return RedirectToAction("Reservations");
        }
    }

    // Helper Methods
    private async Task<Student?> GetCurrentStudentAsync(ApplicationUser currentUser)
    {
        // Try to get student using the linked StudentId from ApplicationUser
        if (currentUser.StudentId.HasValue)
        {
            var studentById = await _studentService.GetStudentByIdAsync(currentUser.StudentId.Value);
            if (studentById != null) return studentById;
        }
        
        // Fallback: try to find by email
        if (!string.IsNullOrEmpty(currentUser.Email))
        {
            var studentByEmail = await _studentService.GetStudentByEmailAsync(currentUser.Email);
            if (studentByEmail != null) return studentByEmail;
        }
        
        // Last resort: try to find by StudentNumber if stored in ApplicationUser
        if (!string.IsNullOrEmpty(currentUser.StudentNumber))
        {
            var studentByNumber = await _studentService.GetStudentByStudentNumberAsync(currentUser.StudentNumber);
            if (studentByNumber != null) return studentByNumber;
        }
        
        return null;
    }
}