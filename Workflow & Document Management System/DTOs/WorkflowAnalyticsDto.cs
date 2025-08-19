namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowAnalyticsDto
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public int TotalDocuments { get; set; }
        public int CompletedDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int RejectedDocuments { get; set; }
        public double AverageCompletionTimeHours { get; set; }
        public int OverdueDocuments { get; set; }
        public List<AdminWorkloadDto> AdminWorkloads { get; set; } = new List<AdminWorkloadDto>();
        public Dictionary<string, int> DocumentsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> AverageTimeByStep { get; set; } = new Dictionary<string, double>();
    }

}
