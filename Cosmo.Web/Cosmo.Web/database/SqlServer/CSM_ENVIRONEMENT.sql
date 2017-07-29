
/****** Object:  Table [dbo].[CSM_ENVIRONEMENT]    Script Date: 2/9/2015 5:50:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO



CREATE TABLE [dbo].[CSM_ENVIRONEMENT](
	[ENV_ID] [int] IDENTITY(1,1) NOT NULL,
	[ENV_NAME] [varchar](100) NOT NULL,
	[ENV_IS_MONITOR] [bit] NULL,
	[ENV_IS_NOTIFY] [bit] NULL CONSTRAINT [DF_CSM_ENVIRONEMENT_ENV_IS_NOTIFY]  DEFAULT ((1)),
	[ENV_CREATED_BY] [varchar](100) NULL,
	[ENV_CREATED_DATE] [datetime] NULL,
	[ENV_UPDATED_BY] [varchar](100) NULL,
	[ENV_UPDATED_DATE] [datetime] NULL,
	[ENV_COMMENTS] [varchar](2000) NULL,
	[ENV_MAIL_FREQ] [int] NULL,
	[ENV_IS_CONSLTD_MAIL] [bit] NULL,
	[ENV_ISACTIVE] [bit] NULL,
	[ENV_SORTORDER] int NULL,
 CONSTRAINT [PK_CSM_ENVIRONEMENT] PRIMARY KEY CLUSTERED 
(
	[ENV_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

