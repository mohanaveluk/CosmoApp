/****** Object:  StoredProcedure [dbo].[CWT_InsertMailLog]    Script Date: 2/9/2015 6:16:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  procedure [dbo].[CWT_InsertMailLog]
(
	   @ENV_ID int
	  ,@CONFIG_ID int
      ,@EMTRAC_TO_ADDRESS varchar(200)
      ,@EMTRAC_CC_ADDRESS varchar(200)
      ,@EMTRAC_BCC_ADDRESS varchar(200)
      ,@EMTRAC_SUBJECT varchar(300)
      ,@EMTRAC_BODY varchar(5000)
      ,@EMTRAC_SEND_STATUS varchar(200)
      ,@EMTRAC_SEND_ERROR varchar(5000)
      ,@EMTRAC_CONTENT_TYPE varchar(200)
      ,@EMTRAC_CREATED_BY varchar(100)
      ,@EMTRAC_CREATED_DATE datetime
      ,@EMTRAC_COMMENTS varchar(2000)
)
As
Begin
	Insert into [CSM_EMAIL_TRACKING]
	(
		   [ENV_ID]
		  ,[Config_ID]
		  ,[EMTRAC_TO_ADDRESS]
		  ,[EMTRAC_CC_ADDRESS]
		  ,[EMTRAC_BCC_ADDRESS]
		  ,[EMTRAC_SUBJECT]
		  ,[EMTRAC_BODY]
		  ,[EMTRAC_SEND_STATUS]
		  ,[EMTRAC_SEND_ERROR]
		  ,[EMTRAC_CONTENT_TYPE]
		  ,[EMTRAC_CREATED_BY]
		  ,[EMTRAC_CREATED_DATE]
		  ,[EMTRAC_COMMENTS]
		  ,[EMTRAC_IS_ACTIVE]
	) values
	(
		@ENV_ID
	  ,@CONFIG_ID
	  ,@EMTRAC_TO_ADDRESS
      ,@EMTRAC_CC_ADDRESS
      ,@EMTRAC_BCC_ADDRESS
      ,@EMTRAC_SUBJECT
      ,@EMTRAC_BODY
      ,@EMTRAC_SEND_STATUS
      ,@EMTRAC_SEND_ERROR
      ,@EMTRAC_CONTENT_TYPE
      ,@EMTRAC_CREATED_BY
      ,@EMTRAC_CREATED_DATE
      ,@EMTRAC_COMMENTS
      ,'True'
	)
End