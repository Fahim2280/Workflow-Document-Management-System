using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.Repository;

namespace Workflow___Document_Management_System.SERVICE
{
    public class WorkflowService
    {
        private readonly WorkflowRepository _workflowRepository;
        private readonly SessionService _sessionService;

        public WorkflowService(WorkflowRepository workflowRepository, SessionService sessionService)
        {
            _workflowRepository = workflowRepository;
            _sessionService = sessionService;
        }

        public async Task<ApiResponseDto<int>> CreateWorkflowAsync(CreateWorkflowDto dto)
        {
            try
            {
                if (!_sessionService.IsLoggedIn())
                    return ApiResponseDto<int>.ErrorResult("You must be logged in to create workflows");

                if (!_sessionService.HasWriteAccess())
                    return ApiResponseDto<int>.ErrorResult("You don't have permission to create workflows");

                if (string.IsNullOrWhiteSpace(dto.WorkflowName))
                    return ApiResponseDto<int>.ErrorResult("Workflow name is required");

                if (dto.WorkflowType != "Order" && dto.WorkflowType != "Pool")
                    return ApiResponseDto<int>.ErrorResult("Workflow type must be 'Order' or 'Pool'");

                if (dto.AssignedAdminIds == null || !dto.AssignedAdminIds.Any())
                    return ApiResponseDto<int>.ErrorResult("At least one admin must be assigned to the workflow");

                var currentAdminId = _sessionService.GetCurrentAdminId();
                if (!currentAdminId.HasValue)
                    return ApiResponseDto<int>.ErrorResult("Unable to determine current admin");

                var workflowId = await _workflowRepository.CreateWorkflowAsync(dto, currentAdminId.Value);

                if (workflowId > 0)
                    return ApiResponseDto<int>.SuccessResult(workflowId, "Workflow created successfully");

                return ApiResponseDto<int>.ErrorResult("Failed to create workflow");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<int>.ErrorResult($"Error creating workflow: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<WorkflowResponseDto>>> GetAllWorkflowsAsync()
        {
            try
            {
                var workflows = await _workflowRepository.GetAllWorkflowsAsync();
                return ApiResponseDto<List<WorkflowResponseDto>>.SuccessResult(workflows, "Workflows retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<WorkflowResponseDto>>.ErrorResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<WorkflowDetailDto>> GetWorkflowByIdAsync(int workflowId)
        {
            try
            {
                var workflow = await _workflowRepository.GetWorkflowByIdAsync(workflowId);

                if (workflow == null)
                    return ApiResponseDto<WorkflowDetailDto>.ErrorResult("Workflow not found");

                return ApiResponseDto<WorkflowDetailDto>.SuccessResult(workflow, "Workflow retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<WorkflowDetailDto>.ErrorResult($"Error retrieving workflow: {ex.Message}");
            }
        }
    }
}
