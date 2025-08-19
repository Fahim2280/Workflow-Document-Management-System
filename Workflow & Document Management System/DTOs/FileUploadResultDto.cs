namespace Workflow___Document_Management_System.DTOs
{
    public class FileUploadResultDto
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string ErrorMessage { get; set; }
    }
}
