/****** Object:  StoredProcedure [dbo].[CWT_SetMonitorStatus]    Script Date: 10/12/2015 16:03:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  CREATE procedure [dbo].[CWT_SetMonitorStatus](
	@SCH_ID int
	,@CONFIG_ID int
	,@ENV_ID int
	,@MON_STATUS varchar(100)
	,@MON_START_DATE_TIME varchar(200)
	,@MON_END_DATE_TIME varchar(200)
	,@MON_IS_ACTIVE bit
	,@MON_CREATED_BY varchar(100)
	,@MON_CREATED_DATE datetime
	,@MON_COMMENTS varchar(1000)
  )
  As
	declare @tempMonitorID int
  Begin
	Declare 
		@MonitorID int,
		@MonitorStatus varchar(50),
		@UpdateStatus bit,
		@tempDailyStatusCount int
	
	select @MonitorID = max([mon_id]) from [CSM_MONITOR] where SCH_ID = @SCH_ID and CONFIG_ID = @CONFIG_ID and ENV_ID = @ENV_ID
	if (@MonitorID = '' or @MonitorID =0 or @MonitorID is null)
	Begin
		set @UpdateStatus = 0
	End
	else
	Begin
		select @MonitorStatus = MON_STATUS from [CSM_MONITOR] where [mon_id] = @MonitorID
		if (@MonitorStatus != @MON_STATUS)
		Begin	
			set @UpdateStatus = 0
		End
		else
		Begin
			set @UpdateStatus = 1
		End
	End


	If(@UpdateStatus = 0)
		begin
			insert into [CSM_MONITOR] (
			[SCH_ID]
			,[CONFIG_ID]
			,[ENV_ID]
			,[MON_STATUS]
			,[MON_START_DATE_TIME]
			,[MON_END_DATE_TIME]
			,[MON_IS_ACTIVE]
			,[MON_CREATED_BY]
			,[MON_CREATED_DATE]
			,[MON_UPDATED_BY]
			,[MON_UPDATED_DATE]
			,[MON_COMMENTS]
			)values
			(
			@SCH_ID
			,@CONFIG_ID
			,@ENV_ID
			,@MON_STATUS
			,@MON_START_DATE_TIME
			,@MON_END_DATE_TIME
			,@MON_IS_ACTIVE
			,@MON_CREATED_BY
			,@MON_CREATED_DATE
			,@MON_CREATED_BY
			,@MON_CREATED_DATE
			,@MON_COMMENTS      
			)
			set @tempMonitorID = IDENT_CURRENT('CSM_MONITOR')
			exec CWT_InsMonitorDailyStatus 
				@tempMonitorID 
				,@CONFIG_ID
				,@ENV_ID
				,@MON_CREATED_DATE
				,@MON_STATUS
				,''
		end
	else
		begin
		/* Update the monistor status*/
			Update [CSM_MONITOR] set 
				[MON_STATUS] = @MON_STATUS,
				[MON_START_DATE_TIME] = @MON_START_DATE_TIME,
				[MON_END_DATE_TIME] = @MON_END_DATE_TIME,
				[MON_UPDATED_BY] = @MON_CREATED_BY,
				[MON_UPDATED_DATE] = @MON_CREATED_DATE,
				[MON_COMMENTS] = @MON_COMMENTS
				where [MON_ID] = @MonitorID
		end
		
		--insert daily status record for report
		if @MonitorID > 0
		Begin
			select @tempDailyStatusCount = count(*)  from [CSM_MON_DAILY_STATUS] 
			where CONVERT(VARCHAR(10),[MON_TRACK_DATE],101) = CONVERT(VARCHAR(10),@MON_CREATED_DATE,101)
				and [MON_ID] = @MonitorID
			if @tempDailyStatusCount <= 0
			Begin
				exec CWT_InsMonitorDailyStatus 
					@MonitorID 
					,@CONFIG_ID
					,@ENV_ID
					,@MON_CREATED_DATE
					,@MON_STATUS
					,''
			End
		ENd
		/*---Reset email notification flag to true in case of service back to normal or running  */
		if(lower(@MON_STATUS) = 'running' or lower(@MON_STATUS) = 'standby')
		begin
			update [CSM_CONFIGURATION] set [CONFIG_ISNOTIFY] = 'True' 
				where [CONFIG_ID] = @CONFIG_ID
					and [CONFIG_ISNOTIFY_MAIN] = 'True'
			Exec CWT_InsertBuildDetails  @CONFIG_ID,@ENV_ID,@MON_CREATED_DATE,@MON_COMMENTS
		end
	 
	 Exec CWT_InsertCSMLog @SCH_ID, @CONFIG_ID, @ENV_ID,@MON_STATUS, @MON_COMMENTS, @MON_CREATED_DATE, @MON_CREATED_BY
	
  End
  /****** Object:  StoredProcedure [dbo].[CWT_InsUpdScheduler]    Script Date: 2/9/2015 6:17:59 PM ******/
SET ANSI_NULLS ON


GO


