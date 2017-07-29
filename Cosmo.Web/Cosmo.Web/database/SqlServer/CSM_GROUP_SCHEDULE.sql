/****** Object:  Table [dbo].[CSM_GROUP_SCHEDULE]    Script Date: 10/04/2015 14:13:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CSM_GROUP_SCHEDULE](
	[GROUP_SCH_ID] [int] IDENTITY(1,1) NOT NULL,
	[GROUP_ID] [int] NOT NULL,
	[GROUP_SCH_TIME] [datetime] NULL,
	[GROUP_SCH_ACTION] [nvarchar](50) NULL,
	[GROUP_SCH_STATUS] [nvarchar](50) NULL,
	[GROUP_SCH_COMPLETED_TIME] [datetime] NULL,
	[GROUP_SCH_COMMENTS] [nvarchar](max) NULL,
	[GROUP_SCH_CREATED_BY] [nchar](20) NULL,
	[GROUP_SCH_CREATED_DATETIME] [datetime] NULL,
	[GROUP_SCH_UPDATED_BY] [nchar](20) NULL,
	[GROUP_SCH_UPDATED_DATETIME] [datetime] NULL,
	[GROUP_SCH_ONDEMAND] [bit] NULL,
	[GROUP_SCH_RESULT] [varchar](50) NULL,
	[GROUP_SCH_REQUESTSOURCE] [varchar](50) NULL,
	[GROUP_SCH_ISACTIVE] bit default 'True',
	[GROUP_SCH_DUMMY1] varchar(1000),
	[GROUP_SCH_DUMMY2] varchar(1000),
	[GROUP_SCH_DUMMY3] varchar(1000),
	[GROUP_SCH_DUMMY4] varchar(1000),
 CONSTRAINT [PK_CSM_GROUP_SCHEDULE] PRIMARY KEY CLUSTERED 
(
	[GROUP_SCH_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'time that group to schedule for action' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CSM_GROUP_SCHEDULE', @level2type=N'COLUMN',@level2name=N'GROUP_SCH_TIME'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'start, stop, restart' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CSM_GROUP_SCHEDULE', @level2type=N'COLUMN',@level2name=N'GROUP_SCH_ACTION'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Action status, O-Open, P-Provessing, C-Completed, F-Failed' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CSM_GROUP_SCHEDULE', @level2type=N'COLUMN',@level2name=N'GROUP_SCH_STATUS'
GO

ALTER TABLE [dbo].[CSM_GROUP_SCHEDULE] ADD  DEFAULT ('false') FOR [GROUP_SCH_ONDEMAND]
GO


