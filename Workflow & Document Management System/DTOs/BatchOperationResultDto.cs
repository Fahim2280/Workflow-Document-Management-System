namespace Workflow___Document_Management_System.DTOs
{
    public class BatchOperationResultDto
    {
        public int TotalDocuments { get; set; }
        public int SuccessfulOperations { get; set; }
        public int FailedOperations { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> SuccessMessages { get; set; } = new List<string>();
        public Dictionary<int, string> DocumentResults { get; set; } = new Dictionary<int, string>();
    }
}
