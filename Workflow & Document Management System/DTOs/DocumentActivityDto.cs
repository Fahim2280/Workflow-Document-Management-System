namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentActivityDto
    {
        public int ActivityId { get; set; }
        public string ActivityType { get; set; }
        public string Comments { get; set; }
        public DateTime ActivityDate { get; set; }
        public string AdminName { get; set; }
    }
}
