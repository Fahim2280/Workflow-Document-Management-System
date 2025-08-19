namespace Workflow___Document_Management_System.DTOs
{
    public class ReassignmentHistoryDto
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public DateTime ReassignedDate { get; set; }
        public string ReassignedByUsername { get; set; }
        public string Comments { get; set; }
        public string Reason { get; set; }
        public int ReassignmentCount { get; set; }
    }
}
