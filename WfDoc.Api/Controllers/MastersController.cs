using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WfDoc.Api.Application.Masters;
using WfDoc.Api.Common;

namespace WfDoc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MastersController : ControllerBase
{
    private readonly IMasterService _masterService;
    public MastersController(IMasterService masterService) => _masterService = masterService;

    public sealed record CreateDocumentTypeRequest(string Name);
    public sealed record UpdateDocumentTypeRequest(string Name);

    [HttpPost("document-types")]
    [Authorize(Policy = PolicyNames.ReadWriteOnly)]
    public async Task<IActionResult> CreateDocumentType([FromBody] CreateDocumentTypeRequest request)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await _masterService.CreateDocumentTypeAsync(adminId, request.Name);
        return Created($"api/masters/document-types/{id}", new { id });
    }

    [HttpPut("document-types/{id:int}")]
    [Authorize(Policy = PolicyNames.ReadWriteOnly)]
    public async Task<IActionResult> UpdateDocumentType(int id, [FromBody] UpdateDocumentTypeRequest request)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _masterService.UpdateDocumentTypeAsync(adminId, id, request.Name);
        return NoContent();
    }

    [HttpDelete("document-types/{id:int}")]
    [Authorize(Policy = PolicyNames.ReadWriteOnly)]
    public async Task<IActionResult> DeleteDocumentType(int id)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _masterService.DeleteDocumentTypeAsync(adminId, id);
        return NoContent();
    }

    [HttpGet("document-types")]
    public async Task<IActionResult> ListDocumentTypes()
    {
        var types = await _masterService.GetDocumentTypesAsync();
        return Ok(types);
    }
}

