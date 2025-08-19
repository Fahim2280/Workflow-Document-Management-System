using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Workflow___Document_Management_System.Models
{
    public class WorkflowAdmin
    {
        [Key]
        public int WorkflowAdminId { get; set; }

        [Required]
        public int WorkflowId { get; set; }

        [Required]
        public int AdminId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
    }

}
