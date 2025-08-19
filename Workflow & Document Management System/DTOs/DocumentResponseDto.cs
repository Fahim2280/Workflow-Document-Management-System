namespace Workflow___Document_Management_System.DTOs
{
    public class DocumentResponseDto
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime UploadedDate { get; set; }
        public string DocumentTypeName { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string UploadedByUsername { get; set; }
        public string FilePath { get; set; }
        public string FileSizeFormatted => FormatFileSize(FileSize);

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

}
