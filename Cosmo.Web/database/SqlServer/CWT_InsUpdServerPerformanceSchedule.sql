SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


  CREATE procedure [dbo].[CWT_InsUpdServerPerformanceSchedule]
  (
	@ENVID int
	,@CONFIGID int
	,@HOSTIP varchar(250)
	,@PORT varchar(10)
	,@LASTJOBRUNTIME datetime
	,@NEXTJOBRUNTIME datetime
	,@MODE varchar(10)
  )
  As
  declare 
	@SVRID int
  Begin
	select @SVRID = [SVR_ID] from [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE] 
	where [ENVID] = @ENVID
	and [CONFIGID] = @CONFIGID
	
	--if @SVRID > 0
	--Begin
		--update [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE] set
		--[SVR_LASTJOBRAN_TIME] = @LASTJOBRUNTIME,
		--[SVR_NEXTJOBRAN_TIME] = @NEXTJOBRUNTIME
		--where [ENVID] = @ENVID
		--and [CONFIGID] = @CONFIGID
	--End
	--Else
	if @MODE = 'NS' -- Next schedule
	Begin
		update [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE] set
		[SVR_LASTJOBRAN_TIME] = @LASTJOBRUNTIME,
		[SVR_NEXTJOBRAN_TIME] = @NEXTJOBRUNTIME
		where [SVR_HOSTIP] = @HOSTIP
	End

	if (@SVRID <= 0 or @SVRID is null or @SVRID = '') and @MODE = 'IS' -- Initial schedule
	Begin
		insert into [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE] 
		(
			[ENVID]
		  ,[CONFIGID]
		  ,[SVR_HOSTIP]
		  ,[SVR_PORT]
		  ,[SVR_LASTJOBRAN_TIME]
		  ,[SVR_NEXTJOBRAN_TIME]
		)
		values
		(
			@ENVID
			,@CONFIGID
			,@HOSTIP
			,@PORT
			,@LASTJOBRUNTIME
			,@NEXTJOBRUNTIME
		)
	End
  End
GO


