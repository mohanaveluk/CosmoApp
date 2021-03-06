/****** Object:  StoredProcedure [dbo].[CWT_GetAllIncident]    Script Date: 2/9/2015 5:58:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetAllIncident]
(
	@ENV_ID int,
	@Type varchar(10)
)
As

Begin

	if(@ENV_ID is not null and @ENV_ID <= 0)
	Begin
		SELECT  con.[CONFIG_ID]
			  ,con.[ENV_ID]
			  ,[CONFIG_SERVICE_TYPE]
			  ,[CONFIG_PORT_NUMBER]
			  ,[CONFIG_DESCRIPTION]
			  ,[CONFIG_HOST_IP]
			  ,[CONFIG_LOCATION]
			  ,[CONFIG_ISPRIMARY]
			  ,[MON_ID]
			  ,[MON_STATUS]
			  ,[MON_CREATED_DATE]
		  FROM [CSM_CONFIGURATION] con
		  inner join [CSM_MONITOR] mon on mon.CONFIG_ID = con.CONFIG_ID
		  Where [CONFIG_ISPRIMARY] = 'True'
				And lower([MON_STATUS]) not in ('running','standby')
				And [MON_ID] not in (select [MON_ID] from [CSM_INCIDENT])

		End
	if(@ENV_ID is not null and @ENV_ID > 0)
	Begin
		if(lower(@Type)='pnd')
		Begin
			SELECT  con.[CONFIG_ID]
				  ,con.[ENV_ID]
				  ,[CONFIG_SERVICE_TYPE]
				  ,[CONFIG_PORT_NUMBER]
				  ,[CONFIG_DESCRIPTION]
				  ,[CONFIG_HOST_IP]
				  ,[CONFIG_LOCATION]
				  ,[CONFIG_ISPRIMARY]
				  ,[MON_ID]
				  ,[MON_STATUS]
				  ,[MON_CREATED_DATE]
			  FROM [CSM_CONFIGURATION] con
			  inner join [CSM_MONITOR] mon on mon.CONFIG_ID = con.CONFIG_ID
			  Where [CONFIG_ISPRIMARY] = 'True'
					And lower([MON_STATUS]) not in ('running','standby')
					And [MON_ID] not in (select [MON_ID] from [CSM_INCIDENT])	
					And mon.ENV_ID = @ENV_ID
		End
		else if(lower(@Type)='all')
		Begin
			SELECT  con.[CONFIG_ID]
				  ,con.[ENV_ID]
				  ,[CONFIG_SERVICE_TYPE]
				  ,[CONFIG_PORT_NUMBER]
				  ,[CONFIG_DESCRIPTION]
				  ,[CONFIG_HOST_IP]
				  ,[CONFIG_LOCATION]
				  ,[CONFIG_ISPRIMARY]
				  ,[MON_ID]
				  ,[MON_STATUS]
				  ,[MON_CREATED_DATE]
			  FROM [CSM_CONFIGURATION] con
			  inner join [CSM_MONITOR] mon on mon.CONFIG_ID = con.CONFIG_ID
			  Where [CONFIG_ISPRIMARY] = 'True'
					And lower([MON_STATUS]) not in ('running','standby')
					And mon.ENV_ID = @ENV_ID
		End
	End
End

GO


