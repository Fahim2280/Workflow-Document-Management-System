namespace Workflow___Document_Management_System.DTOs
{
    public class FileValidationDto
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> AllowedExtensions { get; set; } = new List<string>();
        public int MaxFileSizeMB { get; set; }
    }
}
