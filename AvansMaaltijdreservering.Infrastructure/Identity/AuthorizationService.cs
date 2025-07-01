using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AvansMaaltijdreservering.Infrastructure.Identity;

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<int?> GetCurrentStudentIdAsync(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
            return null;

        var appUser = await _userManager.GetUserAsync(user);
        return appUser?.StudentId;
    }

    public async Task<int?> GetCurrentCanteenEmployeeIdAsync(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
            return null;

        var appUser = await _userManager.GetUserAsync(user);
        return appUser?.CanteenEmployeeId;
    }

    public bool IsStudent(ClaimsPrincipal user)
    {
        return user.IsInRole(IdentityRoles.Student);
    }

    public bool IsCanteenEmployee(ClaimsPrincipal user)
    {
        return user.IsInRole(IdentityRoles.CanteenEmployee);
    }
}