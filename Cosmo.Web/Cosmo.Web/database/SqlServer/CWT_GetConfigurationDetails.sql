/****** Object:  StoredProcedure [dbo].[CWT_GetConfigurationDetails]    Script Date: 08/23/2015 18:56:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetConfigurationDetails]
(
	@ENV_ID int
)
as
Begin
	if(@ENV_ID>0 or @ENV_ID is not null)
	Begin
		select 
		   con.[CONFIG_ID]
		  ,con.[ENV_ID]
		  ,[CSM_ENVIRONEMENT].ENV_NAME
		  ,[CONFIG_SERVICE_TYPE]
		  ,[CONFIG_PORT_NUMBER]
		  ,[CONFIG_URL_ADDRESS]
		  ,[CONFIG_DESCRIPTION]
		  ,[CONFIG_IS_VALIDATED]
		  ,[CONFIG_IS_ACTIVE]
		  ,[CONFIG_IS_MONITORED]
		  ,[CONFIG_IS_LOCKED]
		  ,[CONFIG_CREATED_BY]
		  ,[CONFIG_CREATED_DATE]
		  ,[CONFIG_UPDATED_BY]
		  ,[CONFIG_UPDATED_DATE]
		  ,[CONFIG_COMMENTS]
		  ,[CONFIG_HOST_IP]
		  ,[CONFIG_MAIL_FREQ]
		  ,[CONFIG_LOCATION]
		  ,[CONFIG_ISCONSOLIDATED]
		  ,[CONFIG_ISNOTIFY]
		  ,[CONFIG_ISPRIMARY]
		  ,win.WIN_SERVICENAME
		  ,win.WIN_SERVICE_ID
		  ,usr.USER_FIRST_NAME USERFIRSTNAME
		  ,usr.USER_LAST_NAME USERLASTNAME
	  from [CSM_CONFIGURATION] con
	  join [CSM_ENVIRONEMENT] on con.[ENV_ID] = [CSM_ENVIRONEMENT].[ENV_ID]
	  left outer join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
	  join dbo.CSM_USER usr on usr.USER_ID = [CONFIG_CREATED_BY]
		where [CONFIG_IS_ACTIVE] = 'True' and 	
		con.[ENV_ID] = @ENV_ID
		
	End
End
	
SET ANSI_NULLS ON

--exec [CWT_GetConfigurationDetails] 1

GO


