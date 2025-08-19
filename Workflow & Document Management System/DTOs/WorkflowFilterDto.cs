namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowFilterDto
    {
        public int? WorkflowId { get; set; }
        public string WorkflowType { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? AssignedAdminId { get; set; }
    }
}
