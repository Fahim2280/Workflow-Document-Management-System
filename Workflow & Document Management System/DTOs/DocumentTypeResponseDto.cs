namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentTypeResponseDto
    {
        public int DocumentTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string AllowedExtensions { get; set; }
        public int MaxFileSizeMB { get; set; }
        public int CreatedByAdminId { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
