using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.WebApp.Controllers;

[Authorize]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IReservationService _reservationService;
    private readonly IPackageService _packageService;

    public StudentController(
        IStudentService studentService,
        IReservationService reservationService,
        IPackageService packageService)
    {
        _studentService = studentService;
        _reservationService = reservationService;
        _packageService = packageService;
    }

    // US_01: Student Overview - Main dashboard
    public async Task<IActionResult> Dashboard()
    {
        // TODO: Get current student ID from user claims
        int currentStudentId = 1; // Placeholder

        var student = await _studentService.GetStudentByIdAsync(currentStudentId);
        if (student == null)
            return Forbid();

        var availablePackages = await _packageService.GetAvailablePackagesAsync();
        var studentReservations = await _reservationService.GetStudentReservationsAsync(currentStudentId);

        var viewModel = new StudentDashboardViewModel
        {
            Student = student,
            AvailablePackages = availablePackages.Take(5), // Show recent 5
            MyReservations = studentReservations
        };

        return View(viewModel);
    }

    // US_01: Available Packages page with filtering (US_08)
    public async Task<IActionResult> AvailablePackages(City? city = null, MealType? mealType = null)
    {
        // TODO: Get current student ID from user claims  
        int currentStudentId = 1; // Placeholder
        
        var student = await _studentService.GetStudentByIdAsync(currentStudentId);
        if (student == null)
            return Forbid();

        var packages = await _packageService.GetAvailablePackagesAsync();

        // US_08: Filter by city (default to student's study city)
        if (city.HasValue)
            packages = packages.Where(p => p.City == city.Value);
        else if (student != null)
            packages = packages.Where(p => p.City == student.StudyCity);

        // US_08: Filter by meal type
        if (mealType.HasValue)
            packages = packages.Where(p => p.MealType == mealType.Value);

        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        ViewBag.SelectedCity = city ?? student?.StudyCity;
        ViewBag.SelectedMealType = mealType;
        ViewBag.StudentCity = student?.StudyCity;

        return View(packages);
    }

    // US_01: My Reservations page
    public async Task<IActionResult> MyReservations()
    {
        // TODO: Get current student ID from user claims
        int currentStudentId = 1; // Placeholder

        var reservations = await _reservationService.GetStudentReservationsAsync(currentStudentId);
        return View(reservations);
    }

    // US_05: Make reservation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeReservation(int packageId)
    {
        try
        {
            // TODO: Get current student ID from user claims
            int currentStudentId = 1; // Placeholder

            await _reservationService.MakeReservationAsync(packageId, currentStudentId);
            TempData["Success"] = "Reservation made successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("AvailablePackages");
    }

    // Cancel reservation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int packageId)
    {
        try
        {
            // TODO: Get current student ID from user claims
            int currentStudentId = 1; // Placeholder

            await _reservationService.CancelReservationAsync(packageId, currentStudentId);
            TempData["Success"] = "Reservation cancelled successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("MyReservations");
    }

    // Package details with reservation option
    public async Task<IActionResult> PackageDetails(int id)
    {
        var package = await _packageService.GetPackageByIdAsync(id);
        if (package == null)
            return NotFound();

        // TODO: Get current student ID from user claims
        int currentStudentId = 1; // Placeholder
        
        var student = await _studentService.GetStudentByIdAsync(currentStudentId);
        
        ViewBag.CanReserve = student != null && 
                            await _reservationService.IsStudentEligibleForPackageAsync(currentStudentId, id) &&
                            await _reservationService.IsPackageAvailableAsync(id);
        ViewBag.Student = student;

        return View(package);
    }

    // Student registration/profile
    public async Task<IActionResult> Profile()
    {
        // TODO: Get current student ID from user claims
        int currentStudentId = 1; // Placeholder

        var student = await _studentService.GetStudentByIdAsync(currentStudentId);
        if (student == null)
        {
            // New student registration
            return View("Register", new Student());
        }

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(Student student)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _studentService.UpdateStudentAsync(student);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View("Profile", student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Student student)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _studentService.RegisterStudentAsync(student);
                TempData["Success"] = "Registration successful!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(student);
    }
}

// View Models
public class StudentDashboardViewModel
{
    public Student Student { get; set; } = null!;
    public IEnumerable<Package> AvailablePackages { get; set; } = new List<Package>();
    public IEnumerable<Package> MyReservations { get; set; } = new List<Package>();
}