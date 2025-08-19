namespace Workflow___Document_Management_System.DTOs
{
    public class CreateDocumentDto
    {
        public string DocumentName { get; set; }
        public int DocumentTypeId { get; set; }
        public int WorkflowId { get; set; }
        public IFormFile File { get; set; }
    }
}
