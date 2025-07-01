using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;

namespace AvansMaaltijdreservering.WebApp.Controllers;

[Authorize]
public class PackageController : Controller
{
    private readonly IPackageService _packageService;
    private readonly ICanteenEmployeeRepository _canteenEmployeeRepository;
    private readonly Infrastructure.Identity.IAuthorizationService _authService;

    public PackageController(
        IPackageService packageService, 
        ICanteenEmployeeRepository canteenEmployeeRepository,
        Infrastructure.Identity.IAuthorizationService authService)
    {
        _packageService = packageService;
        _canteenEmployeeRepository = canteenEmployeeRepository;
        _authService = authService;
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
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> Index()
    {
        var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
        if (currentEmployeeId == null)
            return Forbid();
        
        var employee = await _canteenEmployeeRepository.GetByIdAsync(currentEmployeeId.Value);
        if (employee == null)
            return Forbid();

        var packages = await _packageService.GetPackagesByCanteenAsync(employee.Canteen.Location);
        return View(packages);
    }

    // US_02: All canteens overview
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> AllCanteens()
    {
        var packages = await _packageService.GetAvailablePackagesAsync();
        return View(packages);
    }

    // US_03: Create package
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public IActionResult Create()
    {
        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.CanteenLocations = Enum.GetValues<CanteenLocation>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        return View(new Package());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> Create(Package package)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
                if (currentEmployeeId == null)
                    return Forbid();
                
                await _packageService.CreatePackageAsync(package, currentEmployeeId.Value);
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
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> Edit(int id)
    {
        var package = await _packageService.GetPackageByIdAsync(id);
        if (package == null)
            return NotFound();

        var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
        if (currentEmployeeId == null)
            return Forbid();
        if (!await _packageService.CanEmployeeModifyPackageAsync(id, currentEmployeeId.Value))
            return Forbid();

        ViewBag.Cities = Enum.GetValues<City>();
        ViewBag.CanteenLocations = Enum.GetValues<CanteenLocation>();
        ViewBag.MealTypes = Enum.GetValues<MealType>();
        return View(package);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> Edit(Package package)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
                if (currentEmployeeId == null)
                    return Forbid();
                
                await _packageService.UpdatePackageAsync(package, currentEmployeeId.Value);
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
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> Delete(int id)
    {
        var package = await _packageService.GetPackageByIdAsync(id);
        if (package == null)
            return NotFound();

        var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
        if (currentEmployeeId == null)
            return Forbid();
        if (!await _packageService.CanEmployeeModifyPackageAsync(id, currentEmployeeId.Value))
            return Forbid();

        return View(package);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
            if (currentEmployeeId == null)
                return Forbid();
            
            await _packageService.DeletePackageAsync(id, currentEmployeeId.Value);
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