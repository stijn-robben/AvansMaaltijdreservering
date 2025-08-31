using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.WebApp.Models;
using AvansMaaltijdreservering.Infrastructure.Identity;

namespace AvansMaaltijdreservering.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IPackageService _packageService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(IPackageService packageService, UserManager<ApplicationUser> userManager)
    {
        _packageService = packageService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Check if user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                // Redirect to appropriate dashboard based on role
                if (roles.Contains(IdentityRoles.Student))
                {
                    return RedirectToAction("Dashboard", "Students");
                }
                else if (roles.Contains(IdentityRoles.CanteenEmployee))
                {
                    return RedirectToAction("Dashboard", "Employees");
                }
            }
        }

        // If not authenticated or no valid role, show public index
        try
        {
            var packages = await _packageService.GetAvailablePackagesAsync();
            return View(packages);
        }
        catch
        {
            return View(new List<Package>());
        }
    }


    public async Task<IActionResult> TestData()
    {
        try
        {
            var packages = await _packageService.GetAvailablePackagesAsync();
            var packageCount = packages.Count();
            
            ViewBag.Message = $"Integration test successful! Found {packageCount} available packages.";
            ViewBag.Success = true;
            
            return View("TestResult");
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Integration test failed: {ex.Message}";
            ViewBag.Success = false;
            ViewBag.StackTrace = ex.ToString();
            
            return View("TestResult");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}