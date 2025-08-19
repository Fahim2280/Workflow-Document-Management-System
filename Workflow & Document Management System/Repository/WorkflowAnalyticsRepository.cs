using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.Repository
{
    
    public class WorkflowAnalyticsRepository
    {
        private readonly string _connectionString;

        public WorkflowAnalyticsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<WorkflowAnalyticsDto>> GetWorkflowAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var analytics = new List<WorkflowAnalyticsDto>();

            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT 
                    w.WorkflowId,
                    w.WorkflowName,
                    w.WorkflowType,
                    COUNT(d.DocumentId) as TotalDocuments,
                    SUM(CASE WHEN d.CurrentStatus = 'Completed' THEN 1 ELSE 0 END) as CompletedDocuments,
                    SUM(CASE WHEN d.CurrentStatus IN ('Pending', 'Under Review') THEN 1 ELSE 0 END) as PendingDocuments,
                    SUM(CASE WHEN d.CurrentStatus = 'Rejected' THEN 1 ELSE 0 END) as RejectedDocuments,
                    AVG(CASE 
                        WHEN d.CurrentStatus = 'Completed' 
                        THEN DATEDIFF(hour, d.UploadedDate, 
                            (SELECT MAX(da.CompletedDate) FROM DocumentActivities da 
                             WHERE da.DocumentId = d.DocumentId AND da.ActivityType IN ('Approve', 'Complete')))
                        ELSE NULL 
                    END) as AverageCompletionTimeHours,
                    COUNT(CASE 
                        WHEN d.CurrentStatus IN ('Pending', 'Under Review') 
                            AND DATEDIFF(day, d.UploadedDate, GETDATE()) > 3 
                        THEN 1 
                    END) as OverdueDocuments
                FROM Workflows w
                LEFT JOIN Documents d ON w.WorkflowId = d.WorkflowId AND d.IsActive = 1";

            var parameters = new List<SqlParameter>();

            if (fromDate.HasValue || toDate.HasValue)
            {
                var dateConditions = new List<string>();
                if (fromDate.HasValue)
                {
                    dateConditions.Add("d.UploadedDate >= @FromDate");
                    parameters.Add(new SqlParameter("@FromDate", fromDate.Value));
                }
                if (toDate.HasValue)
                {
                    dateConditions.Add("d.UploadedDate <= @ToDate");
                    parameters.Add(new SqlParameter("@ToDate", toDate.Value));
                }

                if (dateConditions.Any())
                {
                    query += " AND (" + string.Join(" AND ", dateConditions) + ")";
                }
            }

            query += @"
                WHERE w.IsActive = 1
                GROUP BY w.WorkflowId, w.WorkflowName, w.WorkflowType
                ORDER BY w.WorkflowName";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                analytics.Add(new WorkflowAnalyticsDto
                {
                    WorkflowId = reader.GetInt32("WorkflowId"),
                    WorkflowName = reader.GetString("WorkflowName"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    TotalDocuments = reader.GetInt32("TotalDocuments"),
                    CompletedDocuments = reader.GetInt32("CompletedDocuments"),
                    PendingDocuments = reader.GetInt32("PendingDocuments"),
                    RejectedDocuments = reader.GetInt32("RejectedDocuments"),
                    AverageCompletionTimeHours = reader.IsDBNull("AverageCompletionTimeHours") ? 0 : reader.GetDouble("AverageCompletionTimeHours"),
                    OverdueDocuments = reader.GetInt32("OverdueDocuments")
                });
            }

            return analytics;
        }

        public async Task<List<AdminWorkloadDto>> GetAdminWorkloadAnalyticsAsync(int? workflowId = null)
        {
            var workloads = new List<AdminWorkloadDto>();

            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT 
                    a.AdminId,
                    a.Username as AdminName,
                    COUNT(da.AssignmentId) as AssignedCount,
                    SUM(CASE WHEN da.IsCompleted = 1 THEN 1 ELSE 0 END) as CompletedCount,
                    SUM(CASE WHEN da.IsCompleted = 0 AND da.IsActive = 1 THEN 1 ELSE 0 END) as PendingCount,
                    COUNT(CASE 
                        WHEN da.IsCompleted = 0 AND da.IsActive = 1 
                            AND DATEDIFF(day, da.AssignedDate, GETDATE()) > 3 
                        THEN 1 
                    END) as OverdueCount,
                    AVG(CASE 
                        WHEN da.IsCompleted = 1 
                        THEN DATEDIFF(hour, da.AssignedDate, da.CompletedDate) 
                        ELSE NULL 
                    END) as AverageCompletionTimeHours
                FROM Admins a
                LEFT JOIN DocumentAssignments da ON a.AdminId = da.AdminId";

            if (workflowId.HasValue)
            {
                query += @" LEFT JOIN Documents d ON da.DocumentId = d.DocumentId 
                           WHERE (da.DocumentId IS NULL OR d.WorkflowId = @WorkflowId) AND a.IsActive = 1";

                using var command = new SqlCommand(query + " GROUP BY a.AdminId, a.Username ORDER BY a.Username", connection);
                command.Parameters.AddWithValue("@WorkflowId", workflowId.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    workloads.Add(CreateAdminWorkloadDto(reader));
                }
            }
            else
            {
                query += " WHERE a.IsActive = 1 GROUP BY a.AdminId, a.Username ORDER BY a.Username";

                using var command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    workloads.Add(CreateAdminWorkloadDto(reader));
                }
            }

            // Calculate workload percentages
            var totalAssignments = workloads.Sum(w => w.AssignedCount);
            foreach (var workload in workloads)
            {
                workload.WorkloadPercentage = totalAssignments > 0 ?
                    (double)workload.AssignedCount / totalAssignments * 100 : 0;
            }

            return workloads;
        }

        private AdminWorkloadDto CreateAdminWorkloadDto(SqlDataReader reader)
        {
            return new AdminWorkloadDto
            {
                AdminId = reader.GetInt32("AdminId"),
                AdminName = reader.GetString("AdminName"),
                AssignedCount = reader.GetInt32("AssignedCount"),
                CompletedCount = reader.GetInt32("CompletedCount"),
                PendingCount = reader.GetInt32("PendingCount"),
                OverdueCount = reader.GetInt32("OverdueCount"),
                AverageCompletionTimeHours = reader.IsDBNull("AverageCompletionTimeHours") ? 0 : reader.GetDouble("AverageCompletionTimeHours")
            };
        }
    }
}
