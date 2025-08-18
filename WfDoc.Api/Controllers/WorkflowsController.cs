using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WfDoc.Api.Application.Workflows;
using WfDoc.Api.Common;

namespace WfDoc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WorkflowsController : ControllerBase
{
    private readonly IWorkflowService _workflows;
    public WorkflowsController(IWorkflowService workflows) => _workflows = workflows;

    public sealed record CreateWorkflowRequest(string Name, WorkflowType Type, int[] AdminIds);

    [HttpPost]
    [Authorize(Policy = PolicyNames.ReadWriteOnly)]
    public async Task<IActionResult> Create([FromBody] CreateWorkflowRequest request)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await _workflows.CreateWorkflowAsync(adminId, request.Name, request.Type, request.AdminIds);
        return Created($"api/workflows/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var workflows = await _workflows.GetWorkflowsAsync();
        return Ok(workflows);
    }
}

