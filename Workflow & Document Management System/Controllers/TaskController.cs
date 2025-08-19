using Microsoft.AspNetCore.Mvc;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.SERVICE;

namespace Workflow___Document_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskAssignmentService _taskAssignmentService;
        private readonly EnhancedValidationService _validationService;

        public TaskController(TaskAssignmentService taskAssignmentService, EnhancedValidationService validationService)
        {
            _taskAssignmentService = taskAssignmentService;
            _validationService = validationService;
        }

        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var result = await _taskAssignmentService.GetMyTasksAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("process-action")]
        public async Task<IActionResult> ProcessDocumentAction([FromBody] ProcessDocumentActionDto dto)
        {
            try
            {
                var validationErrors = _validationService.ValidateProcessDocumentAction(dto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Validation failed", validationErrors));
                }

                var result = await _taskAssignmentService.ProcessDocumentActionAsync(dto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("reassign")]
        public async Task<IActionResult> ReassignDocument([FromBody] ReassignDocumentDto dto)
        {
            try
            {
                var validationErrors = _validationService.ValidateReassignDocument(dto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Validation failed", validationErrors));
                }

                var result = await _taskAssignmentService.ReassignDocumentAsync(dto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("batch-action")]
        public async Task<IActionResult> ProcessBatchDocumentAction([FromBody] BatchDocumentActionDto dto)
        {
            try
            {
                var validationErrors = _validationService.ValidateBatchDocumentAction(dto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Validation failed", validationErrors));
                }

                var result = await _taskAssignmentService.ProcessBatchDocumentActionAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("assignment-history/{documentId}")]
        public async Task<IActionResult> GetDocumentAssignmentHistory(int documentId)
        {
            try
            {
                if (documentId <= 0)
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Valid document ID is required"));

                var result = await _taskAssignmentService.GetDocumentAssignmentHistoryAsync(documentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }        
    }

}
