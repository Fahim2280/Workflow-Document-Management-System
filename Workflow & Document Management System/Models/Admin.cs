namespace Workflow___Document_Management_System.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string AccessLevel { get; set; } // "Read-Write" or "Read-Only"
        public int CreatedByAdminId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
