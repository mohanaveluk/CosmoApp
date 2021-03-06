/****** Object:  StoredProcedure [dbo].[CWT_GetServiceAvailabilityForDowntime]    Script Date: 2/9/2015 6:15:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetServiceAvailabilityForDowntime]
(
	@ENV_ID int,
	@FromTime datetime,
	@ToTime datetime
) AS
Begin
select MON_ID,
		CONFIG_ID, 
		ENV_ID, 
		MON_STATUS, 
		MON_CREATED_DATE, 
		MON_UPDATED_DATE
		--cast(datediff(MI,@FromTime,@ToTime) as decimal(10,3)) as input,
		--cast(datediff(MI,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) as minutediff
		--sum(cast(datediff(MI,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3))) as minutediff

			from CSM_MONITOR mon
			where (MON_UPDATED_DATE >= CONVERT(VARCHAR(19), @FromTime, 120) and MON_UPDATED_DATE <= CONVERT(VARCHAR(19), @ToTime, 120)  
			or (MON_CREATED_DATE <=  CONVERT(VARCHAR(19), @ToTime, 120) AND MON_CREATED_DATE >=CONVERT(VARCHAR(19), @FromTime, 120) ))
			and lower(MON_STATUS) != 'running' AND lower(MON_STATUS) != 'standby'
			and mon.ENV_ID = @ENV_ID
			order by mon.CONFIG_ID, MON_UPDATED_DATE
End

--exec CWT_GetServiceAvailabilityForDowntime 2, '1/1/2015 12:00:00 AM','1/4/2015 1:52:55 PM'