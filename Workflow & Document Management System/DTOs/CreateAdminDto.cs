namespace Workflow___Document_Management_System.DTOs
{
    public class CreateAdminDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AccessLevel { get; set; } // "Read-Write" or "Read-Only"
    }
}
