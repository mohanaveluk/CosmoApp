
/****** Object:  Table [dbo].[CSM_EMAIL_USERLIST]    Script Date: 2/9/2015 5:49:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[CSM_EMAIL_USERLIST](
	[USRLST_ID] [int] IDENTITY(1,1) NOT NULL,
	[ENV_ID] [int] NULL,
	[USRLST_EMAIL_ADDRESS] [varchar](100) NOT NULL,
	[USRLST_TYPE] [varchar](100) NULL,
	[USRLST_IS_ACTIVE] [bit] NULL,
	[USRLST_CREATED_BY] [varchar](100) NULL,
	[USRLST_CREATED_DATE] [datetime] NULL,
	[USRLST_UPDATED_BY] [varchar](100) NULL,
	[USRLST_UPDATED_DATE] [datetime] NULL,
	[USRLST_COMMENTS] [varchar](2000) NULL,
	[USRLST_MESSAGETYPE] [varchar](1) NULL,
 CONSTRAINT [PK_CSM_EMAIL_USERLIST] PRIMARY KEY CLUSTERED 
(
	[USRLST_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


