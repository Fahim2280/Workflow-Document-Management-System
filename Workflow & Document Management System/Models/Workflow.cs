using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Workflow___Document_Management_System.Models
{
    public class Workflow
    {
        [Key]
        public int WorkflowId { get; set; }

        [Required]
        [StringLength(100)]
        public string WorkflowName { get; set; }

        [Required]
        [StringLength(10)]
        public string WorkflowType { get; set; } // "Order" or "Pool"

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int CreatedByAdminId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CreatedByAdminId")]
        public virtual Admin CreatedByAdmin { get; set; }

        public virtual ICollection<WorkflowAdmin> WorkflowAdmins { get; set; } = new List<WorkflowAdmin>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        // Helper Methods
        public bool IsOrderWorkflow => WorkflowType?.ToUpper() == "ORDER";
        public bool IsPoolWorkflow => WorkflowType?.ToUpper() == "POOL";
    }
}
