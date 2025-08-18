-- Database: WfDocDb

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo') EXEC('CREATE SCHEMA dbo');

IF OBJECT_ID('dbo.Admins', 'U') IS NOT NULL DROP TABLE dbo.Admins;
IF OBJECT_ID('dbo.DocumentTypes', 'U') IS NOT NULL DROP TABLE dbo.DocumentTypes;
IF OBJECT_ID('dbo.Workflows', 'U') IS NOT NULL DROP TABLE dbo.Workflows;
IF OBJECT_ID('dbo.WorkflowAdmins', 'U') IS NOT NULL DROP TABLE dbo.WorkflowAdmins;
IF OBJECT_ID('dbo.Documents', 'U') IS NOT NULL DROP TABLE dbo.Documents;
IF OBJECT_ID('dbo.DocumentActions', 'U') IS NOT NULL DROP TABLE dbo.DocumentActions;

CREATE TABLE dbo.Admins (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    AccessLevel INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByAdminId INT NULL
);

CREATE TABLE dbo.DocumentTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByAdminId INT NOT NULL
);

CREATE TABLE dbo.Workflows (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Type INT NOT NULL, -- 0 Order, 1 Pool
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByAdminId INT NOT NULL
);

CREATE TABLE dbo.WorkflowAdmins (
    WorkflowId INT NOT NULL,
    AdminId INT NOT NULL,
    [Order] INT NULL, -- used only for Order workflows; null for Pool
    CONSTRAINT PK_WorkflowAdmins PRIMARY KEY (WorkflowId, AdminId)
);

CREATE TABLE dbo.Documents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    DocumentTypeId INT NOT NULL,
    FilePath NVARCHAR(400) NOT NULL,
    WorkflowId INT NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- 0 Pending, 1 Approved, 2 Rejected
    CurrentOrder INT NULL, -- for Order workflows, index of current reviewer (starts at 0)
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByAdminId INT NOT NULL
);

CREATE TABLE dbo.DocumentActions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocumentId INT NOT NULL,
    AdminId INT NOT NULL,
    Approved BIT NOT NULL,
    Comment NVARCHAR(500) NULL,
    ActionAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

