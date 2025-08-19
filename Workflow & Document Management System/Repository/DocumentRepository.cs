using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.Repository
{

    public class DocumentRepository
    {
        private readonly string _connectionString;

        public DocumentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateDocumentAsync(CreateDocumentDto dto, string fileName, string filePath, long fileSize, string fileExtension, int uploadedByAdminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateDocument", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@DocumentName", dto.DocumentName);
            command.Parameters.AddWithValue("@DocumentTypeId", dto.DocumentTypeId);
            command.Parameters.AddWithValue("@WorkflowId", dto.WorkflowId);
            //command.Parameters.AddWithValue("@File", dto.File);
            command.Parameters.AddWithValue("@FileName", fileName);
            command.Parameters.AddWithValue("@FilePath", filePath);
            command.Parameters.AddWithValue("@FileSize", fileSize);
            command.Parameters.AddWithValue("@FileExtension", fileExtension);
            command.Parameters.AddWithValue("@UploadedByAdminId", uploadedByAdminId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32("NewDocumentId");
            }
            return 0;
        }

        public async Task<List<DocumentResponseDto>> GetAllDocumentsAsync()
        {
            var documents = new List<DocumentResponseDto>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllDocuments", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                documents.Add(new DocumentResponseDto
                {
                    DocumentId = reader.GetInt32("DocumentId"),
                    DocumentName = reader.GetString("DocumentName"),
                    FileName = reader.GetString("FileName"),
                    FileSize = reader.GetInt64("FileSize"),
                    FileExtension = reader.GetString("FileExtension"),
                    CurrentStatus = reader.GetString("CurrentStatus"),
                    UploadedDate = reader.GetDateTime("UploadedDate"),
                    DocumentTypeName = reader.GetString("DocumentTypeName"),
                    WorkflowName = reader.GetString("WorkflowName"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    UploadedByUsername = reader.GetString("UploadedByUsername"),
                    FilePath = reader.GetString("FilePath")
                });
            }
            return documents;
        }

        public async Task<DocumentDetailDto> GetDocumentByIdAsync(int documentId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetDocumentById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DocumentId", documentId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            DocumentDetailDto document = null;

            // Read document details
            if (await reader.ReadAsync())
            {
                document = new DocumentDetailDto
                {
                    DocumentId = reader.GetInt32("DocumentId"),
                    DocumentName = reader.GetString("DocumentName"),
                    FileName = reader.GetString("FileName"),
                    FilePath = reader.GetString("FilePath"),
                    FileSize = reader.GetInt64("FileSize"),
                    FileExtension = reader.GetString("FileExtension"),
                    CurrentStatus = reader.GetString("CurrentStatus"),
                    UploadedDate = reader.GetDateTime("UploadedDate"),
                    DocumentTypeId = reader.GetInt32("DocumentTypeId"),
                    DocumentTypeName = reader.GetString("DocumentTypeName"),
                    WorkflowId = reader.GetInt32("WorkflowId"),
                    WorkflowName = reader.GetString("WorkflowName"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    UploadedByAdminId = reader.GetInt32("UploadedByAdminId"),
                    UploadedByUsername = reader.GetString("UploadedByUsername")
                };
            }

            // Read document activities
            if (document != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    document.Activities.Add(new DocumentActivityDto
                    {
                        ActivityId = reader.GetInt32("ActivityId"),
                        ActivityType = reader.GetString("ActivityType"),
                        Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                        ActivityDate = reader.GetDateTime("ActivityDate"),
                        AdminName = reader.GetString("AdminName")
                    });
                }
            }

            return document;
        }

        public async Task<bool> AddDocumentActivityAsync(AddDocumentActivityDto dto, int adminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_AddDocumentActivity", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@DocumentId", dto.DocumentId);
            command.Parameters.AddWithValue("@AdminId", adminId);
            command.Parameters.AddWithValue("@ActivityType", dto.ActivityType);
            command.Parameters.AddWithValue("@Comments", dto.Comments ?? (object)DBNull.Value);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetString("Status") == "Success";
            }
            return false;
        }
    }
}
