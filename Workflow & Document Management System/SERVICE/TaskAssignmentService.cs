using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.Repository;

namespace Workflow___Document_Management_System.SERVICE
{
    public class TaskAssignmentService
    {
        private readonly TaskAssignmentRepository _taskAssignmentRepository;

        private readonly SessionService _sessionService;
        private readonly ValidationService _validationService;

        public TaskAssignmentService(
            TaskAssignmentRepository taskAssignmentRepository,

            SessionService sessionService,
            ValidationService validationService)
        {
            _taskAssignmentRepository = taskAssignmentRepository;

            _sessionService = sessionService;
            _validationService = validationService;
        }

        public async Task<ApiResponseDto<List<AdminTaskDto>>> GetMyTasksAsync()
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<List<AdminTaskDto>>.ErrorResult("You must be logged in to view tasks");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<List<AdminTaskDto>>.ErrorResult("Unable to determine current admin");

                var (tasks, _) = await _taskAssignmentRepository.GetAdminDashboardTasksAsync(currentAdminId.Value);

                return ApiResponseDto<List<AdminTaskDto>>.SuccessResult(tasks, "Tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<AdminTaskDto>>.ErrorResult($"Error retrieving tasks: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DocumentActionResultDto>> ProcessDocumentActionAsync(ProcessDocumentActionDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<DocumentActionResultDto>.ErrorResult("You must be logged in to process documents");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<DocumentActionResultDto>.ErrorResult("You don't have permission to process documents");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<DocumentActionResultDto>.ErrorResult("Unable to determine current admin");

                // Validate the action
                var validationErrors = ValidateDocumentAction(dto);
                if (validationErrors.Any())
                    return ApiResponseDto<DocumentActionResultDto>.ErrorResult("Validation failed", validationErrors);

                // Check if admin has an active assignment for this document
                         
                var result = await _taskAssignmentRepository.ProcessDocumentActionAsync(dto, currentAdminId.Value);

                return ApiResponseDto<DocumentActionResultDto>.SuccessResult(result, result.Message);
            }
            catch (Exception ex)
            {
                return ApiResponseDto<DocumentActionResultDto>.ErrorResult($"Error processing document action: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> ReassignDocumentAsync(ReassignDocumentDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<bool>.ErrorResult("You must be logged in to reassign documents");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<bool>.ErrorResult("You don't have permission to reassign documents");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<bool>.ErrorResult("Unable to determine current admin");

                // Validate the reassignment
                if (dto.DocumentId <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Valid document ID is required");

                if (string.IsNullOrWhiteSpace(dto.Comments))
                    return ApiResponseDto<bool>.ErrorResult("Comments are required for reassignment");

                var success = await _taskAssignmentRepository.ReassignDocumentAsync(dto, currentAdminId.Value);

                if (success)
                    return ApiResponseDto<bool>.SuccessResult(true, "Document reassigned successfully");

                return ApiResponseDto<bool>.ErrorResult("Failed to reassign document");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"Error reassigning document: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DocumentAssignmentHistoryDto>> GetDocumentAssignmentHistoryAsync(int documentId)
        {
            try
            {
                var history = await _taskAssignmentRepository.GetDocumentAssignmentHistoryAsync(documentId);
                return ApiResponseDto<DocumentAssignmentHistoryDto>.SuccessResult(history, "Assignment history retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<DocumentAssignmentHistoryDto>.ErrorResult($"Error retrieving assignment history: {ex.Message}");
            }
        }
     
        public async Task<ApiResponseDto<BatchOperationResultDto>> ProcessBatchDocumentActionAsync(BatchDocumentActionDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<BatchOperationResultDto>.ErrorResult("You must be logged in to process batch actions");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<BatchOperationResultDto>.ErrorResult("You don't have permission to process batch actions");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<BatchOperationResultDto>.ErrorResult("Unable to determine current admin");

                if (!dto.DocumentIds?.Any() == true)
                    return ApiResponseDto<BatchOperationResultDto>.ErrorResult("No documents selected for batch operation");

                var result = new BatchOperationResultDto
                {
                    TotalDocuments = dto.DocumentIds.Count
                };

                foreach (var documentId in dto.DocumentIds)
                {
                    try
                    {
                        if (dto.Action == "Reassign")
                        {
                            var reassignDto = new ReassignDocumentDto
                            {
                                DocumentId = documentId,
                                Comments = dto.Comments
                            };
                            var reassignResult = await ReassignDocumentAsync(reassignDto);

                            if (reassignResult.Success)
                            {
                                result.SuccessfulOperations++;
                                result.SuccessMessages.Add($"Document {documentId} reassigned successfully");
                                result.DocumentResults[documentId] = "Success";
                            }
                            else
                            {
                                result.FailedOperations++;
                                result.Errors.Add($"Document {documentId}: {reassignResult.Message}");
                                result.DocumentResults[documentId] = reassignResult.Message;
                            }
                        }
                        else
                        {
                            var actionDto = new ProcessDocumentActionDto
                            {
                                DocumentId = documentId,
                                Action = dto.Action,
                                Comments = dto.Comments
                            };
                            var actionResult = await ProcessDocumentActionAsync(actionDto);

                            if (actionResult.Success)
                            {
                                result.SuccessfulOperations++;
                                result.SuccessMessages.Add($"Document {documentId} {dto.Action.ToLower()}ed successfully");
                                result.DocumentResults[documentId] = "Success";
                            }
                            else
                            {
                                result.FailedOperations++;
                                result.Errors.Add($"Document {documentId}: {actionResult.Message}");
                                result.DocumentResults[documentId] = actionResult.Message;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailedOperations++;
                        result.Errors.Add($"Document {documentId}: {ex.Message}");
                        result.DocumentResults[documentId] = ex.Message;
                    }
                }

                var message = $"Batch operation completed: {result.SuccessfulOperations} successful, {result.FailedOperations} failed";
                return ApiResponseDto<BatchOperationResultDto>.SuccessResult(result, message);
            }
            catch (Exception ex)
            {
                return ApiResponseDto<BatchOperationResultDto>.ErrorResult($"Error processing batch action: {ex.Message}");
            }
        }

        private List<string> ValidateDocumentAction(ProcessDocumentActionDto dto)
        {
            var errors = new List<string>();

            if (dto.DocumentId <= 0)
                errors.Add("Valid document ID is required");

            if (string.IsNullOrWhiteSpace(dto.Action))
                errors.Add("Action is required");

            var validActions = new[] { "Approve", "Reject", "Complete" };
            if (!validActions.Contains(dto.Action))
                errors.Add($"Invalid action. Valid actions: {string.Join(", ", validActions)}");

            if (dto.Action == "Reject" && string.IsNullOrWhiteSpace(dto.Comments))
                errors.Add("Comments are required when rejecting a document");

            return errors;
        }
    }
}
