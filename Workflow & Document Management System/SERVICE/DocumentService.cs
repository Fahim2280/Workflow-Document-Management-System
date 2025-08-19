using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.Repository;

namespace Workflow___Document_Management_System.SERVICE
{
    public class DocumentService
    {
        private readonly DocumentRepository _documentRepository;
        private readonly DocumentTypeRepository _documentTypeRepository;
        private readonly FileRepository _fileRepository;
        private readonly SessionService _sessionService;

        public DocumentService(
            DocumentRepository documentRepository,
            DocumentTypeRepository documentTypeRepository,
            FileRepository fileRepository,
            SessionService sessionService)
        {
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _fileRepository = fileRepository;
            _sessionService = sessionService;
        }

        public async Task<ApiResponseDto<int>> CreateDocumentAsync(CreateDocumentDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<int>.ErrorResult("You must be logged in to create documents");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<int>.ErrorResult("You don't have permission to create documents");

                if (string.IsNullOrWhiteSpace(dto.DocumentName))
                    return ApiResponseDto<int>.ErrorResult("Document name is required");

                if (dto.File == null || dto.File.Length == 0)
                    return ApiResponseDto<int>.ErrorResult("File is required");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<int>.ErrorResult("Unable to determine current admin");

                // Get document type to validate file
                var documentTypes = await _documentTypeRepository.GetAllDocumentTypesAsync();
                var documentType = documentTypes.FirstOrDefault(dt => dt.DocumentTypeId == dto.DocumentTypeId);

                if (documentType == null)
                    return ApiResponseDto<int>.ErrorResult("Invalid document type");

                // Validate file
                var fileValidation = _fileRepository.ValidateFile(dto.File, documentType);
                if (!fileValidation.IsValid)
                    return ApiResponseDto<int>.ErrorResult(fileValidation.ErrorMessage);

                // Save file
                var fileUploadResult = await _fileRepository.SaveFileAsync(dto.File, "documents");
                if (!fileUploadResult.Success)
                    return ApiResponseDto<int>.ErrorResult($"Failed to save file: {fileUploadResult.ErrorMessage}");

                // Create document record
                var documentId = await _documentRepository.CreateDocumentAsync(
                    dto,
                    fileUploadResult.FileName,
                    fileUploadResult.FilePath,
                    fileUploadResult.FileSize,
                    fileUploadResult.FileExtension,
                    currentAdminId.Value);

                if (documentId > 0)
                    return ApiResponseDto<int>.SuccessResult(documentId, "Document created successfully");

                // If document creation failed, delete the uploaded file
                _fileRepository.DeleteFile(fileUploadResult.FilePath);
                return ApiResponseDto<int>.ErrorResult("Failed to create document");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<int>.ErrorResult($"Error creating document: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<DocumentResponseDto>>> GetAllDocumentsAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllDocumentsAsync();
                return ApiResponseDto<List<DocumentResponseDto>>.SuccessResult(documents, "Documents retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<DocumentResponseDto>>.ErrorResult($"Error retrieving documents: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DocumentDetailDto>> GetDocumentByIdAsync(int documentId)
        {
            try
            {
                var document = await _documentRepository.GetDocumentByIdAsync(documentId);

                if (document == null)
                    return ApiResponseDto<DocumentDetailDto>.ErrorResult("Document not found");

                return ApiResponseDto<DocumentDetailDto>.SuccessResult(document, "Document retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<DocumentDetailDto>.ErrorResult($"Error retrieving document: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> AddDocumentActivityAsync(AddDocumentActivityDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<bool>.ErrorResult("You must be logged in to add document activities");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<bool>.ErrorResult("You don't have permission to add document activities");

                if (string.IsNullOrWhiteSpace(dto.ActivityType))
                    return ApiResponseDto<bool>.ErrorResult("Activity type is required");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<bool>.ErrorResult("Unable to determine current admin");

                // Validate activity type
                var validActivityTypes = new[] { "Review", "Approve", "Reject", "Complete" };
                if (!validActivityTypes.Contains(dto.ActivityType))
                    return ApiResponseDto<bool>.ErrorResult($"Invalid activity type. Valid types: {string.Join(", ", validActivityTypes)}");

                var success = await _documentRepository.AddDocumentActivityAsync(dto, currentAdminId.Value);

                if (success)
                    return ApiResponseDto<bool>.SuccessResult(true, "Document activity added successfully");

                return ApiResponseDto<bool>.ErrorResult("Failed to add document activity");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"Error adding document activity: {ex.Message}");
            }
        }
    }

}
