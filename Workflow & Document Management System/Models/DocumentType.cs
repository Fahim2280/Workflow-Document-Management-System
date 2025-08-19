using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Workflow___Document_Management_System.Models
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(200)]
        public string AllowedExtensions { get; set; } = ".pdf,.docx,.xlsx,.jpg,.jpeg,.png";

        public int MaxFileSizeMB { get; set; } = 50;

        [Required]
        public int CreatedByAdminId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CreatedByAdminId")]
        public virtual Admin CreatedByAdmin { get; set; }

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        // Helper Methods
        public List<string> GetAllowedExtensionsList()
        {
            return AllowedExtensions?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(ext => ext.Trim().ToLower())
                                   .ToList() ?? new List<string>();
        }

        public bool IsExtensionAllowed(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return false;
            var allowedExtensions = GetAllowedExtensionsList();
            return allowedExtensions.Contains(extension.ToLower());
        }
    }

}
