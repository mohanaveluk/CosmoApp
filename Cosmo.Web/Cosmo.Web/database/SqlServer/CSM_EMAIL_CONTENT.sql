
/****** Object:  Table [dbo].[CSM_EMAIL_CONTENT]    Script Date: 2/9/2015 5:49:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[CSM_EMAIL_CONTENT](
	[EMCNT_ID] [int] IDENTITY(1,1) NOT NULL,
	[ENV_ID] [int] NULL,
	[EMCNT_NOTIFICATION_TYPE] [varchar](100) NOT NULL,
	[EMCNT_SUBJECT] [varchar](100) NOT NULL,
	[EMCNT_BODY] [varchar](100) NULL,
	[EMCNT_IS_ATTACHREPORT] [bit] NULL,
	[EMCNT_SIGNATURE] [varchar](100) NULL,
	[EMCNT_REPLYTO] [varchar](100) NULL,
	[EMCNT_IS_ACTIVE] [bit] NULL,
	[EMCNT_CREATED_BY] [datetime] NULL,
	[EMCNT_CREATED_DATE] [varchar](100) NULL,
	[EMCNT_UPDATED_BY] [datetime] NULL,
	[EMCNT_UPDATED_DATE] [varchar](2000) NULL,
	[EMCNT_COMMENTS] [varchar](2000) NULL,
 CONSTRAINT [PK_CSM_EMAIL_CONTENT] PRIMARY KEY CLUSTERED 
(
	[EMCNT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

