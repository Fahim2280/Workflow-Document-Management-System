namespace Workflow___Document_Management_System.DTOs
{
    public class CreateDocumentTypeDto
    {
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string AllowedExtensions { get; set; } = ".pdf,.docx,.xlsx,.jpg,.jpeg,.png";
        public int MaxFileSizeMB { get; set; } = 50;
    }
}
