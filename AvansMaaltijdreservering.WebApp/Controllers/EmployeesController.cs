using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.WebApp.ViewModels.Employees;

namespace AvansMaaltijdreservering.WebApp.Controllers;

[Authorize(Roles = "CanteenEmployee")]
public class EmployeesController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPackageService _packageService;
    private readonly IReservationService _reservationService;
    private readonly ICanteenEmployeeRepository _employeeRepository;
    private readonly ICanteenRepository _canteenRepository;
    private readonly IProductRepository _productRepository;

    public EmployeesController(
        UserManager<ApplicationUser> userManager,
        IPackageService packageService,
        IReservationService reservationService,
        ICanteenEmployeeRepository employeeRepository,
        ICanteenRepository canteenRepository,
        IProductRepository productRepository)
    {
        _userManager = userManager;
        _packageService = packageService;
        _reservationService = reservationService;
        _employeeRepository = employeeRepository;
        _canteenRepository = canteenRepository;
        _productRepository = productRepository;
    }

    // US_02: Employee Dashboard - Own canteen packages + All canteens overview
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            // Get employee's own canteen packages
            var ownCanteenPackages = await _packageService.GetPackagesForEmployeeCanteenAsync(employee.Id);

            // Get all packages from all canteens (for overview)
            var allPackages = (await _packageService.GetAvailablePackagesAsync())
                .Concat(ownCanteenPackages.Where(p => p.IsReserved))
                .DistinctBy(p => p.Id)
                .OrderBy(p => p.PickupTime);

            var canteen = await _canteenRepository.GetByIdAsync(employee.CanteenId);

            var viewModel = new EmployeeDashboardViewModel
            {
                Employee = employee,
                EmployeeCanteen = canteen,
                OwnCanteenPackages = ownCanteenPackages.OrderBy(p => p.PickupTime).ToList(),
                AllPackages = allPackages.ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
            return View(new EmployeeDashboardViewModel());
        }
    }

    // US_03: Create Package
    [HttpGet]
    public async Task<IActionResult> CreatePackage()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            var canteen = await _canteenRepository.GetByIdAsync(employee.CanteenId);
            var allProducts = await _productRepository.GetAllAsync();

            var viewModel = new CreatePackageViewModel
            {
                Employee = employee,
                EmployeeCanteen = canteen,
                AvailableProducts = allProducts.ToList(),
                PickupTime = DateTime.Today.AddDays(1).AddHours(12), // Default tomorrow at noon
                LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14) // 2 hours later
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading create form: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // US_03: Create Package (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePackage(CreatePackageViewModel model)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var package = new Package
                {
                    Name = model.Name,
                    City = model.City,
                    CanteenLocation = model.CanteenLocation,
                    PickupTime = model.PickupTime,
                    LatestPickupTime = model.LatestPickupTime,
                    Price = model.Price,
                    MealType = model.MealType
                };

                // US_03: Business rule - max 2 days ahead
                if (package.PickupTime > DateTime.Today.AddDays(2))
                {
                    ModelState.AddModelError("PickupTime", "Packages can only be planned up to 2 days ahead");
                    await RepopulateCreateViewModel(model, employee);
                    return View(model);
                }

                var createdPackage = await _packageService.CreatePackageAsync(package, employee.Id, model.SelectedProductIds);
                
                TempData["SuccessMessage"] = $"✅ Package '{createdPackage.Name}' created successfully!";
                return RedirectToAction("Dashboard");
            }

            await RepopulateCreateViewModel(model, employee);
            return View(model);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = currentUser != null ? await GetCurrentEmployeeAsync(currentUser) : null;
            if (employee != null)
                await RepopulateCreateViewModel(model, employee);
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error creating package: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // US_03: Edit Package
    [HttpGet]
    public async Task<IActionResult> EditPackage(int id)
    {
        try
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
                return NotFound();

            // US_03: Cannot modify packages with reservations
            if (package.IsReserved)
            {
                TempData["ErrorMessage"] = "Cannot modify packages that already have reservations.";
                return RedirectToAction("Dashboard");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            var canteen = await _canteenRepository.GetByIdAsync(employee.CanteenId);
            var allProducts = await _productRepository.GetAllAsync();

            var viewModel = new EditPackageViewModel
            {
                Id = package.Id,
                Name = package.Name,
                City = package.City,
                CanteenLocation = package.CanteenLocation,
                PickupTime = package.PickupTime,
                LatestPickupTime = package.LatestPickupTime,
                Price = package.Price,
                MealType = package.MealType,
                SelectedProductIds = package.Products.Select(p => p.Id).ToList(),
                Employee = employee,
                EmployeeCanteen = canteen,
                AvailableProducts = allProducts.ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading package: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // US_03: Edit Package (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPackage(EditPackageViewModel model)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var package = new Package
                {
                    Id = model.Id,
                    Name = model.Name,
                    City = model.City,
                    CanteenLocation = model.CanteenLocation,
                    PickupTime = model.PickupTime,
                    LatestPickupTime = model.LatestPickupTime,
                    Price = model.Price,
                    MealType = model.MealType
                };

                var updatedPackage = await _packageService.UpdatePackageAsync(package, employee.Id);
                
                TempData["SuccessMessage"] = $"✅ Package '{updatedPackage.Name}' updated successfully!";
                return RedirectToAction("Dashboard");
            }

            await RepopulateEditViewModel(model, employee);
            return View(model);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = currentUser != null ? await GetCurrentEmployeeAsync(currentUser) : null;
            if (employee != null)
                await RepopulateEditViewModel(model, employee);
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating package: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // US_03: Delete Package
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePackage(int id)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            await _packageService.DeletePackageAsync(id, employee.Id);
            
            TempData["SuccessMessage"] = "✅ Package deleted successfully!";
            return RedirectToAction("Dashboard");
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting package: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // US_10: Register No-Show
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterNoShow(int packageId)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var employee = await GetCurrentEmployeeAsync(currentUser);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found. Please contact IT support.";
                return RedirectToAction("Login", "Account");
            }

            await _reservationService.RegisterNoShowAsync(packageId, employee.Id);
            
            TempData["SuccessMessage"] = "⚠️ No-show registered. Student's no-show count has been updated and package is now available again.";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error registering no-show: {ex.Message}";
            return RedirectToAction("Dashboard");
        }
    }

    // Helper Methods
    private async Task<CanteenEmployee?> GetCurrentEmployeeAsync(ApplicationUser currentUser)
    {
        // Try to get employee using the linked CanteenEmployeeId from ApplicationUser
        if (currentUser.CanteenEmployeeId.HasValue)
        {
            var employee = await _employeeRepository.GetByIdAsync(currentUser.CanteenEmployeeId.Value);
            if (employee != null) return employee;
        }
        
        // Fallback: try to find by EmployeeNumber stored in ApplicationUser
        if (!string.IsNullOrEmpty(currentUser.EmployeeNumber))
        {
            var employee = await _employeeRepository.GetByEmployeeNumberAsync(currentUser.EmployeeNumber);
            if (employee != null) return employee;
        }
        
        // Last resort fallback: try to find by email domain match
        var allEmployees = await _employeeRepository.GetAllAsync();
        return allEmployees.FirstOrDefault(e => currentUser.Email!.Contains(e.EmployeeNumber));
    }

    private async Task RepopulateCreateViewModel(CreatePackageViewModel model, CanteenEmployee employee)
    {
        var canteen = await _canteenRepository.GetByIdAsync(employee.CanteenId);
        var allProducts = await _productRepository.GetAllAsync();
        
        model.Employee = employee;
        model.EmployeeCanteen = canteen;
        model.AvailableProducts = allProducts.ToList();
    }

    private async Task RepopulateEditViewModel(EditPackageViewModel model, CanteenEmployee employee)
    {
        var canteen = await _canteenRepository.GetByIdAsync(employee.CanteenId);
        var allProducts = await _productRepository.GetAllAsync();
        
        model.Employee = employee;
        model.EmployeeCanteen = canteen;
        model.AvailableProducts = allProducts.ToList();
    }
}