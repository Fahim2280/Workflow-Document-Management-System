namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowProgressDto
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
        public int CurrentStepOrder { get; set; }
        public double ProgressPercentage { get; set; }
        public string StatusDisplayText { get; set; }
    }

}
