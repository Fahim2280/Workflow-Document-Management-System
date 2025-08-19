using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Workflow___Document_Management_System.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [StringLength(255)]
        public string DocumentName { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        public int WorkflowId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; }

        [Required]
        public long FileSize { get; set; } // Size in bytes

        [Required]
        [StringLength(10)]
        public string FileExtension { get; set; }

        [Required]
        public int UploadedByAdminId { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string CurrentStatus { get; set; } = "Pending";

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }

        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; }

        [ForeignKey("UploadedByAdminId")]
        public virtual Admin UploadedByAdmin { get; set; }

        public virtual ICollection<DocumentActivity> DocumentActivities { get; set; } = new List<DocumentActivity>();

        // Helper Methods
        public string GetFormattedFileSize()
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = FileSize;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        public bool IsPending => CurrentStatus?.ToUpper() == "PENDING";
        public bool IsCompleted => CurrentStatus?.ToUpper() == "COMPLETED";
        public bool IsRejected => CurrentStatus?.ToUpper() == "REJECTED";
        public bool IsUnderReview => CurrentStatus?.ToUpper() == "UNDER REVIEW";
    }
}
