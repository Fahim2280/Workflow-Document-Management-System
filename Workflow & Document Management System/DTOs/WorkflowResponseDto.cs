namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowResponseDto
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string Description { get; set; }
        public int CreatedByAdminId { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string AssignedAdmins { get; set; }
        public List<AdminSummaryDto> AssignedAdminsList { get; set; } = new List<AdminSummaryDto>();
    }
}
