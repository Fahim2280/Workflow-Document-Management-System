using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.Repository
{
    public class DocumentTypeRepository
    {
        private readonly string _connectionString;

        public DocumentTypeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateDocumentTypeAsync(CreateDocumentTypeDto dto, int createdByAdminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateDocumentType", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TypeName", dto.TypeName);
            command.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AllowedExtensions", dto.AllowedExtensions);
            command.Parameters.AddWithValue("@MaxFileSizeMB", dto.MaxFileSizeMB);
            command.Parameters.AddWithValue("@CreatedByAdminId", createdByAdminId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32("NewDocumentTypeId");
            }
            return 0;
        }

        public async Task<List<DocumentTypeResponseDto>> GetAllDocumentTypesAsync()
        {
            var documentTypes = new List<DocumentTypeResponseDto>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllDocumentTypes", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                documentTypes.Add(new DocumentTypeResponseDto
                {
                    DocumentTypeId = reader.GetInt32("DocumentTypeId"),
                    TypeName = reader.GetString("TypeName"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    AllowedExtensions = reader.GetString("AllowedExtensions"),
                    MaxFileSizeMB = reader.GetInt32("MaxFileSizeMB"),
                    CreatedByAdminId = reader.GetInt32("CreatedByAdminId"),
                    CreatedByUsername = reader.GetString("CreatedByUsername"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            return documentTypes;
        }

        public async Task<bool> UpdateDocumentTypeAsync(UpdateDocumentTypeDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpdateDocumentType", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@DocumentTypeId", dto.DocumentTypeId);
            command.Parameters.AddWithValue("@TypeName", dto.TypeName);
            command.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AllowedExtensions", dto.AllowedExtensions);
            command.Parameters.AddWithValue("@MaxFileSizeMB", dto.MaxFileSizeMB);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetString("Status") == "Success";
            }
            return false;
        }

        public async Task<bool> DeleteDocumentTypeAsync(int documentTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteDocumentType", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@DocumentTypeId", documentTypeId);

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
