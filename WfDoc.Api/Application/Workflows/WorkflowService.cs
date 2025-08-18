using System.Data;
using Dapper;
using WfDoc.Api.Common;
using WfDoc.Api.Domain;
using WfDoc.Api.Infrastructure;

namespace WfDoc.Api.Application.Workflows;

public interface IWorkflowService
{
    Task<int> CreateWorkflowAsync(int adminId, string name, WorkflowType type, IEnumerable<int> assignedAdminIds);
    Task<IEnumerable<Workflow>> GetWorkflowsAsync();
}

public sealed class WorkflowService : IWorkflowService
{
    private readonly ISqlConnectionFactory _connectionFactory;
    public WorkflowService(ISqlConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateWorkflowAsync(int adminId, string name, WorkflowType type, IEnumerable<int> assignedAdminIds)
    {
        await using var conn = _connectionFactory.Create();
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();

        try
        {
            var workflowId = await conn.ExecuteScalarAsync<int>(
                "sp_Workflow_Create",
                new { Name = name, Type = (int)type, CreatedByAdminId = adminId },
                commandType: CommandType.StoredProcedure, transaction: tx);

            foreach (var aid in assignedAdminIds)
            {
                await conn.ExecuteAsync(
                    "sp_Workflow_AssignAdmin",
                    new { WorkflowId = workflowId, AdminId = aid },
                    commandType: CommandType.StoredProcedure, transaction: tx);
            }

            tx.Commit();
            return workflowId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<Workflow>> GetWorkflowsAsync()
    {
        await using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<Workflow>("sp_Workflow_List", commandType: CommandType.StoredProcedure);
    }
}

