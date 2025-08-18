-- Admins
IF OBJECT_ID('sp_Admin_GetByUsername', 'P') IS NOT NULL DROP PROCEDURE sp_Admin_GetByUsername;
GO
CREATE PROCEDURE sp_Admin_GetByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, PasswordHash, AccessLevel, CreatedAt, CreatedByAdminId FROM dbo.Admins WHERE Username = @Username;
END
GO

IF OBJECT_ID('sp_Admin_Create', 'P') IS NOT NULL DROP PROCEDURE sp_Admin_Create;
GO
CREATE PROCEDURE sp_Admin_Create
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(200),
    @AccessLevel INT,
    @CreatedByAdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Admins (Username, PasswordHash, AccessLevel, CreatedByAdminId)
    VALUES (@Username, @PasswordHash, @AccessLevel, @CreatedByAdminId);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

IF OBJECT_ID('sp_Admin_List', 'P') IS NOT NULL DROP PROCEDURE sp_Admin_List;
GO
CREATE PROCEDURE sp_Admin_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, PasswordHash, AccessLevel, CreatedAt, CreatedByAdminId FROM dbo.Admins ORDER BY Id;
END
GO

-- Document Types
IF OBJECT_ID('sp_DocumentType_Create', 'P') IS NOT NULL DROP PROCEDURE sp_DocumentType_Create;
GO
CREATE PROCEDURE sp_DocumentType_Create
    @Name NVARCHAR(100),
    @CreatedByAdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DocumentTypes (Name, CreatedByAdminId)
    VALUES (@Name, @CreatedByAdminId);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

IF OBJECT_ID('sp_DocumentType_Update', 'P') IS NOT NULL DROP PROCEDURE sp_DocumentType_Update;
GO
CREATE PROCEDURE sp_DocumentType_Update
    @Id INT,
    @Name NVARCHAR(100),
    @UpdatedByAdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.DocumentTypes SET Name = @Name WHERE Id = @Id;
END
GO

IF OBJECT_ID('sp_DocumentType_Delete', 'P') IS NOT NULL DROP PROCEDURE sp_DocumentType_Delete;
GO
CREATE PROCEDURE sp_DocumentType_Delete
    @Id INT,
    @DeletedByAdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.DocumentTypes WHERE Id = @Id;
END
GO

IF OBJECT_ID('sp_DocumentType_List', 'P') IS NOT NULL DROP PROCEDURE sp_DocumentType_List;
GO
CREATE PROCEDURE sp_DocumentType_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CreatedAt, CreatedByAdminId FROM dbo.DocumentTypes ORDER BY Name;
END
GO

-- Workflows
IF OBJECT_ID('sp_Workflow_Create', 'P') IS NOT NULL DROP PROCEDURE sp_Workflow_Create;
GO
CREATE PROCEDURE sp_Workflow_Create
    @Name NVARCHAR(200),
    @Type INT,
    @CreatedByAdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Workflows (Name, Type, CreatedByAdminId)
    VALUES (@Name, @Type, @CreatedByAdminId);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

IF OBJECT_ID('sp_Workflow_AssignAdmin', 'P') IS NOT NULL DROP PROCEDURE sp_Workflow_AssignAdmin;
GO
CREATE PROCEDURE sp_Workflow_AssignAdmin
    @WorkflowId INT,
    @AdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @wfType INT = (SELECT Type FROM dbo.Workflows WHERE Id = @WorkflowId);
    DECLARE @order INT = NULL;
    IF @wfType = 0 -- Order
    BEGIN
        SELECT @order = ISNULL(MAX([Order]), -1) + 1 FROM dbo.WorkflowAdmins WHERE WorkflowId = @WorkflowId;
    END
    INSERT INTO dbo.WorkflowAdmins (WorkflowId, AdminId, [Order]) VALUES (@WorkflowId, @AdminId, @order);
END
GO

IF OBJECT_ID('sp_Workflow_List', 'P') IS NOT NULL DROP PROCEDURE sp_Workflow_List;
GO
CREATE PROCEDURE sp_Workflow_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Type, CreatedAt, CreatedByAdminId FROM dbo.Workflows ORDER BY Id DESC;
END
GO

-- Documents
IF OBJECT_ID('sp_Document_Create', 'P') IS NOT NULL DROP PROCEDURE sp_Document_Create;
GO
CREATE PROCEDURE sp_Document_Create
    @Name NVARCHAR(200),
    @DocumentTypeId INT,
    @FilePath NVARCHAR(400),
    @WorkflowId INT,
    @CreatedByAdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @wfType INT = (SELECT Type FROM dbo.Workflows WHERE Id = @WorkflowId);
    DECLARE @currentOrder INT = CASE WHEN @wfType = 0 THEN 0 ELSE NULL END;
    INSERT INTO dbo.Documents (Name, DocumentTypeId, FilePath, WorkflowId, Status, CurrentOrder, CreatedByAdminId)
    VALUES (@Name, @DocumentTypeId, @FilePath, @WorkflowId, 0, @currentOrder, @CreatedByAdminId);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

IF OBJECT_ID('sp_Document_MyTasks', 'P') IS NOT NULL DROP PROCEDURE sp_Document_MyTasks;
GO
CREATE PROCEDURE sp_Document_MyTasks
    @AdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.Id, d.Name, d.DocumentTypeId, d.FilePath, d.WorkflowId, d.Status, d.CreatedAt, d.CreatedByAdminId, d.CurrentOrder
    FROM dbo.Documents d
    JOIN dbo.Workflows w ON w.Id = d.WorkflowId
    JOIN dbo.WorkflowAdmins wa ON wa.WorkflowId = w.Id
    WHERE d.Status = 0 -- Pending
      AND (
        (w.Type = 1 AND wa.AdminId = @AdminId) -- Pool, everyone assigned sees
        OR
        (w.Type = 0 AND wa.AdminId = @AdminId AND wa.[Order] = d.CurrentOrder) -- Order, only current
      );
END
GO

IF OBJECT_ID('sp_Document_Approve', 'P') IS NOT NULL DROP PROCEDURE sp_Document_Approve;
GO
CREATE PROCEDURE sp_Document_Approve
    @DocumentId INT,
    @AdminId INT,
    @Comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @wfId INT = (SELECT WorkflowId FROM dbo.Documents WHERE Id = @DocumentId);
    DECLARE @wfType INT = (SELECT Type FROM dbo.Workflows WHERE Id = @wfId);

    INSERT INTO dbo.DocumentActions (DocumentId, AdminId, Approved, Comment)
    VALUES (@DocumentId, @AdminId, 1, @Comment);

    IF @wfType = 1 -- Pool
    BEGIN
        -- If any reject exists, status would have been set to Rejected already. Approvals accumulate.
        DECLARE @assignedCount INT = (SELECT COUNT(*) FROM dbo.WorkflowAdmins WHERE WorkflowId = @wfId);
        DECLARE @approvedCount INT = (SELECT COUNT(*) FROM dbo.DocumentActions WHERE DocumentId = @DocumentId AND Approved = 1);
        IF @approvedCount >= @assignedCount
        BEGIN
            UPDATE dbo.Documents SET Status = 1 WHERE Id = @DocumentId; -- Approved
        END
        RETURN;
    END

    -- Order workflow
    DECLARE @current INT = (SELECT CurrentOrder FROM dbo.Documents WHERE Id = @DocumentId);
    DECLARE @maxOrder INT = (SELECT MAX([Order]) FROM dbo.WorkflowAdmins WHERE WorkflowId = @wfId);
    IF @current >= @maxOrder
    BEGIN
        UPDATE dbo.Documents SET Status = 1, CurrentOrder = NULL WHERE Id = @DocumentId; -- Approved
    END
    ELSE
    BEGIN
        UPDATE dbo.Documents SET CurrentOrder = @current + 1 WHERE Id = @DocumentId; -- next reviewer
    END
END
GO

IF OBJECT_ID('sp_Document_Reject', 'P') IS NOT NULL DROP PROCEDURE sp_Document_Reject;
GO
CREATE PROCEDURE sp_Document_Reject
    @DocumentId INT,
    @AdminId INT,
    @Comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DocumentActions (DocumentId, AdminId, Approved, Comment)
    VALUES (@DocumentId, @AdminId, 0, @Comment);
    UPDATE dbo.Documents SET Status = 2 WHERE Id = @DocumentId; -- Rejected immediately
END
GO

IF OBJECT_ID('sp_Document_Reassign', 'P') IS NOT NULL DROP PROCEDURE sp_Document_Reassign;
GO
CREATE PROCEDURE sp_Document_Reassign
    @DocumentId INT,
    @AdminId INT
AS
BEGIN
    SET NOCOUNT ON;
    -- When reassigned, reset status to Pending and start from beginning if Order
    DECLARE @wfId INT = (SELECT WorkflowId FROM dbo.Documents WHERE Id = @DocumentId);
    DECLARE @wfType INT = (SELECT Type FROM dbo.Workflows WHERE Id = @wfId);
    DECLARE @currentOrder INT = CASE WHEN @wfType = 0 THEN 0 ELSE NULL END;
    UPDATE dbo.Documents SET Status = 0, CurrentOrder = @currentOrder WHERE Id = @DocumentId;
    -- We preserve action history for audit; new approvals will accumulate again
END
GO

