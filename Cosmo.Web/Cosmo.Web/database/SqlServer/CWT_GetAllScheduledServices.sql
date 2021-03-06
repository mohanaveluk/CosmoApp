/****** Object:  StoredProcedure [dbo].[CWT_GetAllScheduledServices]    Script Date: 2/9/2015 6:00:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetAllScheduledServices]
	(
		@DateNextJobRunStartBefore datetime
	)
	As
	Begin
	DECLARE 
		@currentDateTime varchar(100)
		set @currentDateTime = CONVERT(VARCHAR(19), @DateNextJobRunStartBefore, 120)
	Select
		[CONFIG_ID]
      ,cc.[ENV_ID]
      ,[CONFIG_SERVICE_TYPE]
      ,[CONFIG_HOST_IP]
      ,[CONFIG_PORT_NUMBER]
      ,[CONFIG_URL_ADDRESS]
      ,[CONFIG_DESCRIPTION]
      ,[CONFIG_IS_VALIDATED]
      ,[CONFIG_IS_ACTIVE]
      ,[CONFIG_IS_MONITORED]
      ,[CONFIG_IS_LOCKED]
      ,[CONFIG_CREATED_BY]
      ,[CONFIG_CREATED_DATE]
      ,[CONFIG_UPDATED_BY]
      ,[CONFIG_UPDATED_DATE]
      ,[CONFIG_COMMENTS]
      ,ce.ENV_NAME
      ,cs.SCH_ID
      from [CSM_CONFIGURATION] cc
      inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = cc.ENV_ID
      inner join CSM_SCHEDULE cs on cs.ENV_ID = cc.ENV_ID and cs.CONGIG_ID = cc.CONFIG_ID
      where cc.Config_ID in (
		Select 
		[CONGIG_ID]
		from CSM_SCHEDULE cs 
		where (SCH_NEXTJOBRAN_TIME is null or CONVERT(VARCHAR(19), cs.SCH_NEXTJOBRAN_TIME, 120) <= CONVERT(VARCHAR(19), @currentDateTime, 120)) 
		and CONVERT(VARCHAR(19), cs.SCH_STARTBY, 120) <= CONVERT(VARCHAR(19), @currentDateTime, 120)
		and CONVERT(VARCHAR(19), cs.SCH_ENDBY, 120) >= CONVERT(VARCHAR(19), @currentDateTime, 120)
		and cs.SCH_IS_ACTIVE = 'True'      
	  ) 
	  and cc.CONFIG_IS_ACTIVE = 'True' 
	  and cc.CONFIG_IS_MONITORED = 'True'  
	  and cc.CONFIG_IS_VALIDATED = 'True'
      	
	End
	


-- exec CWT_GetAllScheduledServices '12/8/2014 8:32:40 PM'/****** Object:  StoredProcedure [dbo].[CWT_GetAllSchedule_NextJobStartBefore]    Script Date: 2/9/2015 5:59:04 PM ******/
