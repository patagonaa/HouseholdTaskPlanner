ALTER TABLE dbo.ScheduledTask
	DROP CONSTRAINT FK_ScheduledTask_RecurringTask
GO
ALTER TABLE dbo.ScheduledTask ADD CONSTRAINT
	FK_ScheduledTask_RecurringTask FOREIGN KEY
	(
	RecurringTaskId
	) REFERENCES dbo.RecurringTask
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE
GO
ALTER TABLE dbo.[User] ADD
	TelegramUsername nvarchar(255) NULL,
	TelegramId int NULL
GO