/****** Object:  StoredProcedure [dbo].[CWT_GetAllSchedule_NextJobStartBefore]    Script Date: 2/9/2015 5:59:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  procedure [dbo].[CWT_GetAllSchedule_NextJobStartBefore] 
@DateNextJobRunStartBefore datetime
As 
Begin
	DECLARE 
		@currentDateTime varchar(100)
	set @currentDateTime = CONVERT(VARCHAR(19), @DateNextJobRunStartBefore, 120)
	Select 
		[SCH_ID]
      ,cs.[ENV_ID]
      ,ce.[ENV_NAME]
      ,[SCH_INTERVAL]
      ,[SCH_DURATION]
      ,[SCH_IS_ACTIVE]
      ,[SCH_LASTJOBRAN_TIME]
      ,[SCH_NEXTJOBRAN_TIME]
      ,[SCH_CREATED_BY]
      ,[SCH_CREATED_DATE]
      ,[SCH_UPDATED_BY]
      ,[SCH_UPDATED_DATE]
      ,[SCH_COMMENTS]	
	from CSM_SCHEDULE cs inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = cs.ENV_ID
	where (SCH_NEXTJOBRAN_TIME is NULL or CONVERT(VARCHAR(19), cs.SCH_NEXTJOBRAN_TIME, 120) <= CONVERT(VARCHAR(19), @currentDateTime, 120)) 
	and CONVERT(VARCHAR(19), cs.SCH_STARTBY, 120) <= CONVERT(VARCHAR(19), @currentDateTime, 120)
	and CONVERT(VARCHAR(19), cs.SCH_ENDBY, 120) >= CONVERT(VARCHAR(19), @currentDateTime, 120)
	and cs.SCH_IS_ACTIVE = 'True'
End

--exec [CWT_GetAllSchedule_NextJobStartBefore] '12/8/2014 8:44:01 PM' 