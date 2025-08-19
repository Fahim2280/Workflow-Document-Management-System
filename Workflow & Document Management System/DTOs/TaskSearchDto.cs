namespace Workflow___Document_Management_System.DTOs
{
    public class TaskSearchDto : DocumentSearchDto
    {
        public int? AssignedToAdminId { get; set; }
        public string AssignmentStatus { get; set; } // "Pending", "Completed", "Overdue"
        public int? StepOrder { get; set; }
        public string CompletionType { get; set; } // "Approved", "Rejected"
        public bool? IsReassigned { get; set; }
        public DateTime? AssignedFromDate { get; set; }
        public DateTime? AssignedToDate { get; set; }
    }
}
