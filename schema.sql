CREATE DATABASE ElectroPiTaskManagerDb;
GO

USE ElectroPiTaskManagerDb;
GO

CREATE TABLE Projects (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Status NVARCHAR(20) NOT NULL,
    DueDate DATETIME2 NULL,
    ProjectId INT NOT NULL,
    CONSTRAINT FK_Tasks_Projects_ProjectId
        FOREIGN KEY (ProjectId) REFERENCES Projects(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_Tasks_ProjectId ON Tasks(ProjectId);
GO
