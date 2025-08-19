using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.Repository
{
    public class WorkflowRepository
    {
        private readonly string _connectionString;

        public WorkflowRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateWorkflowAsync(CreateWorkflowDto dto, int createdByAdminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateWorkflow", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@WorkflowName", dto.WorkflowName);
            command.Parameters.AddWithValue("@WorkflowType", dto.WorkflowType);
            command.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatedByAdminId", createdByAdminId);
            command.Parameters.AddWithValue("@AssignedAdminIds", string.Join(",", dto.AssignedAdminIds));

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32("NewWorkflowId");
            }
            return 0;
        }

        public async Task<List<WorkflowResponseDto>> GetAllWorkflowsAsync()
        {
            var workflows = new List<WorkflowResponseDto>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllWorkflows", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                workflows.Add(new WorkflowResponseDto
                {
                    WorkflowId = reader.GetInt32("WorkflowId"),
                    WorkflowName = reader.GetString("WorkflowName"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    CreatedByAdminId = reader.GetInt32("CreatedByAdminId"),
                    CreatedByUsername = reader.GetString("CreatedByUsername"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    IsActive = reader.GetBoolean("IsActive"),
                    AssignedAdmins = reader.IsDBNull("AssignedAdmins") ? "" : reader.GetString("AssignedAdmins")
                });
            }
            return workflows;
        }

        public async Task<WorkflowDetailDto> GetWorkflowByIdAsync(int workflowId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetWorkflowById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@WorkflowId", workflowId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            WorkflowDetailDto workflow = null;

            // Read workflow details
            if (await reader.ReadAsync())
            {
                workflow = new WorkflowDetailDto
                {
                    WorkflowId = reader.GetInt32("WorkflowId"),
                    WorkflowName = reader.GetString("WorkflowName"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    CreatedByAdminId = reader.GetInt32("CreatedByAdminId"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }

            // Read assigned admins
            if (workflow != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    workflow.AssignedAdmins.Add(new AssignedAdminDto
                    {
                        AdminId = reader.GetInt32("AdminId"),
                        Username = reader.GetString("Username"),
                        Email = reader.GetString("Email"),
                        AssignedDate = reader.GetDateTime("AssignedDate")
                    });
                }
            }

            return workflow;
        }
    }
   
}
