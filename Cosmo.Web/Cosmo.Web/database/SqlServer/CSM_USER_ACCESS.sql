﻿IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CSM_USER_ACCESS]') AND type in (N'U'))
DROP TABLE [dbo].[CSM_USER_ACCESS]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CSM_USER_ACCESS](
	[ACCESS_ID] [int] IDENTITY(1,1) NOT NULL,
	[ACCESS_CODE] [varchar](max) NOT NULL,
	[ACCESS_FIRSTNAME] [varchar](100) NULL,
	[ACCESS_LASTTNAME] [varchar](100) NULL,
	[ACCESS_EMAIL] [varchar](100) NULL,
	[ACCESS_MOBILE] [varchar](100) NULL,
	[DATE_CREATED] [datetime] NULL,
 CONSTRAINT [PK_CSM_USER_ACCESS] PRIMARY KEY CLUSTERED 
(
	[ACCESS_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
