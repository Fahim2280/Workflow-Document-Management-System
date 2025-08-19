namespace Workflow___Document_Management_System.DTOs
{
    public class ProcessDocumentActionDto
    {
        public int DocumentId { get; set; }
        public string Action { get; set; } // "Approve", "Reject", "Complete"
        public string Comments { get; set; }
    }
}
