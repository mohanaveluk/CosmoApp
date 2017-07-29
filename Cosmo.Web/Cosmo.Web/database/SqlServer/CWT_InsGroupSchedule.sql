SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_InsGroupSchedule]
(
		 @GROUP_SCH_ID int
		,@GROUP_ID int
		,@GROUP_NAME varchar(MAX)
		,@ENV_IDS varchar(MAX)
		,@CONFIG_IDS varchar(MAX)
		,@WIN_SERVICE_IDS varchar(MAX)
		,@GROUP_SCH_ACTION varchar(50)
		,@GROUP_SCH_STATUS varchar(50)
		,@GROUP_SCH_TIME datetime
		,@GROUP_SCH_COMMENTS	varchar(MAX)
		,@GROUP_SCH_CREATED_BY varchar(20)
		,@GROUP_SCH_CREATED_DATETIME datetime
		,@GROUP_SCH_ISACTIVE bit
		,@GROUP_SCH_ONDEMAND bit
		,@GROUP_SCH_COMPLETESTATUS varchar(50)
		,@GROUP_SCH_COMPLETEDTIME datetime
		,@GROUP_SCH_REQUESTSOURCE varchar(50)
		,@GROUP_SCH_SERVICE_STARTTIME datetime
		,@GROUP_SCH_SERVICE_COMPLETEDTIME datetime
		,@SCOPE_OUTPUT int output
)
As 
  Declare
	@Character CHAR(1)
	,@StartIndex INT
	,@EndIndex INT
	,@Input varchar(max)
	,@tempConfigID int
	,@tempGroupDetailsID int
	,@tempGroupScheduleID int
Begin
	set @Character = ','
	set @Input = @CONFIG_IDS
	SET @StartIndex = 1
	 
	--insert / update group
	if @GROUP_ID <= 0 or @GROUP_ID=''
	Begin
		exec CWT_InsGroup		
			@GROUP_ID
			,@GROUP_NAME
			,@GROUP_SCH_CREATED_BY
			,@GROUP_SCH_CREATED_DATETIME
			,'Created using the dynamic page'
			,'true'	
		select @GROUP_ID = grp.GROUP_ID from CSM_GROUP grp where grp.GROUP_NAME = @GROUP_NAME
			
		exec [CWT_InsGroupDetail] 
			@GROUP_ID
			,0
			,@CONFIG_IDS
			,0
			,'Updating group details from dynamic page'
			,@GROUP_SCH_CREATED_BY
			,@GROUP_SCH_CREATED_DATETIME
			,'true'	
				
	End
	
	if @GROUP_ID >0
	Begin
		--updating Group detils table if at all any changes
		exec [CWT_InsGroupDetail] 
			@GROUP_ID
			,0
			,@CONFIG_IDS
			,0
			,'Updating group details from dynamic page'
			,@GROUP_SCH_CREATED_BY
			,@GROUP_SCH_CREATED_DATETIME
			,'true'	


		if(@GROUP_SCH_ID = 0) 
		begin
			select @tempGroupScheduleID = GROUP_SCH_ID from [CSM_GROUP_SCHEDULE] 
			where [GROUP_ID] = @GROUP_ID
				and [GROUP_SCH_STATUS] = 'O'
		End
		else
		Begin
			set @tempGroupScheduleID = @GROUP_SCH_ID
		End
	
		

		if(@tempGroupScheduleID >0)
		Begin
			Update [CSM_GROUP_SCHEDULE] set 
				[GROUP_SCH_TIME] = @GROUP_SCH_TIME
				,[GROUP_SCH_ACTION] = @GROUP_SCH_ACTION
				,[GROUP_SCH_COMMENTS] = @GROUP_SCH_COMMENTS
				,[GROUP_SCH_UPDATED_BY] = @GROUP_SCH_CREATED_BY
				,[GROUP_SCH_UPDATED_DATETIME] = @GROUP_SCH_CREATED_DATETIME
				--,[GROUP_SCH_RESULT] = @GROUP_SCH_COMPLETESTATUS
				Where GROUP_SCH_ID = @tempGroupScheduleID
			set @SCOPE_OUTPUT = @tempGroupScheduleID
		End
		Else
		Begin
			if @GROUP_SCH_ONDEMAND = 'True' or @GROUP_SCH_ONDEMAND = 'true'
			Begin
			Insert into [CSM_GROUP_SCHEDULE]
			(
				  [GROUP_ID]
				  ,[GROUP_SCH_TIME]
				  ,[GROUP_SCH_ACTION]
				  ,[GROUP_SCH_STATUS]
				  ,[GROUP_SCH_COMMENTS]
				  ,[GROUP_SCH_CREATED_BY]
				  ,[GROUP_SCH_CREATED_DATETIME]
				  ,[GROUP_SCH_COMPLETED_TIME]
				  ,[GROUP_SCH_ONDEMAND]
				  ,[GROUP_SCH_REQUESTSOURCE]
			)
			values
			(
					@GROUP_ID
					,@GROUP_SCH_TIME
					,@GROUP_SCH_ACTION
					,@GROUP_SCH_STATUS
					,@GROUP_SCH_COMMENTS	
					,@GROUP_SCH_CREATED_BY
					,@GROUP_SCH_CREATED_DATETIME
					,@GROUP_SCH_COMPLETEDTIME
					,@GROUP_SCH_ONDEMAND
					,@GROUP_SCH_REQUESTSOURCE
			)
			End
			Else
			Begin
			Insert into [CSM_GROUP_SCHEDULE]
			(
				  [GROUP_ID]
				  ,[GROUP_SCH_TIME]
				  ,[GROUP_SCH_ACTION]
				  ,[GROUP_SCH_STATUS]
				  ,[GROUP_SCH_COMMENTS]
				  ,[GROUP_SCH_CREATED_BY]
				  ,[GROUP_SCH_CREATED_DATETIME]
				  ,[GROUP_SCH_ONDEMAND]
				  ,[GROUP_SCH_REQUESTSOURCE]
			)
			values
			(
					@GROUP_ID
					,@GROUP_SCH_TIME
					,@GROUP_SCH_ACTION
					,@GROUP_SCH_STATUS
					,@GROUP_SCH_COMMENTS	
					,@GROUP_SCH_CREATED_BY
					,@GROUP_SCH_CREATED_DATETIME
					,@GROUP_SCH_ONDEMAND
					,@GROUP_SCH_REQUESTSOURCE
			)
			End
			set @SCOPE_OUTPUT = IDENT_CURRENT('CSM_GROUP_SCHEDULE')
			set @tempGroupScheduleID = @SCOPE_OUTPUT
		End
		
		if @CONFIG_IDS <> ''
		Begin	
			update [CSM_GROUP_SCHEDULE_DETAIL] set [GROUP_SCH_ISACTIVE] = 'false'
			where [GROUP_ID] = @GROUP_ID
			and [GROUP_SCH_ID] = @tempGroupScheduleID
			and [GROUP_SCH_STATUS] = 'O'
			--IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
			--BEGIN
			--	SET @Input = @Input + @Character
			--END	
			
			WHILE CHARINDEX(@Character, @Input) > 0
			Begin
				set @tempGroupDetailsID = 0
				SET @EndIndex = CHARINDEX(@Character, @Input)
				set @tempConfigID = SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
				--Check whether the current value is there in the tavle already or not
				select @tempGroupDetailsID = GROUP_SERVICE_SCH_ID from [CSM_GROUP_SCHEDULE_DETAIL] 
					where CONFIG_ID = @tempConfigID
						and [GROUP_ID] = @GROUP_ID
						and [GROUP_SCH_STATUS] = 'O'
						and [GROUP_SCH_ID] = @tempGroupScheduleID

				if @tempGroupDetailsID > 0 
				Begin
					update [CSM_GROUP_SCHEDULE_DETAIL] set [GROUP_SCH_ISACTIVE] = 'true'
						where GROUP_SERVICE_SCH_ID = @tempGroupDetailsID
				End
				else
				Begin
					Insert into [CSM_GROUP_SCHEDULE_DETAIL]
					(
						GROUP_SCH_ID
						,GROUP_ID
						,ENV_ID
						,CONFIG_ID
						,GROUP_SCH_ISACTIVE
						,GROUP_SCH_STATUS
						,GROUP_SCH_UPDATEDTIME
						,GROUP_SCH_RESULT
						,GROUP_SCH_SERVICE_STARTTIME
						,GROUP_SCH_SERVICE_COMPLETEDTIME
					)
					values
					(
						--(select isnull(MAX(GROUP_SCH_ID),1) from dbo.CSM_GROUP_SCHEDULE)
						@tempGroupScheduleID
						,@GROUP_ID
						,(select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = @tempConfigID)
						,@tempConfigID
						,@GROUP_SCH_ISACTIVE
						,@GROUP_SCH_STATUS
						,@GROUP_SCH_CREATED_DATETIME
						,@GROUP_SCH_COMPLETESTATUS
						,Case @GROUP_SCH_ONDEMAND when 'True' then @GROUP_SCH_SERVICE_STARTTIME else null end 
						,Case @GROUP_SCH_ONDEMAND when 'True' then @GROUP_SCH_SERVICE_COMPLETEDTIME else null end 
					)
				End
				SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
				
			End
		End
	End
End


GO


