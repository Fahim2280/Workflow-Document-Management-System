using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.SERVICE;

namespace Workflow___Document_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly SessionService _sessionService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, SessionService sessionService, IConfiguration configuration, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _sessionService = sessionService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto loginDto)
        {
            try
            {
                _logger.LogInformation($"Login attempt for username: {loginDto?.Username}");

                if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("Login failed: Username or password is empty");
                    return BadRequest(new { message = "Username and password are required" });
                }

                var success = await _adminService.LoginAsync(loginDto);
                if (success)
                {
                    var username = _sessionService.GetCurrentUsername();
                    var accessLevel = _sessionService.GetCurrentAccessLevel();

                    _logger.LogInformation($"Login successful for user: {username} with access level: {accessLevel}");

                    return Ok(new
                    {
                        message = "Login successful",
                        username = username,
                        accessLevel = accessLevel
                    });
                }

                _logger.LogWarning($"Login failed for username: {loginDto.Username}");
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Login error for username: {loginDto?.Username}");
                return BadRequest(new
                {
                    message = "Login error",
                    error = ex.Message
                });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto createAdminDto)
        {
            try
            {
                _logger.LogInformation($"Create admin attempt for username: {createAdminDto?.Username}");

                if (!_sessionService.IsLoggedIn())
                {
                    _logger.LogWarning("Create admin failed: User not logged in");
                    return Unauthorized(new { message = "You must be logged in" });
                }

                if (!_sessionService.HasWriteAccess())
                {
                    _logger.LogWarning("Create admin failed: User doesn't have write access");
                    return Forbid("You don't have permission to create admins");
                }

                if (string.IsNullOrEmpty(createAdminDto.Username) ||
                    string.IsNullOrEmpty(createAdminDto.Password) ||
                    string.IsNullOrEmpty(createAdminDto.Email))
                {
                    _logger.LogWarning("Create admin failed: Required fields missing");
                    return BadRequest(new { message = "All fields are required" });
                }

                var success = await _adminService.CreateAdminAsync(createAdminDto);
                if (success)
                {
                    _logger.LogInformation($"Admin created successfully: {createAdminDto.Username}");
                    return Ok(new { message = "Admin created successfully" });
                }

                _logger.LogWarning($"Failed to create admin: {createAdminDto.Username}");
                return BadRequest(new { message = "Failed to create admin" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Create admin error for username: {createAdminDto?.Username}");
                return BadRequest(new
                {
                    message = "Create admin error",
                    error = ex.Message
                });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                _logger.LogInformation("Getting all admins list");

                var admins = await _adminService.GetAllAdminsAsync();

                _logger.LogInformation($"Retrieved {admins.Count} admins from database");

                return Ok(new
                {
                    admins = admins,
                    count = admins.Count,
                    message = "Admin list retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin list");
                return StatusCode(500, new
                {
                    message = "Error getting admin list",
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                var username = _sessionService.GetCurrentUsername();
                _logger.LogInformation($"Logout for user: {username}");

                _adminService.Logout();
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return Ok(new { message = "Logged out successfully" }); // Always return success for logout
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrentAdmin()
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                {
                    _logger.LogWarning("Get current admin failed: User not logged in");
                    return Unauthorized(new { message = "Not logged in" });
                }

                var username = _sessionService.GetCurrentUsername();
                var accessLevel = _sessionService.GetCurrentAccessLevel();
                var hasWriteAccess = _sessionService.HasWriteAccess();

                _logger.LogInformation($"Current admin info requested: {username}");

                return Ok(new
                {
                    username = username,
                    accessLevel = accessLevel,
                    hasWriteAccess = hasWriteAccess
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current admin info");
                return StatusCode(500, new { message = "Error getting current admin info" });
            }
        }

        // Add a test endpoint to check if API is working
        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok(new
            {
                message = "Admin API is working",
                timestamp = DateTime.Now,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        }
    }
}