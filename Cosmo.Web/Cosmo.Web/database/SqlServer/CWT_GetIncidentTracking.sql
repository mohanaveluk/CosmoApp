
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetIncidentTracking]
(
	@ENV_ID int,
	@FromTime datetime,
	@ToTime datetime
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
				,mon.[MON_ID]
				,[MON_STATUS]
				,[MON_CREATED_DATE]
				,inc.[TRK_ISSUE]
				,inc.[TRK_SOLUTION]
				,inc.[TRK_ID]
				,inc.TRK_CREATED_DATE
				,inc.TRK_CREATED_BY
			FROM [CSM_CONFIGURATION] con
			inner join [CSM_MONITOR] mon on mon.CONFIG_ID = con.CONFIG_ID
			inner join [CSM_INCIDENT] inc on inc.MON_ID = mon.MON_ID
			Where [CONFIG_ISPRIMARY] = 'True'
			And ([MON_CREATED_DATE] between CONVERT(VARCHAR(19), @FromTime, 120) and CONVERT(VARCHAR(19), @ToTime, 120))
		End
	if(@ENV_ID is not null and @ENV_ID > 0)
	Begin
		SELECT  con.[CONFIG_ID]
				,con.[ENV_ID]
				,[CONFIG_SERVICE_TYPE]
				,[CONFIG_PORT_NUMBER]
				,[CONFIG_DESCRIPTION]
				,[CONFIG_HOST_IP]
				,[CONFIG_LOCATION]
				,[CONFIG_ISPRIMARY]
				,mon.[MON_ID]
				,[MON_STATUS]
				,[MON_CREATED_DATE]
				,inc.[TRK_ISSUE]
				,inc.[TRK_SOLUTION]
				,inc.[TRK_ID]
				,inc.TRK_CREATED_DATE
				,inc.TRK_CREATED_BY
			FROM [CSM_CONFIGURATION] con
			inner join [CSM_MONITOR] mon on mon.CONFIG_ID = con.CONFIG_ID
			inner join [CSM_INCIDENT] inc on inc.MON_ID = mon.MON_ID
			Where [CONFIG_ISPRIMARY] = 'True'
				And ([MON_CREATED_DATE] between CONVERT(VARCHAR(19), @FromTime, 120) and CONVERT(VARCHAR(19), @ToTime, 120))
				And mon.ENV_ID = @ENV_ID
	End
End

SET ANSI_NULLS ON

GO


