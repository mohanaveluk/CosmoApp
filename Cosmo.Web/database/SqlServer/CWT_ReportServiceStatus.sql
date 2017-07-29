/****** Object:  StoredProcedure [dbo].[CWT_ReportServiceStatus]    Script Date: 10/12/2015 16:04:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_ReportServiceStatus]
(
	@ENV_ID int
	,@STARTDATE date
	,@ENDDATE date
)
As
Begin
	SELECT 
		mds.[MON_TRACK_ID]
		,mds.[MON_ID]
		,mds.[ENV_ID]
		,mds.[CONFIG_ID]
		,con.CONFIG_SERVICE_TYPE
		,con.CONFIG_HOST_IP
		,con.CONFIG_DESCRIPTION
		,con.CONFIG_PORT_NUMBER
		,con.CONFIG_DESCRIPTION
		,CONVERT(VARCHAR(10),mds.[MON_TRACK_DATE],101) [MON_CREATED_DATE] --MON_TRACK_DATE
		,case 
			when mds.[MON_TRACK_STATUS] = 'Running' and con.CONFIG_SERVICE_TYPE = '1' then 'A'
			when mds.[MON_TRACK_STATUS] = 'Running' and con.CONFIG_SERVICE_TYPE = '2' then 'R'
			when mds.[MON_TRACK_STATUS] = 'Standby' then 'S'
			when mds.[MON_TRACK_STATUS] = 'Stopped' then 'D'
			when mds.[MON_TRACK_STATUS] = 'Not Ready' then 'D'
			when mds.[MON_TRACK_STATUS] = 'Not running.  See logs.' then 'D'
			else 'N/A'
		end MON_STATUS --[MON_TRACK_STATUS]
		,mds.[MON_TRACK_COMMENTS] [MON_COMMENTS]
	  FROM [dbo].[CSM_MON_DAILY_STATUS] mds
	  inner join CSM_CONFIGURATION con on con.CONFIG_ID = mds.[CONFIG_ID]
	  where mds.[ENV_ID] = @ENV_ID
		and CONVERT(date, CONVERT(VARCHAR(10),mds.[MON_TRACK_DATE],101)) between 
			CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
		and con.CONFIG_IS_ACTIVE = 'True'
		and con.CONFIG_ISPRIMARY = 'True'
	  order by 
		mds.[ENV_ID], 
		mds.[CONFIG_ID], 
		con.CONFIG_SERVICE_TYPE, 
		MON_TRACK_DATE
End		

GO