using Microsoft.AspNetCore.Mvc;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.SERVICE;

namespace Workflow___Document_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly WorkflowService _workflowService;
        private readonly ValidationService _validationService;

        public WorkflowController(WorkflowService workflowService, ValidationService validationService)
        {
            _workflowService = workflowService;
            _validationService = validationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowDto dto)
        {
            try
            {
                var validationErrors = _validationService.ValidateCreateWorkflow(dto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Validation failed", validationErrors));
                }

                var result = await _workflowService.CreateWorkflowAsync(dto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllWorkflows()
        {
            try
            {
                var result = await _workflowService.GetAllWorkflowsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflowById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Valid workflow ID is required"));

                var result = await _workflowService.GetWorkflowByIdAsync(id);

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }
    }
}
