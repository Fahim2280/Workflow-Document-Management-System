using Microsoft.AspNetCore.Mvc;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.SERVICE;

namespace Workflow___Document_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly FileService _fileService;
        private readonly ValidationService _validationService;

        public DocumentController(
            DocumentService documentService,
            FileService fileService,
            ValidationService validationService)
        {
            _documentService = documentService;
            _fileService = fileService;
            _validationService = validationService;
        }

        [HttpPost("create")]
        [RequestSizeLimit(104857600)] // 100MB
        [RequestFormLimits(
            KeyLengthLimit = int.MaxValue,
            ValueLengthLimit = int.MaxValue,
            MultipartBodyLengthLimit = 104857600,
            MultipartHeadersCountLimit = 32,
            MultipartHeadersLengthLimit = 65536
        )]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), 200)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
        public async Task<IActionResult> CreateDocument([FromForm] CreateDocumentDto dto)
        {
            try
            {
                var validationErrors = _validationService.ValidateCreateDocument(dto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Validation failed", validationErrors));
                }

                var result = await _documentService.CreateDocumentAsync(dto);

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
        public async Task<IActionResult> GetAllDocuments()
        {
            try
            {
                var result = await _documentService.GetAllDocumentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Valid document ID is required"));

                var result = await _documentService.GetDocumentByIdAsync(id);

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("activity")]
        public async Task<IActionResult> AddDocumentActivity([FromBody] AddDocumentActivityDto dto)
        {
            try
            {
                if (dto.DocumentId <= 0)
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Valid document ID is required"));

                if (string.IsNullOrWhiteSpace(dto.ActivityType))
                    return BadRequest(ApiResponseDto<object>.ErrorResult("Activity type is required"));

                var result = await _documentService.AddDocumentActivityAsync(dto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<object>.ErrorResult($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Valid document ID is required");

                return await _fileService.DownloadFileAsync(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewDocument(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Valid document ID is required");

                return await _fileService.ViewFileAsync(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}