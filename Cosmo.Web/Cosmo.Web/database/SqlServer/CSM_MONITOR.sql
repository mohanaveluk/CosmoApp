
/****** Object:  Table [dbo].[CSM_MONITOR]    Script Date: 2/9/2015 5:50:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF OBJECT_ID('CSM_MONITOR') IS NOT NULL
DROP TABLE DBO.CSM_MONITOR
GO

CREATE TABLE [dbo].[CSM_MONITOR](
	[MON_ID] [int] IDENTITY(1,1) NOT NULL,
	[SCH_ID] [int] NOT NULL,
	[CONFIG_ID] [int] NOT NULL,
	[ENV_ID] [int] NULL,
	[MON_STATUS] [varchar](200) NULL,
	[MON_START_DATE_TIME] [varchar](200) NULL,
	[MON_END_DATE_TIME] [varchar](200) NULL,
	[MON_IS_ACTIVE] [bit] NOT NULL,
	[MON_CREATED_BY] [varchar](100) NULL,
	[MON_CREATED_DATE] [datetime] NULL,
	[MON_UPDATED_BY] [varchar](100) NULL,
	[MON_UPDATED_DATE] [datetime] NULL,
	[MON_COMMENTS] [varchar](2000) NULL,
	[MON_ISACKNOWLEDGE] [bit] NULL,
 CONSTRAINT [PK_CSM_MONITOR] PRIMARY KEY CLUSTERED 
(
	[MON_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

