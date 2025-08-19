using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.SERVICE
{
    public class EnhancedValidationService : ValidationService
    {
        public List<string> ValidateProcessDocumentAction(ProcessDocumentActionDto dto)
        {
            var errors = new List<string>();

            if (dto.DocumentId <= 0)
                errors.Add("Valid document ID is required");

            if (string.IsNullOrWhiteSpace(dto.Action))
                errors.Add("Action is required");

            var validActions = new[] { "Approve", "Reject", "Complete" };
            if (!validActions.Contains(dto.Action))
                errors.Add($"Invalid action. Valid actions: {string.Join(", ", validActions)}");

            if (dto.Action == "Reject" && string.IsNullOrWhiteSpace(dto.Comments))
                errors.Add("Comments are required when rejecting a document");

            if (dto.Comments?.Length > 1000)
                errors.Add("Comments cannot exceed 1000 characters");

            return errors;
        }

        public List<string> ValidateReassignDocument(ReassignDocumentDto dto)
        {
            var errors = new List<string>();

            if (dto.DocumentId <= 0)
                errors.Add("Valid document ID is required");

            if (string.IsNullOrWhiteSpace(dto.Comments))
                errors.Add("Comments are required for reassignment");

            if (dto.Comments?.Length > 1000)
                errors.Add("Comments cannot exceed 1000 characters");

            if (dto.Reason?.Length > 500)
                errors.Add("Reason cannot exceed 500 characters");

            return errors;
        }

        public List<string> ValidateBatchDocumentAction(BatchDocumentActionDto dto)
        {
            var errors = new List<string>();

            if (!dto.DocumentIds?.Any() == true)
                errors.Add("At least one document must be selected");

            if (dto.DocumentIds?.Count > 50)
                errors.Add("Cannot process more than 50 documents at once");

            if (string.IsNullOrWhiteSpace(dto.Action))
                errors.Add("Action is required");

            var validActions = new[] { "Approve", "Reject", "Reassign", "Complete" };
            if (!validActions.Contains(dto.Action))
                errors.Add($"Invalid action. Valid actions: {string.Join(", ", validActions)}");

            if (dto.Action == "Reject" && string.IsNullOrWhiteSpace(dto.Comments))
                errors.Add("Comments are required when rejecting documents");

            if (dto.Action == "Reassign")
            {
                if (string.IsNullOrWhiteSpace(dto.Comments))
                    errors.Add("Comments are required when reassigning documents");

                if (!dto.ReassignToWorkflowId.HasValue)
                    errors.Add("Target workflow is required when reassigning documents");
            }

            if (dto.Comments?.Length > 1000)
                errors.Add("Comments cannot exceed 1000 characters");

            return errors;
        }
    }
}
