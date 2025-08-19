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

        public AdminController(AdminService adminService, SessionService sessionService, IConfiguration configuration)
        {
            _adminService = adminService;
            _sessionService = sessionService;
            _configuration = configuration;
        }

        // ===== DEBUG ENDPOINT 1: Test Database Connection =====
        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Test if table exists
                using var command = new SqlCommand("SELECT COUNT(*) FROM Admins", connection);
                var count = await command.ExecuteScalarAsync();

                return Ok(new
                {
                    message = "Database connection successful!",
                    adminCount = count,
                    connectionString = connectionString?.Split(';')[0] // Only show server part
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Database connection failed",
                    error = ex.Message
                });
            }
        }

        // ===== DEBUG ENDPOINT 2: Direct Query Without Stored Procedure =====
        [HttpGet("test-direct")]
        public async Task<IActionResult> TestDirectQuery()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("SELECT AdminId, Username, Email, AccessLevel, CreatedDate, IsActive FROM Admins WHERE IsActive = 1", connection);
                var admins = new List<object>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    admins.Add(new
                    {
                        AdminId = reader.GetInt32("AdminId"),
                        Username = reader.GetString("Username"),
                        Email = reader.GetString("Email"),
                        AccessLevel = reader.GetString("AccessLevel"),
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        IsActive = reader.GetBoolean("IsActive")
                    });
                }

                return Ok(new
                {
                    admins = admins,
                    count = admins.Count,
                    message = "Direct query successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Direct query failed",
                    error = ex.Message
                });
            }
        }

        // ===== DEBUG ENDPOINT 3: Create First Admin =====
        [HttpPost("create-first")]
        public async Task<IActionResult> CreateFirstAdmin()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if any admin exists
                using var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Admins", connection);
                var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (count == 0)
                {
                    // Insert first admin with plain password
                    using var insertCommand = new SqlCommand(@"
                        INSERT INTO Admins (Username, Email, PasswordHash, AccessLevel, CreatedByAdminId, IsActive)
                        VALUES ('admin', 'admin@company.com', '123', 'Read-Write', 1, 1)", connection);

                    await insertCommand.ExecuteNonQueryAsync();

                    return Ok(new
                    {
                        message = "First admin created successfully!",
                        username = "admin",
                        password = "123",
                        note = "You can now login with username 'admin' and password '123'"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = $"Admin(s) already exist ({count} found)",
                        note = "Use test-direct endpoint to see existing admins"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to create first admin",
                    error = ex.Message
                });
            }
        }

        // ===== ORIGINAL ENDPOINTS =====
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                    return BadRequest("Username and password are required");

                var success = await _adminService.LoginAsync(loginDto);
                if (success)
                {
                    return Ok(new
                    {
                        message = "Login successful",
                        username = _sessionService.GetCurrentUsername(),
                        accessLevel = _sessionService.GetCurrentAccessLevel()
                    });
                }

                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
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
                if (!_sessionService.IsLoggedIn())
                    return Unauthorized("You must be logged in");

                if (!_sessionService.HasWriteAccess())
                    return Forbid("You don't have permission to create admins");

                if (string.IsNullOrEmpty(createAdminDto.Username) ||
                    string.IsNullOrEmpty(createAdminDto.Password) ||
                    string.IsNullOrEmpty(createAdminDto.Email))
                    return BadRequest("All fields are required");

                var success = await _adminService.CreateAdminAsync(createAdminDto);
                if (success)
                    return Ok(new { message = "Admin created successfully" });

                return BadRequest("Failed to create admin");
            }
            catch (Exception ex)
            {
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
                var admins = await _adminService.GetAllAdminsAsync();
                return Ok(new
                {
                    admins = admins,
                    count = admins.Count,
                    message = "Admin list retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
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
            _adminService.Logout();
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("current")]
        public IActionResult GetCurrentAdmin()
        {
            if (!_sessionService.IsLoggedIn())
                return Unauthorized("Not logged in");

            return Ok(new
            {
                username = _sessionService.GetCurrentUsername(),
                accessLevel = _sessionService.GetCurrentAccessLevel(),
                hasWriteAccess = _sessionService.HasWriteAccess()
            });
        }
    }
}