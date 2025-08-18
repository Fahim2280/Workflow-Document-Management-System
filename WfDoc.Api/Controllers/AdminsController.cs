using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WfDoc.Api.Application.Admins;
using WfDoc.Api.Common;

namespace WfDoc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AdminsController : ControllerBase
{
    private readonly IAdminService _adminService;
    public AdminsController(IAdminService adminService) => _adminService = adminService;

    public sealed record CreateAdminRequest(string Username, string Password, AccessLevel AccessLevel);

    [HttpPost]
    [Authorize(Policy = PolicyNames.ReadWriteOnly)]
    public async Task<IActionResult> Create([FromBody] CreateAdminRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("nameidentifier")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await _adminService.CreateAdminAsync(creatorId, request.Username, request.Password, request.AccessLevel);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var admins = await _adminService.GetAdminsAsync();
        return Ok(admins.Select(a => new { a.Id, a.Username, accessLevel = a.AccessLevel.ToString(), a.CreatedAt, a.CreatedByAdminId }));
    }
}

