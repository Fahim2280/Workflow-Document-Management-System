namespace Workflow___Document_Management_System.DTOs
{
    public class AdminDashboardSummaryDto
    {
        public int TotalAssigned { get; set; }
        public int PendingCount { get; set; }
        public int UnderReviewCount { get; set; }
        public int OverdueCount { get; set; }
    }
}
