using System.Data;
using Dapper;
using WfDoc.Api.Domain;
using WfDoc.Api.Infrastructure;

namespace WfDoc.Api.Application.Masters;

public interface IMasterService
{
    Task<int> CreateDocumentTypeAsync(int adminId, string name);
    Task UpdateDocumentTypeAsync(int adminId, int id, string name);
    Task DeleteDocumentTypeAsync(int adminId, int id);
    Task<IEnumerable<DocumentType>> GetDocumentTypesAsync();
}

public sealed class MasterService : IMasterService
{
    private readonly ISqlConnectionFactory _connectionFactory;
    public MasterService(ISqlConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateDocumentTypeAsync(int adminId, string name)
    {
        await using var conn = _connectionFactory.Create();
        var id = await conn.ExecuteScalarAsync<int>("sp_DocumentType_Create", new { Name = name, CreatedByAdminId = adminId }, commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task UpdateDocumentTypeAsync(int adminId, int id, string name)
    {
        await using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync("sp_DocumentType_Update", new { Id = id, Name = name, UpdatedByAdminId = adminId }, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteDocumentTypeAsync(int adminId, int id)
    {
        await using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync("sp_DocumentType_Delete", new { Id = id, DeletedByAdminId = adminId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync()
    {
        await using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<DocumentType>("sp_DocumentType_List", commandType: CommandType.StoredProcedure);
    }
}

