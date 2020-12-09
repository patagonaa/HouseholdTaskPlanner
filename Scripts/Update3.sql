CREATE UNIQUE NONCLUSTERED INDEX [IX_Scheduled_RecurringTask_Unique] ON [dbo].[ScheduledTask]
(
	[RecurringTaskId] ASC
)
WHERE [RecurringTaskId] IS NOT NULL AND [State] = 10