namespace Workflow___Document_Management_System.DTOs
{
    public class BatchDocumentActionDto
    {
        public List<int> DocumentIds { get; set; } = new List<int>();
        public string Action { get; set; } // "Approve", "Reject", "Reassign"
        public string Comments { get; set; }
        public int? ReassignToWorkflowId { get; set; }
    }
}
