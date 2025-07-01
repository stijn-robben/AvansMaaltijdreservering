using System.Security.Claims;

namespace AvansMaaltijdreservering.Infrastructure.Identity;

public interface IAuthorizationService
{
    Task<int?> GetCurrentStudentIdAsync(ClaimsPrincipal user);
    Task<int?> GetCurrentCanteenEmployeeIdAsync(ClaimsPrincipal user);
    bool IsStudent(ClaimsPrincipal user);
    bool IsCanteenEmployee(ClaimsPrincipal user);
}