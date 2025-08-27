using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AvansMaaltijdreservering.Infrastructure.Identity;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Login endpoint for employees and students
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user, roles);

            return Ok(new LoginResponse
            {
                Token = token,
                Email = user.Email,
                Roles = roles.ToList(),
                EmployeeId = user.CanteenEmployeeId,
                StudentId = user.StudentId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Login failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Register test users (DEVELOPMENT ONLY - REMOVE IN PRODUCTION)
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> RegisterTestUser([FromBody] RegisterRequest request)
    {
        // SECURITY: Only allow in Development environment
        if (!_environment.IsDevelopment())
        {
            return BadRequest(new { message = "Registration is disabled in production" });
        }
        
        try
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                EmployeeNumber = request.EmployeeNumber,
                StudentNumber = request.StudentNumber,
                CanteenEmployeeId = request.CanteenEmployeeId,
                StudentId = request.StudentId
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            // Add role if specified
            if (!string.IsNullOrEmpty(request.Role))
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            return Ok(new { message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Registration failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Get current user info (requires authentication)
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserInfo>> GetCurrentUser()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserInfo
            {
                Email = user.Email,
                Roles = roles.ToList(),
                EmployeeId = user.CanteenEmployeeId,
                StudentId = user.StudentId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to get user info", details = ex.Message });
        }
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add employee/student ID claims
        if (user.CanteenEmployeeId.HasValue)
        {
            claims.Add(new Claim("CanteenEmployeeId", user.CanteenEmployeeId.Value.ToString()));
        }
        if (user.StudentId.HasValue)
        {
            claims.Add(new Claim("StudentId", user.StudentId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourDefaultSecretKeyThatIsLongEnoughForHMAC256"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddHours(24);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"] ?? "AvansMaaltijdreservering",
            _configuration["Jwt:Audience"] ?? "AvansMaaltijdreservering",
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTOs
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int? EmployeeId { get; set; }
    public int? StudentId { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? EmployeeNumber { get; set; }
    public string? StudentNumber { get; set; }
    public int? CanteenEmployeeId { get; set; }
    public int? StudentId { get; set; }
}

public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int? EmployeeId { get; set; }
    public int? StudentId { get; set; }
}