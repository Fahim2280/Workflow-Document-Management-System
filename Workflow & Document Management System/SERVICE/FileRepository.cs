using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.SERVICE
{
    public class FileRepository
    {
        private readonly string _uploadPath;
        private readonly IWebHostEnvironment _environment;
        private readonly string _basePath;

        public FileRepository(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _uploadPath = configuration.GetValue<string>("FileUpload:UploadPath") ?? "uploads";

            // Handle null WebRootPath by using ContentRootPath + wwwroot
            _basePath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            // Ensure the base directory exists
            Directory.CreateDirectory(_basePath);
        }

        public async Task<FileUploadResultDto> SaveFileAsync(IFormFile file, string subFolder = "uploads/documents")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new FileUploadResultDto
                    {
                        Success = false,
                        ErrorMessage = "No file provided"
                    };
                }

                // Create upload directory if it doesn't exist
                var uploadDir = Path.Combine(_basePath, _uploadPath, subFolder);
                Directory.CreateDirectory(uploadDir);

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadDir, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path for database storage
                var relativePath = Path.Combine(_uploadPath, subFolder, uniqueFileName).Replace('\\', '/');

                return new FileUploadResultDto
                {
                    Success = true,
                    FileName = uniqueFileName,
                    FilePath = relativePath,
                    FileSize = file.Length,
                    FileExtension = fileExtension
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public FileValidationDto ValidateFile(IFormFile file, DocumentTypeResponseDto documentType)
        {
            var result = new FileValidationDto { IsValid = true };

            if (file == null || file.Length == 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "No file provided";
                return result;
            }

            // Check file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = documentType.AllowedExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                                  .Select(ext => ext.Trim().ToLower())
                                                                  .ToList();

            if (!allowedExtensions.Contains(fileExtension))
            {
                result.IsValid = false;
                result.ErrorMessage = $"File extension '{fileExtension}' is not allowed. Allowed extensions: {string.Join(", ", allowedExtensions)}";
                result.AllowedExtensions = allowedExtensions;
                return result;
            }

            // Check file size
            var maxSizeBytes = documentType.MaxFileSizeMB * 1024 * 1024;
            if (file.Length > maxSizeBytes)
            {
                result.IsValid = false;
                result.ErrorMessage = $"File size ({file.Length / (1024 * 1024):F2} MB) exceeds maximum allowed size ({documentType.MaxFileSizeMB} MB)";
                result.MaxFileSizeMB = documentType.MaxFileSizeMB;
                return result;
            }

            return result;
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath.Replace('/', '\\'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool FileExists(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath.Replace('/', '\\'));
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        }
    }
}