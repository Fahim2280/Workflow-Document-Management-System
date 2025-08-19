namespace Workflow___Document_Management_System.DTOs
{
    public class CreateWorkflowDto
    {
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; } // "Order" or "Pool"
        public string Description { get; set; }
        public List<int> AssignedAdminIds { get; set; } = new List<int>();
    }
}
