using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Identity;
using System.Security.Claims;

namespace AvansMaaltijdreservering.API.GraphQL;

public class Mutation
{
    // Make a reservation for a package
    public async Task<ReservationResult> MakeReservation(
        [Service] IReservationService reservationService,
        [Service] Infrastructure.Identity.IUserAuthorizationService authService,
        ClaimsPrincipal claimsPrincipal,
        int packageId)
    {
        try
        {
            var currentStudentId = await authService.GetCurrentStudentIdAsync(claimsPrincipal);
            if (currentStudentId == null)
            {
                return new ReservationResult
                {
                    Success = false,
                    Message = "Authentication required"
                };
            }

            await reservationService.MakeReservationAsync(packageId, currentStudentId.Value);
            
            return new ReservationResult
            {
                Success = true,
                Message = "Reservation created successfully",
                PackageId = packageId
            };
        }
        catch (Exception ex)
        {
            return new ReservationResult
            {
                Success = false,
                Message = ex.Message,
                PackageId = packageId
            };
        }
    }

    // Cancel a reservation
    public async Task<ReservationResult> CancelReservation(
        [Service] IReservationService reservationService,
        [Service] Infrastructure.Identity.IUserAuthorizationService authService,
        ClaimsPrincipal claimsPrincipal,
        int packageId)
    {
        try
        {
            var currentStudentId = await authService.GetCurrentStudentIdAsync(claimsPrincipal);
            if (currentStudentId == null)
            {
                return new ReservationResult
                {
                    Success = false,
                    Message = "Authentication required"
                };
            }

            await reservationService.CancelReservationAsync(packageId, currentStudentId.Value);
            
            return new ReservationResult
            {
                Success = true,
                Message = "Reservation cancelled successfully",
                PackageId = packageId
            };
        }
        catch (Exception ex)
        {
            return new ReservationResult
            {
                Success = false,
                Message = ex.Message,
                PackageId = packageId
            };
        }
    }

    // Register a new student
    public async Task<StudentRegistrationResult> RegisterStudent(
        [Service] IStudentService studentService,
        StudentInput input)
    {
        try
        {
            var student = new Student
            {
                Name = input.Name,
                DateOfBirth = input.DateOfBirth,
                StudentNumber = input.StudentNumber,
                Email = input.Email,
                StudyCity = input.StudyCity,
                PhoneNumber = input.PhoneNumber
            };

            await studentService.RegisterStudentAsync(student);
            
            return new StudentRegistrationResult
            {
                Success = true,
                Message = "Student registered successfully",
                Student = student
            };
        }
        catch (Exception ex)
        {
            return new StudentRegistrationResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}

// GraphQL response types
public class ReservationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? PackageId { get; set; }
}

public class StudentRegistrationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Student? Student { get; set; }
}

// GraphQL input types
public class StudentInput
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AvansMaaltijdreservering.Core.Domain.Enums.City StudyCity { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}