using WfDoc.Api.Common;

namespace WfDoc.Api.Domain;

public record Admin
(
    int Id,
    string Username,
    string PasswordHash,
    AccessLevel AccessLevel,
    DateTime CreatedAt,
    int CreatedByAdminId
);

public record DocumentType
(
    int Id,
    string Name,
    DateTime CreatedAt,
    int CreatedByAdminId
);

public record Workflow
(
    int Id,
    string Name,
    WorkflowType Type,
    DateTime CreatedAt,
    int CreatedByAdminId
);

public record WorkflowAssignment
(
    int WorkflowId,
    int AdminId
);

public record Document
(
    int Id,
    string Name,
    int DocumentTypeId,
    string FilePath,
    int WorkflowId,
    DocumentStatus Status,
    DateTime CreatedAt,
    int CreatedByAdminId
);

public record DocumentAction
(
    int Id,
    int DocumentId,
    int AdminId,
    bool Approved,
    string? Comment,
    DateTime ActionAt
);

