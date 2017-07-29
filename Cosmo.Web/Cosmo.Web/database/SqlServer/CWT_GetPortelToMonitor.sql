SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetPortelToMonitor](@ID int)
As
Begin
	if @ID > 0
	Begin
		SELECT uc.[URL_ID]
			  ,uc.[ENV_ID]
			  ,env.[ENV_NAME]
			  ,uc.[URL_TYPE]
			  ,uc.[URL_ADDRESS]
			  ,uc.[URL_DISPLAYNAME]
			  ,uc.[URL_MATCHCONTENT]
			  ,uc.[URL_INTERVAL]
			  ,uc.[URL_USERNAME]
			  ,uc.[URL_PASSWORD]
			  ,uc.[URL_STATUS]
			  ,uc.[URL_CREATEDBY]
			  ,uc.[URL_CREATEDDATE]
			  ,uc.[URL_UPDATEDBY]
			  ,uc.[URL_UPDATEDDATE]
			  ,uc.[URL_COMMENTS]
			  ,uc.[URL_LASTJOBRUNTIME]
			  ,uc.[URL_NEXTJOBRUNTIME]
		  FROM [dbo].[CSM_URLCONFIGURATION] uc
		  join CSM_ENVIRONEMENT env 
		  ON env.ENV_ID = uc.ENV_ID
		  where [URL_ISACTIVE] = 1
		  and [URL_ID] = @ID
	End
	Else
	Begin
		SELECT uc.[URL_ID]
			  ,uc.[ENV_ID]
			  ,env.[ENV_NAME]
			  ,uc.[URL_TYPE]
			  ,uc.[URL_ADDRESS]
			  ,uc.[URL_DISPLAYNAME]
			  ,uc.[URL_MATCHCONTENT]
			  ,uc.[URL_INTERVAL]
			  ,uc.[URL_USERNAME]
			  ,uc.[URL_PASSWORD]
			  ,uc.[URL_STATUS]
			  ,uc.[URL_CREATEDBY]
			  ,uc.[URL_CREATEDDATE]
			  ,uc.[URL_UPDATEDBY]
			  ,uc.[URL_UPDATEDDATE]
			  ,uc.[URL_COMMENTS]
			  ,uc.[URL_LASTJOBRUNTIME]
			  ,uc.[URL_NEXTJOBRUNTIME]
		  FROM [dbo].[CSM_URLCONFIGURATION] uc
		  join CSM_ENVIRONEMENT env 
		  ON env.ENV_ID = uc.ENV_ID
		  where [URL_ISACTIVE] = 1
	End
End
GO


