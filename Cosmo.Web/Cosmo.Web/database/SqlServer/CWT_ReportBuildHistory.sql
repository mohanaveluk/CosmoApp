SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_ReportBuildHistory](

	@ENV_ID int
	,@STARTDATE date
	,@ENDDATE date
	
) As
Begin
	if @ENV_ID > 0
	Begin
		select
			mon.CONFIG_ID,
			mon.BUILD_VERSION MON_COMMENTS,
			mon.BUILD_DATE MON_BUILD_DATE,
			CONVERT(VARCHAR(19),mon.CREATED_DATE, 120) MON_CREATED_DATE,
			case con.CONFIG_SERVICE_TYPE 
				when '1' then 'Content Manager'
				when '2' then 'Dispatcher'
			end 	CONFIG_SERVICE_TYPE
			,con.CONFIG_PORT_NUMBER,
			con.CONFIG_DESCRIPTION,
			con.CONFIG_HOST_IP,
			con.CONFIG_LOCATION,
			con.CONFIG_ISPRIMARY
		from [dbo].[CSM_SERVICEBUILD] mon
		inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
		where con.ENV_ID = @Env_ID
			--and CONVERT(date, CONVERT(VARCHAR(10), mon.CREATED_DATE, 101)) between  
			--	CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
			--and mon.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [Bootcosmo].[dbo].[CSM_SERVICEBUILD] group by CONFIG_ID)
		order by CONFIG_ID, MON_CREATED_DATE
	End
	else
	Begin
		select
			mon.CONFIG_ID,
			mon.BUILD_VERSION MON_COMMENTS,
			mon.BUILD_DATE MON_BUILD_DATE,
			CONVERT(VARCHAR(19),mon.CREATED_DATE, 120) MON_CREATED_DATE,
			case con.CONFIG_SERVICE_TYPE 
				when '1' then 'Content Manager'
				when '2' then 'Dispatcher'
			end 	CONFIG_SERVICE_TYPE,
			con.CONFIG_PORT_NUMBER,
			con.CONFIG_DESCRIPTION,
			con.CONFIG_HOST_IP,
			con.CONFIG_LOCATION,
			con.CONFIG_ISPRIMARY
			,con.ENV_ID
		from [dbo].[CSM_SERVICEBUILD] mon
		inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
		--where 	CONVERT(date, CONVERT(VARCHAR(10), mon.CREATED_DATE, 101)) between  
		--		CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
		--		and mon.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [Bootcosmo].[dbo].[CSM_SERVICEBUILD] group by CONFIG_ID)
		order by CONFIG_ID, MON_CREATED_DATE
	End
End

GO