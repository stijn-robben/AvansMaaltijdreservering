using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.API.DTOs;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly Infrastructure.Identity.IAuthorizationService _authService;

    public StudentsController(
        IStudentService studentService,
        Infrastructure.Identity.IAuthorizationService authService)
    {
        _studentService = studentService;
        _authService = authService;
    }

    /// <summary>
    /// Get current student profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Roles = IdentityRoles.Student)]
    public async Task<ActionResult<Student>> GetProfile()
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            var student = await _studentService.GetStudentByIdAsync(currentStudentId.Value);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            return Ok(student);
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