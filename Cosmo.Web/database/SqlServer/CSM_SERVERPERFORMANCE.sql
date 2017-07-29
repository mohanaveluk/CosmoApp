SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CSM_SERVERPERFORMANCE](
	[PER_ID] [int] IDENTITY(1,1) NOT NULL,
	[ENVID] [int] NOT NULL,
	[CONFIGID] [int] NOT NULL,
	[PER_HOSTIP] [varchar](250) NOT NULL,
	[PER_CPU_USAGE] [float] NULL,
	[PER_AVAILABLEMEMORY] [float] NULL,
	[PER_TOTALMEMORY] [float] NULL,
	[PER_CREATED_BY] [varchar](100) NULL,
	[PER_CREATED_DATE] [datetime] NULL,
	[PER_COMMENTS] [varchar](max) NULL,
 CONSTRAINT [PK_CSM_SERVERPERFORMANCE] PRIMARY KEY CLUSTERED 
(
	[PER_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CSM_SERVERPERFORMANCE] ADD  DEFAULT (getdate()) FOR [PER_CREATED_DATE]
GO


