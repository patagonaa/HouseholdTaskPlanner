USE [HouseholdTaskPlanner]
GO
/****** Object:  Table [dbo].[RecurringTask]    Script Date: 30.08.2020 00:45:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecurringTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IntervalDays] [int] NOT NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduledTask]    Script Date: 30.08.2020 00:45:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduledTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RecurringTaskId] [int] NULL,
	[Date] [date] NOT NULL,
	[State] [int] NOT NULL,
	[AssignedUserId] [int] NULL,
	[Name] [nvarchar](255) NULL,
 CONSTRAINT [PK_ScheduledTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 30.08.2020 00:45:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ScheduledTask]  WITH CHECK ADD  CONSTRAINT [FK_ScheduledTask_RecurringTask] FOREIGN KEY([RecurringTaskId])
REFERENCES [dbo].[RecurringTask] ([Id])
GO
ALTER TABLE [dbo].[ScheduledTask] CHECK CONSTRAINT [FK_ScheduledTask_RecurringTask]
GO
ALTER TABLE [dbo].[ScheduledTask]  WITH CHECK ADD  CONSTRAINT [FK_ScheduledTask_User] FOREIGN KEY([AssignedUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ScheduledTask] CHECK CONSTRAINT [FK_ScheduledTask_User]
GO
