using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Workflow___Document_Management_System.Models
{
    public class DocumentActivity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Required]
        public int AdminId { get; set; }

        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; } // 'Upload', 'Review', 'Approve', 'Reject', 'Complete'

        [StringLength(1000)]
        public string Comments { get; set; }

        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }

        // Helper Methods
        public bool IsUploadActivity => ActivityType?.ToUpper() == "UPLOAD";
        public bool IsReviewActivity => ActivityType?.ToUpper() == "REVIEW";
        public bool IsApproveActivity => ActivityType?.ToUpper() == "APPROVE";
        public bool IsRejectActivity => ActivityType?.ToUpper() == "REJECT";
        public bool IsCompleteActivity => ActivityType?.ToUpper() == "COMPLETE";
    }

    public enum WorkflowType
    {
        Order,
        Pool
    }

    public enum DocumentStatus
    {
        Pending,
        UnderReview,
        Completed,
        Rejected
    }

    public enum ActivityType
    {
        Upload,
        Review,
        Approve,
        Reject,
        Complete
    }

}
