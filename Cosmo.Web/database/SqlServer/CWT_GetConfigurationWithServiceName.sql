/****** Object:  StoredProcedure [dbo].[CWT_GetConfigurationWithServiceName]    Script Date: 06/26/2015 14:16:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[CWT_GetConfigurationWithServiceName] (@ENV_ID int) As
Begin
	select 
		con.ENV_ID,
		con.CONFIG_ID,
		con.CONFIG_HOST_IP,
		con.CONFIG_PORT_NUMBER,
		con.CONFIG_SERVICE_TYPE,
		con.CONFIG_URL_ADDRESS,
		con.CONFIG_IS_MONITORED,
		con.CONFIG_ISNOTIFY,
		con.CONFIG_MAIL_FREQ,
		con.CONFIG_LOCATION,
		win.WIN_SERVICENAME,
		win.WIN_SERVICE_ID
	from CSM_CONFIGURATION con
	left outer join [CSM_WINDOWS_SERVICES] win on win.CONFIG_ID = con.CONFIG_ID
	where con.ENV_ID = @ENV_ID  
		and con.CONFIG_IS_ACTIVE = 'true'
		and con.CONFIG_ISPRIMARY = 'true'
	order by CONFIG_SERVICE_TYPE

End


-- exec [CWT_GetConfigurationWithServiceName] 3 /****** Object:  StoredProcedure [dbo].[CWT_GetIncidentTracking]    Script Date: 2/9/2015 6:14:02 PM ******/
SET ANSI_NULLS ON

GO


