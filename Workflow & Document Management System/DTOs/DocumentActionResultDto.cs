namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentActionResultDto
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string NextAction { get; set; }
        public string NextAdminUsername { get; set; }
        public bool IsWorkflowComplete { get; set; }
    }
}
