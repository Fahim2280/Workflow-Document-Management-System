using Workflow___Document_Management_System.DTOs;

namespace Workflow___Document_Management_System.SERVICE
{
    public class ValidationService
    {
        public List<string> ValidateCreateDocumentType(CreateDocumentTypeDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.TypeName))
                errors.Add("Type name is required");

            if (dto.TypeName?.Length > 100)
                errors.Add("Type name cannot exceed 100 characters");

            if (dto.Description?.Length > 500)
                errors.Add("Description cannot exceed 500 characters");

            if (dto.MaxFileSizeMB <= 0 || dto.MaxFileSizeMB > 1000)
                errors.Add("Max file size must be between 1 and 1000 MB");

            if (string.IsNullOrWhiteSpace(dto.AllowedExtensions))
                errors.Add("At least one allowed extension must be specified");

            return errors;
        }

        public List<string> ValidateCreateWorkflow(CreateWorkflowDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.WorkflowName))
                errors.Add("Workflow name is required");

            if (dto.WorkflowName?.Length > 100)
                errors.Add("Workflow name cannot exceed 100 characters");

            if (dto.WorkflowType != "Order" && dto.WorkflowType != "Pool")
                errors.Add("Workflow type must be 'Order' or 'Pool'");

            if (dto.AssignedAdminIds == null || !dto.AssignedAdminIds.Any())
                errors.Add("At least one admin must be assigned");

            if (dto.Description?.Length > 500)
                errors.Add("Description cannot exceed 500 characters");

            return errors;
        }

        public List<string> ValidateCreateDocument(CreateDocumentDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.DocumentName))
                errors.Add("Document name is required");

            if (dto.DocumentName?.Length > 255)
                errors.Add("Document name cannot exceed 255 characters");

            if (dto.DocumentTypeId <= 0)
                errors.Add("Valid document type must be selected");

            if (dto.WorkflowId <= 0)
                errors.Add("Valid workflow must be selected");

            if (dto.File == null || dto.File.Length == 0)
                errors.Add("File is required");

            return errors;
        }
    }
}
