using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.API.DTOs;
using AvansMaaltijdreservering.API.Extensions;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;
    private readonly Infrastructure.Identity.IUserAuthorizationService _authService;

    public PackagesController(
        IPackageService packageService,
        Infrastructure.Identity.IUserAuthorizationService authService)
    {
        _packageService = packageService;
        _authService = authService;
    }

    /// <summary>
    /// Get all available packages (RMM Level 2)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PackageResponseDto>>> GetAvailablePackages(
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

            var packageDtos = packages.Select(p => p.ToResponseDto());
            return Ok(packageDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get specific package by ID with product information (US_06 - RMM Level 2)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PackageDetailsDto>> GetPackage(int id)
    {
        try
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            
            if (package == null)
                return NotFound(new { message = $"Package with ID {id} not found" });

            var packageDto = new PackageDetailsDto
            {
                Package = package.ToResponseDto(),
                ProductDisclaimer = "⚠️ DISCLAIMER: The products shown are examples based on historical data. The actual contents may vary and are not guaranteed. Products are subject to availability.",
                ReservationInfo = new ReservationInfoDto
                {
                    IsAvailable = !package.IsReserved && package.IsValidPickupTime(),
                    RequiresAge18Plus = package.Is18Plus,
                    PickupWindow = $"{package.PickupTime:HH:mm} - {package.LatestPickupTime:HH:mm}",
                    Location = $"{package.City} - {package.CanteenLocation}",
                    ReservationDeadline = package.PickupTime.AddHours(-2) // 2 hours before pickup
                }
            };

            return Ok(packageDto);
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
    public async Task<ActionResult<Package>> CreatePackage([FromBody] CreatePackageDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
            if (currentEmployeeId == null)
                return Forbid();

            // Convert DTO to Package entity
            var package = new Package
            {
                Name = dto.Name,
                City = dto.City,
                CanteenLocation = dto.CanteenLocation,
                PickupTime = dto.PickupTime,
                LatestPickupTime = dto.LatestPickupTime,
                Price = dto.Price,
                MealType = dto.MealType
            };

            var createdPackage = await _packageService.CreatePackageAsync(package, currentEmployeeId.Value, dto.ProductIds);
            
            return CreatedAtAction(
                nameof(GetPackage), 
                new { id = createdPackage.Id }, 
                createdPackage);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
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
            return BadRequest(new { message = ex.Message });
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
            return BadRequest(new { message = ex.Message });
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
    /// Get packages for current employee's canteen (CanteenEmployee only)
    /// </summary>
    [HttpGet("my-canteen")]
    [Authorize(Roles = IdentityRoles.CanteenEmployee)]
    public async Task<ActionResult<IEnumerable<Package>>> GetMyCanteenPackages()
    {
        try
        {
            var currentEmployeeId = await _authService.GetCurrentCanteenEmployeeIdAsync(User);
            if (currentEmployeeId == null)
                return Forbid();

            var packages = await _packageService.GetPackagesForEmployeeCanteenAsync(currentEmployeeId.Value);
            return Ok(packages);
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