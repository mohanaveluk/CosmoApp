/****** Object:  StoredProcedure [dbo].[CWT_GetMonitorStatus]    Script Date: 08/09/2015 00:15:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetMonitorStatus] (@ENV_ID int /*, @IS_PRIMARY bit*/) As
Declare
	@BuildCount int
Begin

	select @BuildCount = count(*) from dbo.CSM_SERVICEBUILD where ENV_ID = @ENV_ID
	if @BuildCount > 0
	Begin
		select distinct
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
			con.CONFIG_ISPRIMARY,
			mon.MON_ID,
			mon.SCH_ID,
			mon.MON_STATUS,
			mon.MON_COMMENTS,
			mon.MON_CREATED_DATE,
			mon.MON_UPDATED_DATE,
			mon.MON_START_DATE_TIME,
			mon.MON_END_DATE_TIME,
			mon.MON_IS_ACTIVE,
			mon.MON_ISACKNOWLEDGE
			,case when mon.MON_STATUS = 'Running' or mon.MON_STATUS = 'Standby' then
			cast(cast(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) as int ) as varchar(10))  + 'd' + ', ' 
		  + cast(cast((cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) - 
			floor(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60)) ) * 24 as int) as varchar(10)) + 'h' + ', ' 

		 + cast( cast(((cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) 
		  - floor(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60)))*24
			-
			cast(floor((cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) 
		  - floor(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60)))*24) as decimal)) * 60 as int) as varchar(10)) + 'm'  
		  else '0d, 0h, 0m'
		  end as MON_UPTIME
		  ,(select BUILD_VERSION from [CSM_SERVICEBUILD] T1
				inner join		
				(
					select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
					where CONFIG_ID = con.CONFIG_ID
					group by env_id, config_id
				) T2
				on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
				) BUILD_VERSION
		  ,(select CREATED_DATE from [CSM_SERVICEBUILD] T1
				inner join		
				(
					select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
					where CONFIG_ID = con.CONFIG_ID
					group by env_id, config_id
				) T2
				on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
				)CREATED_DATE
		from CSM_CONFIGURATION con
		left join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
		--left join [dbo].[CSM_SERVICEBUILD] sb on sb.CONFIG_ID = con.CONFIG_ID
		where con.ENV_ID = @ENV_ID  
			and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID union Select null) or mon.MON_ID is null)
			and con.CONFIG_IS_ACTIVE = 'true'
			--and (sb.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [dbo].[CSM_SERVICEBUILD] where ENV_ID = con.ENV_ID group by CONFIG_ID) ) 
		order by CONFIG_SERVICE_TYPE, CONFIG_HOST_IP, CONFIG_PORT_NUMBER
	End
	Else
	Begin
		select distinct
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
			con.CONFIG_ISPRIMARY,
			mon.MON_ID,
			mon.SCH_ID,
			mon.MON_STATUS,
			mon.MON_COMMENTS,
			mon.MON_CREATED_DATE,
			mon.MON_UPDATED_DATE,
			mon.MON_START_DATE_TIME,
			mon.MON_END_DATE_TIME,
			mon.MON_IS_ACTIVE,
			mon.MON_ISACKNOWLEDGE
			,case when mon.MON_STATUS = 'Running' or mon.MON_STATUS = 'Standby' then
			cast(cast(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) as int ) as varchar(10))  + 'd' + ', ' 
		  + cast(cast((cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) - 
			floor(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60)) ) * 24 as int) as varchar(10)) + 'h' + ', ' 

		 + cast( cast(((cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) 
		  - floor(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60)))*24
			-
			cast(floor((cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60) 
		  - floor(cast(datediff(MINUTE,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) / (24*60)))*24) as decimal)) * 60 as int) as varchar(10)) + 'm'  
		  else '0d, 0h, 0m'
		  end as MON_UPTIME
		  ,(select BUILD_VERSION from [CSM_SERVICEBUILD] T1
				inner join		
				(
					select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
					where CONFIG_ID = con.CONFIG_ID
					group by env_id, config_id
				) T2
				on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
				) BUILD_VERSION
		  ,(select CREATED_DATE from [CSM_SERVICEBUILD] T1
				inner join		
				(
					select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
					where CONFIG_ID = con.CONFIG_ID
					group by env_id, config_id
				) T2
				on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
				)CREATED_DATE
		from CSM_CONFIGURATION con
		left join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
		--left join [dbo].[CSM_SERVICEBUILD] sb on sb.CONFIG_ID = con.CONFIG_ID
		where con.ENV_ID = @ENV_ID  
			and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID union Select null) or mon.MON_ID is null)
			and con.CONFIG_IS_ACTIVE = 'true'
			--and (sb.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [dbo].[CSM_SERVICEBUILD] where ENV_ID = con.ENV_ID group by CONFIG_ID) ) 
		order by CONFIG_SERVICE_TYPE, CONFIG_HOST_IP, CONFIG_PORT_NUMBER
	End
End

SET ANSI_NULLS ON
GO