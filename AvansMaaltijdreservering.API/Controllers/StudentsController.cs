using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.API.DTOs;
using AvansMaaltijdreservering.API.Extensions;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IPackageService _packageService;
    private readonly IReservationService _reservationService;
    private readonly Infrastructure.Identity.IUserAuthorizationService _authService;

    public StudentsController(
        IStudentService studentService,
        IPackageService packageService,
        IReservationService reservationService,
        Infrastructure.Identity.IUserAuthorizationService authService)
    {
        _studentService = studentService;
        _packageService = packageService;
        _reservationService = reservationService;
        _authService = authService;
    }

    /// <summary>
    /// Get student dashboard with available packages and reservations (US_01)
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = IdentityRoles.Student)]
    public async Task<ActionResult<StudentDashboardDto>> GetDashboard()
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            var student = await _studentService.GetStudentByIdAsync(currentStudentId.Value);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            // Get all available packages
            var allPackages = await _packageService.GetAvailablePackagesAsync();
            
            // Get student's reservations
            var reservations = await _reservationService.GetStudentReservationsAsync(currentStudentId.Value);
            
            // Filter packages for student's city (US_08 default filtering)
            var packagesInMyCity = allPackages.Where(p => p.City == student.StudyCity);
            
            // Count packages student can actually reserve (age eligible + available)
            var eligiblePackages = 0;
            foreach (var package in allPackages.Where(p => !p.IsReserved))
            {
                var isEligible = await _reservationService.IsStudentEligibleForPackageAsync(currentStudentId.Value, package.Id);
                if (isEligible) eligiblePackages++;
            }

            var dashboard = new StudentDashboardDto
            {
                Student = new StudentProfileDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    Age = student.GetAge(),
                    IsAdult = student.IsAdult(),
                    StudyCity = student.StudyCity,
                    Email = student.Email,
                    NoShowCount = student.NoShowCount,
                    IsBlocked = student.IsBlocked()
                },
                AvailablePackages = packagesInMyCity.Where(p => !p.IsReserved).Select(p => p.ToResponseDto()),
                MyReservations = reservations.Select(r => r.ToResponseDto()),
                Stats = new DashboardStats
                {
                    TotalAvailablePackages = allPackages.Count(p => !p.IsReserved),
                    AvailablePackagesInMyCity = packagesInMyCity.Count(p => !p.IsReserved),
                    MyActiveReservations = reservations.Count(),
                    PackagesICanReserve = eligiblePackages
                }
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get current student profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Roles = IdentityRoles.Student)]
    public async Task<ActionResult<StudentResponseDto>> GetProfile()
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            var student = await _studentService.GetStudentByIdAsync(currentStudentId.Value);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            return Ok(student.ToResponseDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Register a new student
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<Student>> RegisterStudent([FromBody] StudentRegistrationDto studentDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = new Student
            {
                Name = studentDto.Name,
                DateOfBirth = studentDto.DateOfBirth,
                StudentNumber = studentDto.StudentNumber,
                Email = studentDto.Email,
                StudyCity = studentDto.StudyCity,
                PhoneNumber = studentDto.PhoneNumber
            };

            await _studentService.RegisterStudentAsync(student);
            
            return CreatedAtAction(
                nameof(GetProfile), 
                student);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Update student profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Roles = IdentityRoles.Student)]
    public async Task<IActionResult> UpdateProfile([FromBody] Student student)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            if (student.Id != currentStudentId.Value)
                return BadRequest(new { message = "Cannot update another student's profile" });

            await _studentService.UpdateStudentAsync(student);
            
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get student by ID (Admin/Employee access)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { message = $"Student with ID {id} not found" });

            return Ok(student);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}