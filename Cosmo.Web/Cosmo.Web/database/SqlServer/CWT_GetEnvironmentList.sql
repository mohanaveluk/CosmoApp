/****** Object:  StoredProcedure [dbo].[CWT_GetEnvironmentList]    Script Date: 2/9/2015 6:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetEnvironmentList]
(
      @ENV_ID int
)
as
Begin
	if(@ENV_ID=0 or @ENV_ID='0' or @ENV_ID is null)
	Begin
		select [ENV_ID] 
		,[ENV_NAME]
      --,[ENV_HOST_IP_ADDRESS]
      --,[ENV_LOCATION]
      ,[ENV_IS_MONITOR]
      ,[ENV_IS_NOTIFY]
      ,[ENV_CREATED_BY]
      ,[ENV_CREATED_DATE]
      ,[ENV_UPDATED_BY]
      ,[ENV_UPDATED_DATE]
      ,[ENV_COMMENTS]
      ,[ENV_MAIL_FREQ]
      ,[ENV_IS_CONSLTD_MAIL]		
      ,[ENV_SORTORDER]
      from [CSM_ENVIRONEMENT] where ENV_ISACTIVE = 'True' order by [ENV_SORTORDER]
	End
	else
	Begin
		select [ENV_ID] 
		,[ENV_NAME]
      --,[ENV_HOST_IP_ADDRESS]
      --,[ENV_LOCATION]
      ,[ENV_IS_MONITOR]
      ,[ENV_IS_NOTIFY]
      ,[ENV_CREATED_BY]
      ,[ENV_CREATED_DATE]
      ,[ENV_UPDATED_BY]
      ,[ENV_UPDATED_DATE]
      ,[ENV_COMMENTS]
      ,[ENV_MAIL_FREQ]
      ,[ENV_IS_CONSLTD_MAIL]	
      ,[ENV_SORTORDER]	
      from [CSM_ENVIRONEMENT] 
	  where [ENV_ID] = @ENV_ID and ENV_ISACTIVE = 'True'  order by [ENV_SORTORDER]
	End
End

SET ANSI_NULLS ON

GO

