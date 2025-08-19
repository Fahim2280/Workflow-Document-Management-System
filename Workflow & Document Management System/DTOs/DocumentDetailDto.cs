namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentDetailDto
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime UploadedDate { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public int UploadedByAdminId { get; set; }
        public string UploadedByUsername { get; set; }
        public List<DocumentActivityDto> Activities { get; set; } = new List<DocumentActivityDto>();
    }
}
