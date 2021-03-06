/****** Object:  StoredProcedure [dbo].[CWT_SetServiceAcknowledge]    Script Date: 2/9/2015 6:18:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_SetServiceAcknowledge]
(
	@ENV_ID int,
	@CONFIG_ID int,
	@MON_ID int,
	@ACK_ISACKNOWLEDGE bit,
	@ACK_ALERT varchar(50),
	@ACK_COMMENTS varchar(50),
	@CREATED_BY varchar(50),
	@CREATED_DATE datetime
) As
declare
	@SCH_ID int
	,@ENDDATETIME varchar(50)
	,@CONFIG_DISP_ID int
	,@comments varchar(1000)
Begin
	
	select @CONFIG_DISP_ID = CONFIG_ID from CSM_CONFIGURATION where CONFIG_REF_ID = @CONFIG_ID and CONFIG_IS_ACTIVE = 'True'
	insert into [CSM_ACKNOWLEDGE] 
	(
       [ENV_ID]
      ,[CONFIG_ID]
      ,[ACK_ISACKNOWLEDGE]
      ,[ACK_ALERT]
      ,[ACK_COMMENTS]
      ,[CREATED_BY]
      ,[CREATED_DATE]
	)
	values
	(
		@ENV_ID,
		@CONFIG_ID ,
		@ACK_ISACKNOWLEDGE,
		@ACK_ALERT ,
		@ACK_COMMENTS ,
		@CREATED_BY ,
		@CREATED_DATE 
	)
	
	if @CONFIG_DISP_ID > 0
	Begin
		insert into [CSM_ACKNOWLEDGE] 
		(
		   [ENV_ID]
		  ,[CONFIG_ID]
		  ,[ACK_ISACKNOWLEDGE]
		  ,[ACK_ALERT]
		  ,[ACK_COMMENTS]
		  ,[CREATED_BY]
		  ,[CREATED_DATE]
		)
		values
		(
			@ENV_ID,
			@CONFIG_DISP_ID ,
			@ACK_ISACKNOWLEDGE,
			@ACK_ALERT ,
			@ACK_COMMENTS ,
			@CREATED_BY ,
			@CREATED_DATE 
		)
		End
  
  	if LOWER(@ACK_ALERT) = 'stop'
	Begin
		Update [CSM_CONFIGURATION] set [CONFIG_ISNOTIFY] = 'False', CONFIG_IS_MONITORED = 'False' 
			where [CONFIG_ID] = @CONFIG_ID or [CONFIG_REF_ID] = @CONFIG_ID
	End
	else if LOWER(@ACK_ALERT) = 'start'
	Begin
		Update [CSM_CONFIGURATION] set [CONFIG_ISNOTIFY] = 'True', CONFIG_IS_MONITORED = 'True' 
			where [CONFIG_ID] = @CONFIG_ID or [CONFIG_REF_ID] = @CONFIG_ID
	End
	
	if LOWER(@ACK_COMMENTS) =  'scheduled service operation' OR LOWER(@ACK_COMMENTS) =  'ondemand service operation' 
	Begin
		select @SCH_ID = SCH_ID from [dbo].[CSM_SCHEDULE] where ENV_ID = @ENV_ID and CONGIG_ID = @CONFIG_ID
		select @ENDDATETIME = datename(dw,getdate()) + ', ' + CONVERT(VARCHAR(26), GETDATE(), 109)
		select @CONFIG_DISP_ID = CONFIG_ID from CSM_CONFIGURATION where CONFIG_REF_ID = @CONFIG_ID and CONFIG_IS_ACTIVE = 'True'
		
		set @comments = 'Requested to Stop service through ' + @ACK_COMMENTS
		
		if LOWER(@ACK_ALERT) = 'stop'
		Begin
			--Update [CSM_CONFIGURATION] set CONFIG_IS_MONITORED = 'False' where [CONFIG_ID] = @CONFIG_ID or [CONFIG_REF_ID] = @CONFIG_ID

			exec CWT_SetMonitorStatus 
						@SCH_ID, 
						@CONFIG_ID, 
						@ENV_ID,
						'Stopped', 
						'', 
						@ENDDATETIME, 
						'True', 
						@CREATED_BY, 
						@CREATED_DATE, 
						@comments
						
				if @CONFIG_DISP_ID > 0
				Begin
					select @SCH_ID = SCH_ID from [dbo].[CSM_SCHEDULE] where ENV_ID = @ENV_ID and CONGIG_ID = @CONFIG_DISP_ID
					exec CWT_SetMonitorStatus 
								@SCH_ID, 
								@CONFIG_DISP_ID, 
								@ENV_ID,
								'Stopped', 
								'', 
								@ENDDATETIME, 
								'True', 
								@CREATED_BY, 
								@CREATED_DATE, 
								@comments
				End

		End
		--if LOWER(@ACK_ALERT) = 'start'
		--Begin
			--Update [CSM_CONFIGURATION] set CONFIG_IS_MONITORED = 'True' where [CONFIG_ID] = @CONFIG_ID or [CONFIG_REF_ID] = @CONFIG_ID
		--End
	End
	if @MON_ID > 0
	Begin
		update [CSM_MONITOR] set MON_ISACKNOWLEDGE = 'True'  where MON_ID = @MON_ID 
		update [CSM_MONITOR] set MON_ISACKNOWLEDGE = 'True'  where MON_ID in 
			(select MON_ID from CSM_MONITOR where CONFIG_ID = @CONFIG_DISP_ID and MON_ID > @MON_ID)
	End

End
SET ANSI_NULLS ON

GO

GO


