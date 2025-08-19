namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowDetailDto
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string Description { get; set; }
        public int CreatedByAdminId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<AssignedAdminDto> AssignedAdmins { get; set; } = new List<AssignedAdminDto>();
    }
}
