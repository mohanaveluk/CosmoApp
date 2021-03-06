/****** Object:  StoredProcedure [dbo].[CWT_GetServiceAvailability]    Script Date: 2/9/2015 6:15:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetServiceAvailability](@ENV_ID int, @FromTime datetime, @ToTime datetime, @Type varchar(10)) As
Begin
	if(@ENV_ID = 0)
	begin
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
			mon.MON_ID,
			mon.SCH_ID,
			mon.MON_STATUS,
			mon.MON_COMMENTS,
			mon.MON_CREATED_DATE,
			mon.MON_UPDATED_DATE,
			mon.MON_START_DATE_TIME,
			mon.MON_END_DATE_TIME,
			mon.MON_IS_ACTIVE
			from CSM_CONFIGURATION con
			inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
			inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = mon.ENV_ID
			where (MON_UPDATED_DATE between CONVERT(VARCHAR(19), @FromTime, 120) and CONVERT(VARCHAR(19), @ToTime, 120) 
			or CONVERT(VARCHAR(19), @ToTime, 120) <= MON_UPDATED_DATE)
			and ce.ENV_ISACTIVE = 'true'
			and con.CONFIG_IS_ACTIVE = 'true'
			order by mon.ENV_ID, MON_UPDATED_DATE
	End
	else
	Begin
		if(@Type='all')
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
			mon.MON_ID,
			mon.SCH_ID,
			mon.MON_STATUS,
			mon.MON_COMMENTS,
			mon.MON_CREATED_DATE,
			mon.MON_UPDATED_DATE,
			mon.MON_START_DATE_TIME,
			mon.MON_END_DATE_TIME,
			mon.MON_IS_ACTIVE
			from CSM_CONFIGURATION con
			inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
			inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = mon.ENV_ID
			where (MON_UPDATED_DATE between CONVERT(VARCHAR(19), @FromTime, 120) and CONVERT(VARCHAR(19), @ToTime, 120)
			 or CONVERT(VARCHAR(19), @ToTime, 120) <= MON_UPDATED_DATE)
			and ce.ENV_ISACTIVE = 'true'
			and con.CONFIG_IS_ACTIVE = 'true'
			and mon.ENV_ID = @ENV_ID
			order by mon.ENV_ID, MON_UPDATED_DATE
		End
		else if(@Type='env')
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
			mon.MON_ID,
			mon.SCH_ID,
			mon.MON_STATUS,
			mon.MON_COMMENTS,
			mon.MON_CREATED_DATE,
			mon.MON_UPDATED_DATE,
			mon.MON_START_DATE_TIME,
			mon.MON_END_DATE_TIME,
			mon.MON_IS_ACTIVE
			from CSM_CONFIGURATION con
			inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
			inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = mon.ENV_ID
			where 
			((MON_CREATED_DATE >= CONVERT(VARCHAR(19), @FromTime, 120) and MON_UPDATED_DATE <= CONVERT(VARCHAR(19), @ToTime, 120))
			or (MON_CREATED_DATE between CONVERT(VARCHAR(19), @FromTime, 120) and CONVERT(VARCHAR(19), @ToTime, 120)))
			and ce.ENV_ISACTIVE = 'true'
			and con.CONFIG_IS_ACTIVE = 'true'
			and mon.ENV_ID = @ENV_ID
			order by mon.CONFIG_ID, con.CONFIG_SERVICE_TYPE, MON_UPDATED_DATE
		End
	End
End

--select * from CSM_MONITOR
--exec CWT_GetServiceAvailability 1, '1/25/2015 3:08:08 PM','1/26/2015 3:08:08 PM', 'env'