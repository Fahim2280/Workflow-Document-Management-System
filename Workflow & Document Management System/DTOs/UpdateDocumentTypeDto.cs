namespace Workflow___Document_Management_System.DTOs
{
    public class UpdateDocumentTypeDto
    {
        public int DocumentTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string AllowedExtensions { get; set; }
        public int MaxFileSizeMB { get; set; }
    }
}
