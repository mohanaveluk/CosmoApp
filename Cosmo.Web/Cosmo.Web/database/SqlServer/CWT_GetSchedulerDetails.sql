/****** Object:  StoredProcedure [dbo].[CWT_GetSchedulerDetails]    Script Date: 2/9/2015 6:14:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetSchedulerDetails] (@ENVID int, @CONFIGID int) AS
Begin
	if(@ENVID > 0 and @CONFIGID > 0)
	Begin
		SELECT [SCH_ID]
		  ,[ENV_ID]
		  ,[CONGIG_ID]
		  ,[SCH_INTERVAL]
		  ,[SCH_DURATION]
		  ,[SCH_REPEATS]
		  ,[SCH_STARTBY]
		  ,[SCH_ENDAS]
		  ,[SCH_END_OCCURANCE]
		  ,[SCH_ENDBY]
		  ,[SCH_IS_ACTIVE]
		  ,[SCH_LASTJOBRAN_TIME]
		  ,[SCH_NEXTJOBRAN_TIME]
		  ,[SCH_CREATED_BY]
		  ,[SCH_CREATED_DATE]
		  ,[SCH_UPDATED_BY]
		  ,[SCH_UPDATED_DATE]
		  ,[SCH_COMMENTS]
		FROM [dbo].[CSM_SCHEDULE]
		Where [ENV_ID] = @ENVID and [CONGIG_ID] = @CONFIGID
	End
	else if(@ENVID > 0 and @CONFIGID <= 0)
	Begin
		SELECT [SCH_ID]
		  ,[ENV_ID]
		  ,[CONGIG_ID]
		  ,[SCH_INTERVAL]
		  ,[SCH_DURATION]
		  ,[SCH_REPEATS]
		  ,[SCH_STARTBY]
		  ,[SCH_ENDAS]
		  ,[SCH_END_OCCURANCE]
		  ,[SCH_ENDBY]
		  ,[SCH_IS_ACTIVE]
		  ,[SCH_LASTJOBRAN_TIME]
		  ,[SCH_NEXTJOBRAN_TIME]
		  ,[SCH_CREATED_BY]
		  ,[SCH_CREATED_DATE]
		  ,[SCH_UPDATED_BY]
		  ,[SCH_UPDATED_DATE]
		  ,[SCH_COMMENTS]
		FROM [dbo].[CSM_SCHEDULE]
		Where [ENV_ID] = @ENVID
	End
End	


--exec CWT_GetSchedulerDetails 2,6