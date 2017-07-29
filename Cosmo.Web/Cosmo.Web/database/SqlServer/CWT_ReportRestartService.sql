/****** Object:  StoredProcedure [dbo].[CWT_ReportRestartService]    Script Date: 10/20/2015 21:44:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[CWT_ReportRestartService]
(
	@SCHEDULETYPE varchar(10)
	,@STARTDATE date
	,@ENDDATE date
)
As
Begin
	if lower(@SCHEDULETYPE) = 'all' 
	Begin
		SELECT 
			[CSM_GROUP_SCHEDULE].[GROUP_SCH_ID]
		  ,[CSM_GROUP_SCHEDULE].[GROUP_ID]
		  ,[CSM_GROUP].GROUP_NAME
		  ,[CSM_GROUP_SCHEDULE_DETAIL].ENV_ID
		  ,[CSM_ENVIRONEMENT].ENV_NAME
		  ,[CSM_CONFIGURATION].CONFIG_HOST_IP
		  ,[CSM_CONFIGURATION].CONFIG_PORT_NUMBER
		  ,[CSM_CONFIGURATION].CONFIG_DESCRIPTION
		  ,[CSM_CONFIGURATION].CONFIG_LOCATION
		  ,case [CSM_CONFIGURATION].CONFIG_SERVICE_TYPE when 1 then 'Content Manager' else 'Dispatcher' End CONFIG_SERVICE_TYPE
		  ,CONVERT(VARCHAR(19), [GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
		  ,case [GROUP_SCH_ACTION] when 1 then 'Start' when 2 then 'Stop' else 'Restart' End [GROUP_SCH_ACTION]
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS] 
				when 'O' then 'Upcoming'
				when 'C' then 'Cancelled'
				when 'U' then 'Unsuccessful'
				else 'Completed'
		   end GROUP_SCH_TYPE
		  ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_STATUS] 
				when 'O' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Upcoming'
						when 'C' then 'Cancelled'
						when 'S' then 'Skipped'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'U' then 'Unsuccessful'
				when 'N' then 'No Action'
				when 'T' then 'Timed Out'
				when 'R' then 'Unknown'
				else 'Completed'
		   end GROUP_SCH_DETAIL_TYPE
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS] 
				when 'O' then 'Scheduled'
				when 'C' then 'Cancelled'
				when 'S' then 'Success'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
		   end GROUP_SCH_STATUS
		  ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_STATUS] 
				when 'O' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Scheduled'
						when 'C' then 'Cancelled'
						when 'S' then 'Skipped'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'S' then 'Success'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
		   end GROUP_SCH_DETAIL_STATUS
		  ,[GROUP_SCH_COMPLETED_TIME]
		  ,[GROUP_SCH_COMMENTS]
		  ,[GROUP_SCH_CREATED_BY]
		  ,[GROUP_SCH_CREATED_DATETIME]
		  ,[GROUP_SCH_UPDATED_BY]
		  ,[GROUP_SCH_UPDATED_DATETIME]
		  ,[CSM_USER].USER_FIRST_NAME + ' ' + [CSM_USER].USER_LAST_NAME USERNAME
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_ONDEMAND]
			when 1 then 'Yes'
			else 'No'
		   end [GROUP_SCH_ONDEMAND]
		   ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_RESULT]
				when 'N/A' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'C' then 'Cancelled'
						when 'O' then 'Scheduled'
						when 'S' then 'Skipped'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'Time Out' then 'Timed Out'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
				else [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_RESULT]
			end GROUP_SCH_RESULT
			,[CSM_GROUP_SCHEDULE].[GROUP_SCH_REQUESTSOURCE]	
			,[CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_UPDATEDTIME]
			,[CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_SERVICE_STARTTIME
			,[CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_SERVICE_COMPLETEDTIME
		FROM [CSM_GROUP_SCHEDULE]
		inner join [CSM_GROUP_SCHEDULE_DETAIL] on [CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_ID = [CSM_GROUP_SCHEDULE].GROUP_SCH_ID
		inner join [CSM_GROUP] on [CSM_GROUP].[GROUP_ID] = [CSM_GROUP_SCHEDULE].GROUP_ID
		inner join [CSM_ENVIRONEMENT] on [CSM_ENVIRONEMENT].ENV_ID = [CSM_GROUP_SCHEDULE_DETAIL].ENV_ID
		inner join [CSM_CONFIGURATION] on [CSM_CONFIGURATION].CONFIG_ID =  [CSM_GROUP_SCHEDULE_DETAIL].CONFIG_ID
		inner join [CSM_USER] on [CSM_USER].USER_ID = [CSM_GROUP_SCHEDULE].[GROUP_SCH_CREATED_BY]
		WHERE 
			[CSM_GROUP].GROUP_IS_ACTIVE = 'true'
			and (
				CONVERT(date, CONVERT(VARCHAR(10), [GROUP_SCH_TIME], 101)) between  
				CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)	
				OR
				CONVERT(date, CONVERT(VARCHAR(10), [GROUP_SCH_UPDATEDTIME], 101)) between  
				CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)	
				)
	End
	Else if lower(@SCHEDULETYPE) = 's' or lower(@SCHEDULETYPE) = 'c'
	Begin
		SELECT 
			[CSM_GROUP_SCHEDULE].[GROUP_SCH_ID]
		  ,[CSM_GROUP_SCHEDULE].[GROUP_ID]
		  ,[CSM_GROUP].GROUP_NAME
		  ,[CSM_GROUP_SCHEDULE_DETAIL].ENV_ID
		  ,[CSM_ENVIRONEMENT].ENV_NAME
		  ,[CSM_CONFIGURATION].CONFIG_HOST_IP
		  ,[CSM_CONFIGURATION].CONFIG_PORT_NUMBER
		  ,[CSM_CONFIGURATION].CONFIG_DESCRIPTION
		  ,[CSM_CONFIGURATION].CONFIG_LOCATION
		  ,case [CSM_CONFIGURATION].CONFIG_SERVICE_TYPE when 1 then 'Content Manager' else 'Dispatcher' End CONFIG_SERVICE_TYPE
		  ,CONVERT(VARCHAR(19), [GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
		  ,case [GROUP_SCH_ACTION] when 1 then 'Start' when 2 then 'Stop' else 'Restart' End [GROUP_SCH_ACTION]
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS] 
				when 'O' then 'Upcoming'
				when 'C' then 'Cancelled'
				when 'U' then 'Unsuccessful'
				else 'Completed'
		   end GROUP_SCH_TYPE
		  ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_STATUS] 
				when 'O' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Upcoming'
						when 'S' then 'Skipped'
						when 'C' then 'Cancelled'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'U' then 'Unsuccessful'
				when 'N' then 'No Action'
				when 'T' then 'Timed Out'
				when 'R' then 'Unknown'
				else 'Completed'
		   end GROUP_SCH_DETAIL_TYPE
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS] 
				when 'O' then 'Scheduled'
				when 'C' then 'Cancelled'
				when 'S' then 'Success'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
		   end GROUP_SCH_STATUS
		  ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_STATUS] 
				when 'O' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Scheduled'
						when 'S' then 'Skipped'
						when 'C' then 'Cancelled'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'S' then 'Success'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
		   end GROUP_SCH_DETAIL_STATUS
		  ,[GROUP_SCH_COMPLETED_TIME]
		  ,[GROUP_SCH_COMMENTS]
		  ,[GROUP_SCH_CREATED_BY]
		  ,[GROUP_SCH_CREATED_DATETIME]
		  ,[GROUP_SCH_UPDATED_BY]
		  ,[GROUP_SCH_UPDATED_DATETIME]
		  ,[CSM_USER].USER_FIRST_NAME + ' ' + [CSM_USER].USER_LAST_NAME USERNAME
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_ONDEMAND]
			when 1 then 'Yes'
			else 'No'
		   end [GROUP_SCH_ONDEMAND]
		   ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_RESULT]
				when 'N/A' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Scheduled'
						when 'S' then 'Skipped'
						when 'U' then 'Skipped'
						when 'C' then 'Cancelled'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'Time Out' then 'Timed Out'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
				else [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_RESULT]
			end GROUP_SCH_RESULT
			,[CSM_GROUP_SCHEDULE].[GROUP_SCH_REQUESTSOURCE]	
			,[CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_UPDATEDTIME]
			,[CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_SERVICE_STARTTIME
			,[CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_SERVICE_COMPLETEDTIME
		FROM [CSM_GROUP_SCHEDULE]
		inner join [CSM_GROUP_SCHEDULE_DETAIL] on [CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_ID = [CSM_GROUP_SCHEDULE].GROUP_SCH_ID
		inner join [CSM_GROUP] on [CSM_GROUP].[GROUP_ID] = [CSM_GROUP_SCHEDULE].GROUP_ID
		inner join [CSM_ENVIRONEMENT] on [CSM_ENVIRONEMENT].ENV_ID = [CSM_GROUP_SCHEDULE_DETAIL].ENV_ID
		inner join [CSM_CONFIGURATION] on [CSM_CONFIGURATION].CONFIG_ID =  [CSM_GROUP_SCHEDULE_DETAIL].CONFIG_ID
		inner join [CSM_USER] on [CSM_USER].USER_ID = [CSM_GROUP_SCHEDULE].[GROUP_SCH_CREATED_BY]
		WHERE [CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_STATUS = @SCHEDULETYPE
			and [CSM_GROUP].GROUP_IS_ACTIVE = 'true'
			and (
				CONVERT(date, CONVERT(VARCHAR(10), [GROUP_SCH_TIME], 101)) between  
				CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)	
				OR
				CONVERT(date, CONVERT(VARCHAR(10), [GROUP_SCH_UPDATEDTIME], 101)) between  
				CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)	
				)				
	End
	Else if lower(@SCHEDULETYPE) = 'o'
	Begin
		SELECT 
			[CSM_GROUP_SCHEDULE].[GROUP_SCH_ID]
		  ,[CSM_GROUP_SCHEDULE].[GROUP_ID]
		  ,[CSM_GROUP].GROUP_NAME
		  ,[CSM_GROUP_SCHEDULE_DETAIL].ENV_ID
		  ,[CSM_ENVIRONEMENT].ENV_NAME
		  ,[CSM_CONFIGURATION].CONFIG_HOST_IP
		  ,[CSM_CONFIGURATION].CONFIG_PORT_NUMBER
		  ,[CSM_CONFIGURATION].CONFIG_DESCRIPTION
		  ,[CSM_CONFIGURATION].CONFIG_LOCATION
		  ,case [CSM_CONFIGURATION].CONFIG_SERVICE_TYPE when 1 then 'Content Manager' else 'Dispatcher' End CONFIG_SERVICE_TYPE
		  ,CONVERT(VARCHAR(19), [GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
		  ,case [GROUP_SCH_ACTION] when 1 then 'Start' when 2 then 'Stop' else 'Restart' End [GROUP_SCH_ACTION]
		  		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS] 
				when 'O' then 'Upcoming'
				when 'C' then 'Cancelled'
				when 'U' then 'Unsuccessful'
				else 'Completed'
		   end GROUP_SCH_TYPE
		  ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_STATUS] 
				when 'O' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Upcoming'
						when 'S' then 'Skipped'
						when 'C' then 'Cancelled'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'U' then 'Unsuccessful'
				when 'N' then 'No Action'
				when 'T' then 'Timed Out'
				when 'R' then 'Unknown'
				else 'Completed'
		   end GROUP_SCH_DETAIL_TYPE
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS] 
				when 'C' then 'Cancelled'
				when 'O' then 'Scheduled'
				when 'S' then 'Success'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
		   end GROUP_SCH_STATUS
		  ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_STATUS] 
				when 'O' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Scheduled'
						when 'C' then 'Cancelled'
						when 'S' then 'Skipped'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'S' then 'Success'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
		   end GROUP_SCH_DETAIL_STATUS
		  ,[GROUP_SCH_COMPLETED_TIME]
		  ,[GROUP_SCH_COMMENTS]
		  ,[GROUP_SCH_CREATED_BY]
		  ,[GROUP_SCH_CREATED_DATETIME]
		  ,[GROUP_SCH_UPDATED_BY]
		  ,[GROUP_SCH_UPDATED_DATETIME]
		  ,[CSM_USER].USER_FIRST_NAME + ' ' + [CSM_USER].USER_LAST_NAME USERNAME
		  ,case [CSM_GROUP_SCHEDULE].[GROUP_SCH_ONDEMAND]
			when 1 then 'Yes'
			else 'No'
		   end [GROUP_SCH_ONDEMAND]
		   ,case [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_RESULT]
				when 'N/A' then 
					case [CSM_GROUP_SCHEDULE].[GROUP_SCH_STATUS]
						when 'O' then 'Scheduled'
						when 'S' then 'Skipped'
						when 'C' then 'Cancelled'
						when 'U' then 'Skipped'
						when 'N' then 'No Action'
					else
						'Unknown'
					end
				when 'C' then 'Cancelled'
				when 'Time Out' then 'Timed Out'
				when 'U' then 'Unsuccess'
				when 'N' then 'No Action'
				else [CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_RESULT]
			end GROUP_SCH_RESULT
			,[CSM_GROUP_SCHEDULE].[GROUP_SCH_REQUESTSOURCE]	
			,[CSM_GROUP_SCHEDULE_DETAIL].[GROUP_SCH_UPDATEDTIME]
			,[CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_SERVICE_STARTTIME
			,[CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_SERVICE_COMPLETEDTIME
		FROM [CSM_GROUP_SCHEDULE]
		inner join [CSM_GROUP_SCHEDULE_DETAIL] on [CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_ID = [CSM_GROUP_SCHEDULE].GROUP_SCH_ID
		inner join [CSM_GROUP] on [CSM_GROUP].[GROUP_ID] = [CSM_GROUP_SCHEDULE].GROUP_ID
		inner join [CSM_ENVIRONEMENT] on [CSM_ENVIRONEMENT].ENV_ID = [CSM_GROUP_SCHEDULE_DETAIL].ENV_ID
		inner join [CSM_CONFIGURATION] on [CSM_CONFIGURATION].CONFIG_ID =  [CSM_GROUP_SCHEDULE_DETAIL].CONFIG_ID
		inner join [CSM_USER] on [CSM_USER].USER_ID = [CSM_GROUP_SCHEDULE].[GROUP_SCH_CREATED_BY]
		WHERE [CSM_GROUP_SCHEDULE_DETAIL].GROUP_SCH_STATUS = 'O'--@SCHEDULETYPE
			and [CSM_GROUP].GROUP_IS_ACTIVE = 'true'
			and [GROUP_SCH_TIME] >= Getdate()
			--and CONVERT(date, CONVERT(VARCHAR(10), [GROUP_SCH_TIME], 101)) between  
			--	CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
	End
End


GO