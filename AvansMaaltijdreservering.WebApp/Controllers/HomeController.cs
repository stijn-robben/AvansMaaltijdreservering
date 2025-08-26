using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.WebApp.Models;

namespace AvansMaaltijdreservering.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IPackageService _packageService;

    public HomeController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    public async Task<IActionResult> Index()
    {
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

    public IActionResult Privacy()
    {
        return View();
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