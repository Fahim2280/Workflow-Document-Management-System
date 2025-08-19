namespace Workflow___Document_Management_System.DTOs
{
    public class TaskNotificationDto
    {
        public int NotificationId { get; set; }
        public int AdminId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string NotificationType { get; set; } // "NewAssignment", "Overdue", "Completed", "Rejected"
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string Priority { get; set; } // "Low", "Medium", "High"
    }
}
