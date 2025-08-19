namespace Workflow___Document_Management_System.DTOs
{
    public class WorkflowSearchDto
    {
        public string SearchTerm { get; set; }
        public string WorkflowType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}