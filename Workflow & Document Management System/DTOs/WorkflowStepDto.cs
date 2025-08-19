namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowStepDto
    {
        public int StepOrder { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string Status { get; set; } // "Pending", "Completed", "Skipped", "Current"
        public DateTime? CompletedDate { get; set; }
        public string CompletionType { get; set; }
        public string Comments { get; set; }
        public bool IsCurrentStep { get; set; }
        public bool IsCompleted { get; set; }
    }
}
