-- Add default constraints for Employees.JobArchitecture* columns to avoid NULL insert failures
-- Safe to run multiple times.

DECLARE @tbl sysname = 'Employees';

-- Helper to add default constraint if missing
DECLARE @sql nvarchar(max);

IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Employees') AND c.name = 'JobArchitectureJobFamily'
)
BEGIN
    ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Employees_JobArchitectureJobFamily] DEFAULT ('') FOR [JobArchitectureJobFamily];
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Employees') AND c.name = 'JobArchitectureJobFunction'
)
BEGIN
    ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Employees_JobArchitectureJobFunction] DEFAULT ('') FOR [JobArchitectureJobFunction];
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Employees') AND c.name = 'JobArchitectureJobLevel'
)
BEGIN
    ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Employees_JobArchitectureJobLevel] DEFAULT ('') FOR [JobArchitectureJobLevel];
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Employees') AND c.name = 'JobArchitectureJobCode'
)
BEGIN
    ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Employees_JobArchitectureJobCode] DEFAULT ('') FOR [JobArchitectureJobCode];
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Employees') AND c.name = 'JobArchitectureCareerTrack'
)
BEGIN
    ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Employees_JobArchitectureCareerTrack] DEFAULT ('') FOR [JobArchitectureCareerTrack];
END
GO

