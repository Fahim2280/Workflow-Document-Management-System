using Microsoft.AspNetCore.Mvc;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.Repository;

namespace Workflow___Document_Management_System.SERVICE
{
    public class DashboardService
    {
        private readonly DocumentRepository _documentRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly DocumentTypeRepository _documentTypeRepository;

        public DashboardService(
            DocumentRepository documentRepository,
            WorkflowRepository workflowRepository,
            DocumentTypeRepository documentTypeRepository)
        {
            _documentRepository = documentRepository;
            _workflowRepository = workflowRepository;
            _documentTypeRepository = documentTypeRepository;
        }

        public async Task<ApiResponseDto<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllDocumentsAsync();
                var workflows = await _workflowRepository.GetAllWorkflowsAsync();
                var documentTypes = await _documentTypeRepository.GetAllDocumentTypesAsync();

                var stats = new DashboardStatsDto
                {
                    TotalDocuments = documents.Count,
                    PendingDocuments = documents.Count(d => d.CurrentStatus == "Pending"),
                    CompletedDocuments = documents.Count(d => d.CurrentStatus == "Completed"),
                    RejectedDocuments = documents.Count(d => d.CurrentStatus == "Rejected"),
                    TotalWorkflows = workflows.Count,
                    TotalDocumentTypes = documentTypes.Count,
                    RecentDocuments = documents.OrderByDescending(d => d.UploadedDate).Take(5).ToList()
                };

                // Calculate workflow stats
                foreach (var workflow in workflows)
                {
                    var workflowDocs = documents.Where(d => d.WorkflowName == workflow.WorkflowName).ToList();
                    stats.WorkflowStats.Add(new WorkflowStatsDto
                    {
                        WorkflowId = workflow.WorkflowId,
                        WorkflowName = workflow.WorkflowName,
                        WorkflowType = workflow.WorkflowType,
                        DocumentCount = workflowDocs.Count,
                        PendingCount = workflowDocs.Count(d => d.CurrentStatus == "Pending"),
                        CompletedCount = workflowDocs.Count(d => d.CurrentStatus == "Completed")
                    });
                }

                return ApiResponseDto<DashboardStatsDto>.SuccessResult(stats, "Dashboard stats retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<DashboardStatsDto>.ErrorResult($"Error retrieving dashboard stats: {ex.Message}");
            }
        }
    }

   
}
