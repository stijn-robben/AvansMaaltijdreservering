using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Interfaces;

namespace AvansMaaltijdreservering.WebApp.Controllers;

[Authorize]
public class PackageController : Controller
{
    private readonly IPackageService _packageService;
    private readonly ICanteenEmployeeRepository _canteenEmployeeRepository;

    public PackageController(
        IPackageService packageService, 
        ICanteenEmployeeRepository canteenEmployeeRepository)
    {
        _packageService = packageService;
        _canteenEmployeeRepository = canteenEmployeeRepository;
    }

    // US_01: Student view - Available packages
    [AllowAnonymous]
    public async Task<IActionResult> Available(City? city = null, MealType? mealType = null)
    {
        var packages = await _packageService.GetAvailablePackagesAsync();
        
        if (city.HasValue)
            packages = packages.Where(p => p.City == city.Value);
            
        if (mealType.HasValue)
            packages = packages.Where(p => p.MealType == mealType.Value);

        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        ViewBag.SelectedCity = city;
        ViewBag.SelectedMealType = mealType;

        return View(packages);
    }

    // US_02: Canteen employee view - Own canteen packages
    public async Task<IActionResult> Index()
    {
        // TODO: Get current employee ID from user claims
        int currentEmployeeId = 1; // Placeholder
        
        var employee = await _canteenEmployeeRepository.GetByIdAsync(currentEmployeeId);
        if (employee == null)
            return Forbid();

        var packages = await _packageService.GetPackagesByCanteenAsync(employee.Canteen.Location);
        return View(packages);
    }

    // US_02: All canteens overview
    public async Task<IActionResult> AllCanteens()
    {
        var packages = await _packageService.GetAvailablePackagesAsync();
        return View(packages);
    }

    // US_03: Create package
    public IActionResult Create()
    {
        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.CanteenLocations = Enum.GetValues<CanteenLocation>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        return View(new Package());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Package package)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // TODO: Get current employee ID from user claims
                int currentEmployeeId = 1; // Placeholder
                
                await _packageService.CreatePackageAsync(package, currentEmployeeId);
                TempData["Success"] = "Package created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.CanteenLocations = Enum.GetValues<CanteenLocation>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        return View(package);
    }

    // US_03: Edit package
    public async Task<IActionResult> Edit(int id)
    {
        var package = await _packageService.GetPackageByIdAsync(id);
        if (package == null)
            return NotFound();

        // TODO: Check if current employee can modify this package
        int currentEmployeeId = 1; // Placeholder
        if (!await _packageService.CanEmployeeModifyPackageAsync(id, currentEmployeeId))
            return Forbid();

        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.CanteenLocations = Enum.GetValues<CanteenLocation>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        return View(package);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Package package)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // TODO: Get current employee ID from user claims
                int currentEmployeeId = 1; // Placeholder
                
                await _packageService.UpdatePackageAsync(package, currentEmployeeId);
                TempData["Success"] = "Package updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.CanteenLocations = Enum.GetValues<CanteenLocation>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        return View(package);
    }

    // US_03: Delete package
    public async Task<IActionResult> Delete(int id)
    {
        var package = await _packageService.GetPackageByIdAsync(id);
        if (package == null)
            return NotFound();

        // TODO: Check if current employee can modify this package
        int currentEmployeeId = 1; // Placeholder
        if (!await _packageService.CanEmployeeModifyPackageAsync(id, currentEmployeeId))
            return Forbid();

        return View(package);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            // TODO: Get current employee ID from user claims
            int currentEmployeeId = 1; // Placeholder
            
            await _packageService.DeletePackageAsync(id, currentEmployeeId);
            TempData["Success"] = "Package deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // Package details view
    public async Task<IActionResult> Details(int id)
    {
        var package = await _packageService.GetPackageByIdAsync(id);
        if (package == null)
            return NotFound();

        return View(package);
    }
}