
/****** Object:  Table [dbo].[CSM_ACKNOWLEDGE]    Script Date: 2/9/2015 5:47:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[CSM_ACKNOWLEDGE](
	[ACK_ID] [int] IDENTITY(1,1) NOT NULL,
	[ENV_ID] [int] NOT NULL,
	[CONFIG_ID] [int] NOT NULL,
	[ACK_ISACKNOWLEDGE] [bit] NULL,
	[ACK_ALERT] [varchar](50) NULL,
	[ACK_COMMENTS] [varchar](5000) NULL,
	[CREATED_BY] [varchar](50) NULL,
	[CREATED_DATE] [datetime] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


