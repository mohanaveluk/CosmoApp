/****** Object:  StoredProcedure [dbo].[CWT_GetMonitorStatusWithServiceName_ConID]    Script Date: 08/09/2015 00:13:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetMonitorStatusWithServiceName_ConID] (@CONFIG_ID int) As
Begin
	select 
		con.ENV_ID,
		con.CONFIG_ID,
		con.CONFIG_HOST_IP,
		con.CONFIG_PORT_NUMBER,
		con.CONFIG_DESCRIPTION,
		con.CONFIG_SERVICE_TYPE,
		con.CONFIG_URL_ADDRESS,
		con.CONFIG_IS_MONITORED,
		con.CONFIG_ISNOTIFY,
		con.CONFIG_MAIL_FREQ,
		con.CONFIG_LOCATION,
		mon.MON_ID,
		mon.SCH_ID,
		mon.MON_STATUS,
		mon.MON_COMMENTS,
		mon.MON_CREATED_DATE,
		mon.MON_UPDATED_DATE,
		mon.MON_START_DATE_TIME,
		mon.MON_END_DATE_TIME,
		mon.MON_IS_ACTIVE,
		mon.MON_ISACKNOWLEDGE,
		win.WIN_SERVICENAME,
		win.WIN_SERVICE_ID
	from CSM_CONFIGURATION con
	left outer join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
	left outer join [CSM_WINDOWS_SERVICES] win on win.CONFIG_ID = con.CONFIG_ID
	where con.CONFIG_ID = @CONFIG_ID
		and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID, SCH_ID) or mon.MON_ID is null)
		and con.CONFIG_IS_ACTIVE = 'true'
		and con.CONFIG_ISPRIMARY = 'true'
	--order by CONFIG_SERVICE_TYPE

End


SET ANSI_NULLS ON

GO



