using Microsoft.AspNetCore.Mvc;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.SERVICE;

namespace Workflow___Document_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentTypeController : ControllerBase
    {
        private readonly DocumentTypeService _documentTypeService;
        private readonly ValidationService _validationService;

        public DocumentTypeController(DocumentTypeService documentTypeService, ValidationService validationService)
        {
            _documentTypeService = documentTypeService;
            _validationService = validationService;   
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDocumentType([FromBody] CreateDocumentTypeDto dto)
        {
            try
            {
                var validationErrors = _validationService.ValidateCreateDocumentType(dto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Validation failed", validationErrors));
                }

                var result = await _documentTypeService.CreateDocumentTypeAsync(dto);

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
        public async Task<IActionResult> GetAllDocumentTypes()
        {
            try
            {
                var result = await _documentTypeService.GetAllDocumentTypesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateDocumentType([FromBody] UpdateDocumentTypeDto dto)
        {
            try
            {
                if (dto.DocumentTypeId <= 0)
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Valid document type ID is required"));

                var result = await _documentTypeService.UpdateDocumentTypeAsync(dto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Valid document type ID is required"));

                var result = await _documentTypeService.DeleteDocumentTypeAsync(id);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }
    }
}
