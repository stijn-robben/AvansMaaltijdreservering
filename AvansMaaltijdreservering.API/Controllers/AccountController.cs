using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ICanteenEmployeeRepository _employeeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICanteenRepository _canteenRepository;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ICanteenEmployeeRepository employeeRepository,
        IStudentRepository studentRepository,
        ICanteenRepository canteenRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _environment = environment;
        _employeeRepository = employeeRepository;
        _studentRepository = studentRepository;
        _canteenRepository = canteenRepository;
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
                Email = user.Email ?? string.Empty,
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
    /// Register users (can be enabled/disabled via configuration)
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> RegisterTestUser([FromBody] RegisterRequest request)
    {
        // Check if registration is allowed via configuration
        var allowRegistration = _configuration.GetValue<bool>("Registration:AllowRegistration", false);
        if (!allowRegistration)
        {
            return BadRequest(new { message = "Registration is disabled in production" });
        }
        
        try
        {
            // Create business entity first (CanteenEmployee or Student)
            int? canteenEmployeeId = null;
            int? studentId = null;

            if (request.Role == IdentityRoles.CanteenEmployee)
            {
                if (!request.WorksAtCanteen.HasValue)
                {
                    return BadRequest(new { message = "WorksAtCanteen is required for employee registration" });
                }

                // Find the canteen by location
                var canteen = await _canteenRepository.GetByLocationAsync(request.WorksAtCanteen.Value);
                
                if (canteen == null)
                {
                    return BadRequest(new { message = $"Canteen at location {request.WorksAtCanteen.Value} not found. Please run database seed script first." });
                }

                // Generate a more user-friendly name from email
                var emailPrefix = request.Email.Split('@')[0];
                var displayName = emailPrefix.Replace(".", " ").Replace("_", " ");
                displayName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(displayName.ToLower());
                
                var employee = new CanteenEmployee
                {
                    Name = displayName,
                    EmployeeNumber = request.EmployeeNumber ?? GenerateUniqueEmployeeNumber(),
                    CanteenId = canteen.Id
                };
                
                var createdEmployee = await _employeeRepository.AddAsync(employee);
                canteenEmployeeId = createdEmployee.Id;
            }
            else if (request.Role == IdentityRoles.Student)
            {
                // Validate required student fields
                if (string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest(new { message = "Name is required for student registration" });
                }
                
                if (!request.DateOfBirth.HasValue)
                {
                    return BadRequest(new { message = "Date of birth is required for student registration" });
                }
                
                if (!request.StudyCity.HasValue)
                {
                    return BadRequest(new { message = "Study city is required for student registration" });
                }
                
                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    return BadRequest(new { message = "Phone number is required for student registration" });
                }
                
                var student = new Student
                {
                    Name = request.Name,
                    Email = request.Email,
                    StudentNumber = request.StudentNumber ?? GenerateUniqueStudentNumber(),
                    PhoneNumber = request.PhoneNumber,
                    StudyCity = request.StudyCity.Value,
                    DateOfBirth = request.DateOfBirth.Value
                };
                
                var createdStudent = await _studentRepository.AddAsync(student);
                studentId = createdStudent.Id;
            }

            // Create Identity user with proper linking
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                EmployeeNumber = request.EmployeeNumber,
                StudentNumber = request.StudentNumber,
                CanteenEmployeeId = canteenEmployeeId,
                StudentId = studentId
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

            return Ok(new { 
                message = "User registered successfully",
                canteenEmployeeId = canteenEmployeeId,
                studentId = studentId
            });
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
                Email = user.Email ?? string.Empty,
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
            new(ClaimTypes.Email, user.Email ?? string.Empty),
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

        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT:Key configuration is missing. Please configure a secure JWT key.");
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
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

    private string GenerateUniqueEmployeeNumber()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(100, 999);
        return $"EMP{timestamp}{random}";
    }

    private string GenerateUniqueStudentNumber()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(100, 999);
        return $"STU{timestamp}{random}";
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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CanteenLocation? WorksAtCanteen { get; set; }
    
    // Student-specific fields
    public string? Name { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public City? StudyCity { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int? EmployeeId { get; set; }
    public int? StudentId { get; set; }
}