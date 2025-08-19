using Microsoft.AspNetCore.Mvc;

namespace Workflow___Document_Management_System.SERVICE
{
    public class FileService
    {
        private readonly FileRepository _fileRepository;
        private readonly DocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _environment;

        public FileService(
            FileRepository fileRepository,
            DocumentRepository documentRepository,
            IWebHostEnvironment environment)
        {
            _fileRepository = fileRepository;
            _documentRepository = documentRepository;
            _environment = environment;
        }

        public async Task<IActionResult> DownloadFileAsync(int documentId)
        {
            try
            {
                var document = await _documentRepository.GetDocumentByIdAsync(documentId);

                if (document == null)
                    return new NotFoundObjectResult("Document not found");

                var fullPath = Path.Combine(_environment.WebRootPath, document.FilePath.Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(fullPath))
                    return new NotFoundObjectResult("File not found on disk");

                var fileBytes = await File.ReadAllBytesAsync(fullPath);
                var contentType = GetContentType(document.FileExtension);

                return new FileContentResult(fileBytes, contentType)
                {
                    FileDownloadName = document.FileName
                };
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error downloading file: {ex.Message}");
            }
        }

        public async Task<IActionResult> ViewFileAsync(int documentId)
        {
            try
            {
                var document = await _documentRepository.GetDocumentByIdAsync(documentId);

                if (document == null)
                    return new NotFoundObjectResult("Document not found");

                var fullPath = Path.Combine(_environment.WebRootPath, document.FilePath.Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(fullPath))
                    return new NotFoundObjectResult("File not found on disk");

                var fileBytes = await File.ReadAllBytesAsync(fullPath);
                var contentType = GetContentType(document.FileExtension);

                return new FileContentResult(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error viewing file: {ex.Message}");
            }
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}
