namespace Workflow___Document_Management_System.DTOs
{
    public class AddDocumentActivityDto
    {
        public int DocumentId { get; set; }
        public string ActivityType { get; set; } // 'Review', 'Approve', 'Reject', 'Complete'
        public string Comments { get; set; }
    }
}
