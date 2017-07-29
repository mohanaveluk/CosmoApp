/****** Object:  StoredProcedure [dbo].[CWT_GetSubscriptionMonitorStatus]    Script Date: 09/13/2016 22:13:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_GetSubscriptionMonitorStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_GetSubscriptionMonitorStatus]
GO


/****** Object:  StoredProcedure [dbo].[CWT_GetSubscriptionMonitorStatus]    Script Date: 09/13/2016 22:13:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetSubscriptionMonitorStatus]
(
	@ENV_ID int
	,@TYPE varchar(50)
	,@STARTTIME datetime
	,@ENDTIME datetime
) As
Begin
	if @TYPE = 'Daily'
	Begin
		select m.MON_ID
		,m.ENV_ID
		,m.CONFIG_ID
		,mds.MON_TRACK_STATUS [MON_STATUS]
		,m.[MON_CREATED_DATE]
		,m.[MON_UPDATED_DATE]
		, case 
			when c.CONFIG_SERVICE_TYPE = 1 then 'Content Manager'
			when c.CONFIG_SERVICE_TYPE = 2 then 'Dispatcher'
		  end CONFIG_SERVICE_TYPE
		,c.CONFIG_HOST_IP
		,c.CONFIG_PORT_NUMBER
		,c.CONFIG_DESCRIPTION
		,c.CONFIG_ISPRIMARY
		,mds.MON_TRACK_DATE	
		from CSM_MONITOR m
		left join [CSM_MON_DAILY_STATUS] mds 
		ON m.MON_ID = mds.MON_ID
		left join CSM_CONFIGURATION c
		ON m.CONFIG_ID = c.CONFIG_ID and
			m.ENV_ID = c.ENV_ID
		where 
			m.ENV_ID = @ENV_ID
			and mds.MON_TRACK_DATE >= @STARTTIME
			and mds.MON_TRACK_DATE <= @ENDTIME
	End
End

GO


