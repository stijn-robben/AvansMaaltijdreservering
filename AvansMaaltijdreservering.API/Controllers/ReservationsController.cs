using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = IdentityRoles.Student)]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly Infrastructure.Identity.IAuthorizationService _authService;

    public ReservationsController(
        IReservationService reservationService,
        Infrastructure.Identity.IAuthorizationService authService)
    {
        _reservationService = reservationService;
        _authService = authService;
    }

    /// <summary>
    /// Get current student's reservations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Package>>> GetMyReservations()
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            var reservations = await _reservationService.GetStudentReservationsAsync(currentStudentId.Value);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Make a reservation for a package
    /// </summary>
    [HttpPost("{packageId}")]
    public async Task<IActionResult> MakeReservation(int packageId)
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            await _reservationService.MakeReservationAsync(packageId, currentStudentId.Value);
            
            return CreatedAtAction(
                nameof(GetMyReservations), 
                new { message = "Reservation created successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpDelete("{packageId}")]
    public async Task<IActionResult> CancelReservation(int packageId)
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            await _reservationService.CancelReservationAsync(packageId, currentStudentId.Value);
            
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
    /// Check if student is eligible for a specific package
    /// </summary>
    [HttpGet("eligibility/{packageId}")]
    public async Task<ActionResult<bool>> CheckEligibility(int packageId)
    {
        try
        {
            var currentStudentId = await _authService.GetCurrentStudentIdAsync(User);
            if (currentStudentId == null)
                return Forbid();

            var isEligible = await _reservationService.IsStudentEligibleForPackageAsync(currentStudentId.Value, packageId);
            var isAvailable = await _reservationService.IsPackageAvailableAsync(packageId);
            
            return Ok(new { 
                isEligible = isEligible,
                isAvailable = isAvailable,
                canReserve = isEligible && isAvailable
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}