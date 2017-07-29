SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[CWT_GetServerPerformanceSchedule](@ENVID int)
As

Begin
	if @ENVID > 0
	Begin
		select sp.[SVR_ID]
		  ,sp.[ENVID]
		  ,env.ENV_NAME
		  ,sp.[CONFIGID]
		  ,con.CONFIG_HOST_IP
		  ,con.CONFIG_PORT_NUMBER
		  ,sp.[SVR_LASTJOBRAN_TIME]
		  ,sp.[SVR_NEXTJOBRAN_TIME]
      From [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE]  sp
      Join CSM_ENVIRONEMENT env
      On env.ENV_ID = sp.[ENVID]
      join CSM_CONFIGURATION con
      On con.CONFIG_ID = sp.[CONFIGID]
      Where sp.[ENVID] = @ENVID
		and con.CONFIG_ISPRIMARY = 1
		and con.CONFIG_ISNOTIFY = 1
	End
	Else
	Begin
		select sp.[SVR_ID]
		  ,sp.[ENVID]
		  ,env.ENV_NAME
		  ,sp.[CONFIGID]
		  ,con.CONFIG_HOST_IP
		  ,con.CONFIG_PORT_NUMBER
		  ,sp.[SVR_LASTJOBRAN_TIME]
		  ,sp.[SVR_NEXTJOBRAN_TIME]
      From [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE]  sp
      Join CSM_ENVIRONEMENT env
      On env.ENV_ID = sp.[ENVID]
      join CSM_CONFIGURATION con
      On con.CONFIG_ID = sp.[CONFIGID]
      Where con.CONFIG_ISPRIMARY = 1
		and con.CONFIG_ISNOTIFY = 1
	End
	
End

GO


