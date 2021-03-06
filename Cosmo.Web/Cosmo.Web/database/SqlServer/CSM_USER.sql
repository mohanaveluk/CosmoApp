
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__CSM_USER__USER_I__123EB7A3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CSM_USER] DROP CONSTRAINT [DF__CSM_USER__USER_I__123EB7A3]
END

GO

/****** Object:  Table [dbo].[CSM_USER]    Script Date: 09/24/2015 11:09:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CSM_USER]') AND type in (N'U'))
DROP TABLE [dbo].[CSM_USER]
GO


/****** Object:  Table [dbo].[CSM_USER]    Script Date: 09/24/2015 11:09:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CSM_USER](
	[USER_ID] [int] IDENTITY(1,1) NOT NULL,
	[USER_LOGIN_ID] [varchar](200) NOT NULL,
	[USER_FIRST_NAME] [varchar](100) NULL,
	[USER_LAST_NAME] [varchar](100) NULL,
	[USER_EMAIL_ADDRESS] [varchar](200) NULL,
	[USER_ROLE] [varchar](100) NULL,
	[USER_PASSWORD] [varchar](max) NULL,
	[USER_IS_LDAP_USER] [bit] NULL,
	[USER_IS_ACTIVE] [bit] NULL,
	[USER_ATTEMPS] [int] NULL,
	[USER_ISLOCKED] [datetime] NULL,
	[USER_LOCKEDTIME] [int] NULL,
	[USER_CREATED_BY] [varchar](100) NULL,
	[USER_CREATED_DATE] [datetime] NULL,
	[USER_UPDATED_BY] [varchar](100) NULL,
	[USER_UPDATED_DATE] [datetime] NULL,
	[USER_COMMENTS] [varchar](2000) NULL,
	[USER_IS_DELETED] [bit] NULL DEFAULT ('false'),
 CONSTRAINT [PK_CSM_USER] PRIMARY KEY CLUSTERED 
(
	[USER_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

