/****** Object:  StoredProcedure [dbo].[CWT_GetSendNotification]    Script Date: 2/9/2015 6:14:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetSendNotification]
(
	@CurrentTimeStamp datetime
)
As
Begin
Declare
	@currentDateTime varchar(100)
	set @currentDateTime = CONVERT(VARCHAR(19), @CurrentTimeStamp, 120)
select 
	con.[CONFIG_ID],
	con.ENV_ID,
	con.CONFIG_SERVICE_TYPE,
	con.CONFIG_IS_ACTIVE,
	con.CONFIG_ISNOTIFY,
	con.CONFIG_MAIL_FREQ,
	mon.MON_STATUS,
	--max(eml.[EMTRAC_CREATED_DATE]) [EMTRAC_CREATED_DATE]
	(select max([EMTRAC_CREATED_DATE]) from [CSM_EMAIL_TRACKING] where [Config_ID] = con.[CONFIG_ID] AND ENV_ID = con.ENV_ID) EMTRAC_CREATED_DATE
	from [CSM_CONFIGURATION] con
	left  join [CSM_MONITOR] mon on mon.CONFIG_ID = con.CONFIG_ID
	left  join [CSM_EMAIL_TRACKING] eml on eml.[Config_ID] = mon.CONFIG_ID
	--and eml.[Config_ID] = con.CONFIG_ID
	where (lower(mon.MON_STATUS) = 'stopped' or lower(mon.MON_STATUS) like '%not running%') 
		--and CONVERT(VARCHAR(19), @currentDateTime, 120) >= CONVERT(VARCHAR(19), DATEADD(mi,con.CONFIG_MAIL_FREQ,eml.[EMTRAC_CREATED_DATE]), 120)
		and con.CONFIG_IS_ACTIVE = 'True'
		and con.CONFIG_ISNOTIFY = 'True'
		and con.[CONFIG_ID] in
		(
		Select 
			[CONGIG_ID]
			from CSM_SCHEDULE cs 
			where (SCH_NEXTJOBRAN_TIME is null or CONVERT(VARCHAR(19), cs.SCH_NEXTJOBRAN_TIME, 120) <= CONVERT(VARCHAR(19), @currentDateTime, 120)) 
			and CONVERT(VARCHAR(19), cs.SCH_STARTBY, 120) <= CONVERT(VARCHAR(19), @currentDateTime, 120)
			and CONVERT(VARCHAR(19), cs.SCH_ENDBY, 120) >= CONVERT(VARCHAR(19), @currentDateTime, 120)
			and cs.SCH_IS_ACTIVE = 'True' 
		)
	Group by
		con.[CONFIG_ID],
		con.ENV_ID,
		con.CONFIG_SERVICE_TYPE,
		con.CONFIG_IS_ACTIVE,
		con.CONFIG_ISNOTIFY,
		con.CONFIG_MAIL_FREQ,
		mon.MON_STATUS

End

SET ANSI_NULLS ON
GO


