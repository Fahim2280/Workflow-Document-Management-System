using Microsoft.AspNetCore.Mvc;
using WfDoc.Api.Application.Admins;

namespace WfDoc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAdminService _adminService;
    public AuthController(IAdminService adminService) => _adminService = adminService;

    public sealed record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (admin, token) = await _adminService.LoginAsync(request.Username, request.Password);
        return Ok(new { token, admin = new { admin.Id, admin.Username, accessLevel = admin.AccessLevel.ToString() } });
    }
}

