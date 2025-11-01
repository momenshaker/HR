-- Fix multiple cascade paths by enforcing NO ACTION on Invoices.SubscriptionId FK
-- Safe to run multiple times.

DECLARE @fkName sysname = 'FK_Invoices_Subscriptions_SubscriptionId';

IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = @fkName
      AND parent_object_id = OBJECT_ID('dbo.Invoices')
)
BEGIN
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_Invoices_Subscriptions_SubscriptionId];
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_Invoices_Subscriptions_SubscriptionId'
      AND parent_object_id = OBJECT_ID('dbo.Invoices')
)
BEGIN
    ALTER TABLE [dbo].[Invoices] WITH CHECK
    ADD CONSTRAINT [FK_Invoices_Subscriptions_SubscriptionId]
        FOREIGN KEY([SubscriptionId])
        REFERENCES [dbo].[Subscriptions]([Id])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION;

    ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Subscriptions_SubscriptionId];
END
GO

