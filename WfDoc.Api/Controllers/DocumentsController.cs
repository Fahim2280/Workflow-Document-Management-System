using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WfDoc.Api.Application.Documents;
using WfDoc.Api.Common;

namespace WfDoc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;
    private readonly IWebHostEnvironment _env;
    public DocumentsController(IDocumentService documents, IWebHostEnvironment env)
    {
        _documents = documents;
        _env = env;
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.ReadWriteOnly)]
    [RequestSizeLimit(104_857_600)] // 100 MB
    public async Task<IActionResult> Create([FromForm] string name, [FromForm] int documentTypeId, [FromForm] int workflowId, [FromForm] IFormFile file)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var storage = Path.Combine(_env.ContentRootPath, "storage");
        Directory.CreateDirectory(storage);
        var fileName = $"{Guid.NewGuid():N}_{file.FileName}";
        var fullPath = Path.Combine(storage, fileName);
        await using (var fs = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(fs);
        }

        var id = await _documents.CreateDocumentAsync(adminId, name, documentTypeId, fullPath, workflowId);
        return Created($"api/documents/{id}", new { id });
    }

    [HttpGet("my-tasks")]
    public async Task<IActionResult> MyTasks()
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var docs = await _documents.GetMyTasksAsync(adminId);
        return Ok(docs);
    }

    public sealed record DecisionRequest(string? Comment);

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] DecisionRequest request)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _documents.ApproveAsync(adminId, id, request.Comment);
        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] DecisionRequest request)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _documents.RejectAsync(adminId, id, request.Comment);
        return NoContent();
    }

    [HttpPost("{id:int}/reassign")]
    public async Task<IActionResult> Reassign(int id)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _documents.ReassignAsync(adminId, id);
        return NoContent();
    }
}

