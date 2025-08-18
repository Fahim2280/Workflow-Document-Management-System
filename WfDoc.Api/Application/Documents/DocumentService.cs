using System.Data;
using Dapper;
using WfDoc.Api.Common;
using WfDoc.Api.Domain;
using WfDoc.Api.Infrastructure;

namespace WfDoc.Api.Application.Documents;

public interface IDocumentService
{
    Task<int> CreateDocumentAsync(int adminId, string name, int documentTypeId, string filePath, int workflowId);
    Task<IEnumerable<Document>> GetMyTasksAsync(int adminId);
    Task ApproveAsync(int adminId, int documentId, string? comment);
    Task RejectAsync(int adminId, int documentId, string? comment);
    Task ReassignAsync(int adminId, int documentId);
}

public sealed class DocumentService : IDocumentService
{
    private readonly ISqlConnectionFactory _connectionFactory;
    public DocumentService(ISqlConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateDocumentAsync(int adminId, string name, int documentTypeId, string filePath, int workflowId)
    {
        await using var conn = _connectionFactory.Create();
        var id = await conn.ExecuteScalarAsync<int>(
            "sp_Document_Create",
            new { Name = name, DocumentTypeId = documentTypeId, FilePath = filePath, WorkflowId = workflowId, CreatedByAdminId = adminId },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task<IEnumerable<Document>> GetMyTasksAsync(int adminId)
    {
        await using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<Document>("sp_Document_MyTasks", new { AdminId = adminId }, commandType: CommandType.StoredProcedure);
    }

    public async Task ApproveAsync(int adminId, int documentId, string? comment)
    {
        await using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync("sp_Document_Approve", new { DocumentId = documentId, AdminId = adminId, Comment = comment }, commandType: CommandType.StoredProcedure);
    }

    public async Task RejectAsync(int adminId, int documentId, string? comment)
    {
        await using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync("sp_Document_Reject", new { DocumentId = documentId, AdminId = adminId, Comment = comment }, commandType: CommandType.StoredProcedure);
    }

    public async Task ReassignAsync(int adminId, int documentId)
    {
        await using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync("sp_Document_Reassign", new { DocumentId = documentId, AdminId = adminId }, commandType: CommandType.StoredProcedure);
    }
}

