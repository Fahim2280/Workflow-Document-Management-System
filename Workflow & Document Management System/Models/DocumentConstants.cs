using System.ComponentModel.DataAnnotations;

namespace Workflow___Document_Management_System.Models
{
    public class DocumentConstants
    {
        public static class FileExtensions
        {
            public const string PDF = ".pdf";
            public const string DOCX = ".docx";
            public const string XLSX = ".xlsx";
            public const string JPG = ".jpg";
            public const string JPEG = ".jpeg";
            public const string PNG = ".png";

            public static readonly List<string> DefaultAllowed = new()
            {
                PDF, DOCX, XLSX, JPG, JPEG, PNG
            };
        }

        public static class Status
        {
            public const string PENDING = "Pending";
            public const string UNDER_REVIEW = "Under Review";
            public const string COMPLETED = "Completed";
            public const string REJECTED = "Rejected";
        }

        public static class Activity
        {
            public const string UPLOAD = "Upload";
            public const string REVIEW = "Review";
            public const string APPROVE = "Approve";
            public const string REJECT = "Reject";
            public const string COMPLETE = "Complete";
        }

        public static class Workflow
        {
            public const string ORDER = "Order";
            public const string POOL = "Pool";
        }

        public static class FileSizes
        {
            public const int DEFAULT_MAX_SIZE_MB = 50;
            public const long MAX_FILE_SIZE_BYTES = DEFAULT_MAX_SIZE_MB * 1024 * 1024; // 50MB in bytes
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"File extension {extension} is not allowed. Allowed extensions: {string.Join(", ", _extensions)}");
                }
            }
            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeMB;

        public MaxFileSizeAttribute(int maxFileSizeMB)
        {
            _maxFileSizeMB = maxFileSizeMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSizeMB * 1024 * 1024)
                {
                    return new ValidationResult($"File size cannot exceed {_maxFileSizeMB} MB");
                }
            }
            return ValidationResult.Success;
        }
    }
}

