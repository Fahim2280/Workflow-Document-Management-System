namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentAssignmentHistoryDto
    {
        public List<DocumentAssignmentDto> Assignments { get; set; } = new List<DocumentAssignmentDto>();
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public int PendingAssignments { get; set; }
        public string CurrentWorkflowStatus { get; set; }
        public int CurrentStepOrder { get; set; }
        public int TotalSteps { get; set; }
        public double WorkflowProgress => TotalSteps > 0 ? (double)CompletedAssignments / TotalSteps * 100 : 0;
    }
}
