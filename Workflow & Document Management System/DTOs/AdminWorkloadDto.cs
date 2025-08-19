namespace Workflow___Document_Management_System.DTOs
{
    public class AdminWorkloadDto
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int AssignedCount { get; set; }
        public int CompletedCount { get; set; }
        public int PendingCount { get; set; }
        public int OverdueCount { get; set; }
        public double AverageCompletionTimeHours { get; set; }
        public double WorkloadPercentage { get; set; }
    }
}
