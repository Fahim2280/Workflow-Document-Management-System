namespace Workflow___Document_Management_System.DTOs
{
    public class AdminDashboardDto
    {
        public List<AdminTaskDto> AssignedTasks { get; set; } = new List<AdminTaskDto>();
        public AdminDashboardSummaryDto Summary { get; set; } = new AdminDashboardSummaryDto();
        public string AdminName { get; set; }
        public string AccessLevel { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
