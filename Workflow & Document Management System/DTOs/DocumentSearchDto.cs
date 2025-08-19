namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentSearchDto
    {
        public string SearchTerm { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? WorkflowId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "UploadedDate";
        public string SortOrder { get; set; } = "DESC";
    }
}
