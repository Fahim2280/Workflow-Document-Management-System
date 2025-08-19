namespace Workflow___Document_Management_System.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int CompletedDocuments { get; set; }
        public int RejectedDocuments { get; set; }
        public int TotalWorkflows { get; set; }
        public int TotalDocumentTypes { get; set; }
        public List<DocumentResponseDto> RecentDocuments { get; set; } = new List<DocumentResponseDto>();
        public List<WorkflowStatsDto> WorkflowStats { get; set; } = new List<WorkflowStatsDto>();
    }
}
