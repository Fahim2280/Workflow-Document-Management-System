using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.SERVICE
{
    public class DocumentTypeService
    {
        private readonly DocumentTypeRepository _documentTypeRepository;
        private readonly SessionService _sessionService;

        public DocumentTypeService(DocumentTypeRepository documentTypeRepository, SessionService sessionService)
        {
            _documentTypeRepository = documentTypeRepository;
            _sessionService = sessionService;
        }

        public async Task<ApiResponseDto<int>> CreateDocumentTypeAsync(CreateDocumentTypeDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<int>.ErrorResult("You must be logged in to create document types");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<int>.ErrorResult("You don't have permission to create document types");

                if (string.IsNullOrWhiteSpace(dto.TypeName))
                    return ApiResponseDto<int>.ErrorResult("Type name is required");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<int>.ErrorResult("Unable to determine current admin");

                var documentTypeId = await _documentTypeRepository.CreateDocumentTypeAsync(dto, currentAdminId.Value);

                if (documentTypeId > 0)
                    return ApiResponseDto<int>.SuccessResult(documentTypeId, "Document type created successfully");

                return ApiResponseDto<int>.ErrorResult("Failed to create document type");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<int>.ErrorResult($"Error creating document type: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<DocumentTypeResponseDto>>> GetAllDocumentTypesAsync()
        {
            try
            {
                var documentTypes = await _documentTypeRepository.GetAllDocumentTypesAsync();
                return ApiResponseDto<List<DocumentTypeResponseDto>>.SuccessResult(documentTypes, "Document types retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<DocumentTypeResponseDto>>.ErrorResult($"Error retrieving document types: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> UpdateDocumentTypeAsync(UpdateDocumentTypeDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<bool>.ErrorResult("You must be logged in to update document types");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<bool>.ErrorResult("You don't have permission to update document types");

                if (string.IsNullOrWhiteSpace(dto.TypeName))
                    return ApiResponseDto<bool>.ErrorResult("Type name is required");

                var success = await _documentTypeRepository.UpdateDocumentTypeAsync(dto);

                if (success)
                    return ApiResponseDto<bool>.SuccessResult(true, "Document type updated successfully");

                return ApiResponseDto<bool>.ErrorResult("Failed to update document type");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"Error updating document type: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteDocumentTypeAsync(int documentTypeId)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<bool>.ErrorResult("You must be logged in to delete document types");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<bool>.ErrorResult("You don't have permission to delete document types");

                var success = await _documentTypeRepository.DeleteDocumentTypeAsync(documentTypeId);

                if (success)
                    return ApiResponseDto<bool>.SuccessResult(true, "Document type deleted successfully");

                return ApiResponseDto<bool>.ErrorResult("Failed to delete document type");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"Error deleting document type: {ex.Message}");
            }
        }
    }
}
