-- Quick hotfix for missing AspNetUsers.EmployeeId column and index
-- Safe to run multiple times.

IF COL_LENGTH('dbo.AspNetUsers', 'EmployeeId') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
        ADD [EmployeeId] UNIQUEIDENTIFIER NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetUsers_EmployeeId' AND object_id = OBJECT_ID('dbo.AspNetUsers')
)
BEGIN
    CREATE UNIQUE INDEX [IX_AspNetUsers_EmployeeId]
        ON [dbo].[AspNetUsers]([EmployeeId])
        WHERE [EmployeeId] IS NOT NULL;
END
GO

