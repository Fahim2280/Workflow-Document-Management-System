using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.Repository
{
    public class TaskAssignmentRepository
    {
        private readonly string _connectionString;

        public TaskAssignmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<(List<AdminTaskDto> tasks, AdminDashboardSummaryDto summary)> GetAdminDashboardTasksAsync(int adminId)
        {
            var tasks = new List<AdminTaskDto>();
            var summary = new AdminDashboardSummaryDto();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAdminDashboardTasks", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@AdminId", adminId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            // Read tasks
            while (await reader.ReadAsync())
            {
                tasks.Add(new AdminTaskDto
                {
                    DocumentId = reader.GetInt32("DocumentId"),
                    DocumentName = reader.GetString("DocumentName"),
                    FileName = reader.GetString("FileName"),
                    FileSize = reader.GetInt64("FileSize"),
                    CurrentStatus = reader.GetString("CurrentStatus"),
                    UploadedDate = reader.GetDateTime("UploadedDate"),
                    DocumentTypeName = reader.GetString("DocumentTypeName"),
                    WorkflowId = reader.GetInt32("WorkflowId"),
                    WorkflowName = reader.GetString("WorkflowName"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    UploadedByUsername = reader.GetString("UploadedByUsername"),
                    AssignmentId = reader.GetInt32("AssignmentId"),
                    AssignedDate = reader.GetDateTime("AssignedDate"),
                    StepOrder = reader.GetInt32("StepOrder"),
                    LastComments = reader.IsDBNull("LastComments") ? null : reader.GetString("LastComments"),
                    NextAdminUsername = reader.IsDBNull("NextAdminUsername") ? null : reader.GetString("NextAdminUsername"),
                    TotalSteps = reader.GetInt32("TotalSteps")
                });
            }

            // Read summary
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                summary = new AdminDashboardSummaryDto
                {
                    TotalAssigned = reader.GetInt32("TotalAssigned"),
                    PendingCount = reader.GetInt32("PendingCount"),
                    UnderReviewCount = reader.GetInt32("UnderReviewCount"),
                    OverdueCount = reader.GetInt32("OverdueCount")
                };
            }

            return (tasks, summary);
        }

        public async Task<DocumentActionResultDto> ProcessDocumentActionAsync(ProcessDocumentActionDto dto, int adminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ProcessDocumentAction", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@DocumentId", dto.DocumentId);
            command.Parameters.AddWithValue("@AdminId", adminId);
            command.Parameters.AddWithValue("@Action", dto.Action);
            command.Parameters.AddWithValue("@Comments", dto.Comments ?? (object)DBNull.Value);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var result = new DocumentActionResultDto { Success = false };

            if (await reader.ReadAsync())
            {
                var status = reader.GetString("Status");
                result.Success = status.StartsWith("Success");
                result.Status = status;
                result.Message = status;

                // Determine workflow completion and next actions
                if (result.Success)
                {
                    result.IsWorkflowComplete = status.Contains("completed") || status.Contains("rejected");

                    if (status.Contains("next step"))
                    {
                        result.NextAction = "Document moved to next reviewer";
                    }
                    else if (status.Contains("completed"))
                    {
                        result.NextAction = "Workflow completed successfully";
                    }
                    else if (status.Contains("rejected"))
                    {
                        result.NextAction = "Document rejected - workflow ended";
                    }
                }
            }

            return result;
        }

        public async Task<bool> ReassignDocumentAsync(ReassignDocumentDto dto, int reassignedByAdminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ReassignDocument", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@DocumentId", dto.DocumentId);
            command.Parameters.AddWithValue("@ReassignedByAdminId", reassignedByAdminId);
            command.Parameters.AddWithValue("@Comments", dto.Comments ?? (object)DBNull.Value);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetString("Status").StartsWith("Success");
            }
            return false;
        }

        public async Task<DocumentAssignmentHistoryDto> GetDocumentAssignmentHistoryAsync(int documentId)
        {
            var history = new DocumentAssignmentHistoryDto();
            var assignments = new List<DocumentAssignmentDto>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetDocumentAssignmentHistory", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DocumentId", documentId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                assignments.Add(new DocumentAssignmentDto
                {
                    AssignmentId = reader.GetInt32("AssignmentId"),
                    DocumentId = documentId,
                    AdminId = reader.GetInt32("AdminId"),
                    AdminName = reader.GetString("AdminName"),
                    AssignedDate = reader.GetDateTime("AssignedDate"),
                    StepOrder = reader.GetInt32("StepOrder"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    CompletedDate = reader.IsDBNull("CompletedDate") ? null : reader.GetDateTime("CompletedDate"),
                    CompletionType = reader.IsDBNull("CompletionType") ? null : reader.GetString("CompletionType"),
                    Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }

            history.Assignments = assignments;
            history.TotalAssignments = assignments.Count;
            history.CompletedAssignments = assignments.Count(a => a.IsCompleted);
            history.PendingAssignments = assignments.Count(a => !a.IsCompleted && a.IsActive);

            if (assignments.Any())
            {
                var maxStep = assignments.Where(a => a.IsCompleted).DefaultIfEmpty().Max(a => a?.StepOrder ?? -1);
                history.CurrentStepOrder = maxStep + 1;
                history.TotalSteps = assignments.Select(a => a.StepOrder).DefaultIfEmpty().Max() + 1;
            }

            return history;
        }
    }
}
