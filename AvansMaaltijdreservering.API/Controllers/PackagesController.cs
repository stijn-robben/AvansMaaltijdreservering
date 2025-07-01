using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;
    private readonly Infrastructure.Identity.IAuthorizationService _authService;

    public PackagesController(
        IPackageService packageService,
        Infrastructure.Identity.IAuthorizationService authService)
    {
        _packageService = packageService;
        _authService = authService;
    }

    /// <summary>
    /// Get all available packages (RMM Level 2)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Package>>> GetAvailablePackages(
        [FromQuery] City? city = null,
        [FromQuery] MealType? mealType = null)
    {
        try
        {
            var packages = await _packageService.GetAvailablePackagesAsync();

            if (city.HasValue)
                packages = packages.Where(p => p.City == city.Value);

            if (mealType.HasValue)
                packages = packages.Where(p => p.MealType == mealType.Value);

            return Ok(packages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get specific package by ID (RMM Level 2)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Package>> GetPackage(int id)
    {
        try
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            
            if (package == null)
                return NotFound(new { message = $"Package with ID {id} not found" });

            return Ok(package);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new package (CanteenEmployee only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<ActionResult<Package>> CreatePackage([FromBody] Package package)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
            if (currentEmployeeId == null)
                return Forbid();

            await _packageService.CreatePackageAsync(package, currentEmployeeId.Value);
            
            return CreatedAtAction(
                nameof(GetPackage), 
                new { id = package.Id }, 
                package);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
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
    /// Update existing package (CanteenEmployee only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> UpdatePackage(int id, [FromBody] Package package)
    {
        try
        {
            if (id != package.Id)
                return BadRequest(new { message = "Package ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
            if (currentEmployeeId == null)
                return Forbid();

            await _packageService.UpdatePackageAsync(package, currentEmployeeId.Value);
            
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
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
    /// Delete package (CanteenEmployee only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<IActionResult> DeletePackage(int id)
    {
        try
        {
            var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
            if (currentEmployeeId == null)
                return Forbid();

            await _packageService.DeletePackageAsync(id, currentEmployeeId.Value);
            
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
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
    /// Get packages by canteen location (CanteenEmployee only)
    /// </summary>
    [HttpGet("canteen/{location}")]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<ActionResult<IEnumerable<Package>>> GetPackagesByCanteen(CanteenLocation location)
    {
        try
        {
            var packages = await _packageService.GetPackagesByCanteenAsync(location);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}