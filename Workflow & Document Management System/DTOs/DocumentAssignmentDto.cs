namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentAssignmentDto
    {
        public int AssignmentId { get; set; }
        public int DocumentId { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public DateTime AssignedDate { get; set; }
        public int StepOrder { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletionType { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
        public string StatusDisplayText => IsCompleted ?
            $"{CompletionType} on {CompletedDate:MM/dd/yyyy}" :
            IsActive ? "Pending" : "Inactive";
    }
}
