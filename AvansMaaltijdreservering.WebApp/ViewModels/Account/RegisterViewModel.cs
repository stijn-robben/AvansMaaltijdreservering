using System.ComponentModel.DataAnnotations;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.Domain.ValidationAttributes;

namespace AvansMaaltijdreservering.WebApp.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [Display(Name = "Full Name")]
    public string? Name { get; set; }

    [Display(Name = "I am registering as a student")]
    public bool IsStudent { get; set; } = true;

    // Student-specific fields
    [Display(Name = "Student Number")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Student number must be between 1 and 20 characters")]
    public string? StudentNumber { get; set; }

    [Display(Name = "Date of Birth")]
    [MinimumAge(16)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-20);

    [Display(Name = "Study City")]
    public City StudyCity { get; set; }

    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? PhoneNumber { get; set; }

    // Employee-specific fields
    [Display(Name = "Employee Number")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Employee number must be between 1 and 20 characters")]
    public string? EmployeeNumber { get; set; }

    [Display(Name = "Works at Canteen")]
    public int? CanteenId { get; set; }
}