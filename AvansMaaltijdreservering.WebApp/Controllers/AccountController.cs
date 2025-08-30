using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.WebApp.ViewModels.Account;

namespace AvansMaaltijdreservering.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IStudentService _studentService;
    private readonly ICanteenEmployeeRepository _employeeRepository;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IStudentService studentService,
        ICanteenEmployeeRepository employeeRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _studentService = studentService;
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    if (roles.Contains(IdentityRoles.Student))
                    {
                        return RedirectToAction("Dashboard", "Students");
                    }
                    else if (roles.Contains(IdentityRoles.CanteenEmployee))
                    {
                        return RedirectToAction("Dashboard", "Employees");
                    }
                }
                
                return LocalRedirect(returnUrl ?? "/");
            }
            
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser 
            { 
                UserName = model.Email, 
                Email = model.Email 
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Add role based on registration type
                if (model.IsStudent)
                {
                    await _userManager.AddToRoleAsync(user, IdentityRoles.Student);
                    
                    // Create student record
                    var student = new Student
                    {
                        Name = model.Name!,
                        Email = model.Email,
                        StudentNumber = model.StudentNumber!,
                        DateOfBirth = model.DateOfBirth,
                        StudyCity = model.StudyCity,
                        PhoneNumber = model.PhoneNumber!
                    };

                    try
                    {
                        await _studentService.RegisterStudentAsync(student);
                        TempData["SuccessMessage"] = "Student registration successful! You can now log in.";
                    }
                    catch (ArgumentException ex)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(model);
                    }
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, IdentityRoles.CanteenEmployee);
                    
                    // Create employee record
                    var employee = new CanteenEmployee
                    {
                        Name = model.Name!,
                        EmployeeNumber = model.EmployeeNumber!,
                        CanteenId = (int)model.CanteenId!
                    };

                    try
                    {
                        await _employeeRepository.AddAsync(employee);
                        TempData["SuccessMessage"] = "Employee registration successful! You can now log in.";
                    }
                    catch
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, "Employee registration failed. Please try again.");
                        return View(model);
                    }
                }

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}