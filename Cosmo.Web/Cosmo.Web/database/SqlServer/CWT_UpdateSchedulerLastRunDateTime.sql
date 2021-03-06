
/****** Object:  StoredProcedure [dbo].[CWT_UpdateSchedulerLastRunDateTime]    Script Date: 2/9/2015 6:18:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_UpdateSchedulerLastRunDateTime] (
	@SchedulerID int,
	@DateLastJobRun datetime
)
As 
Begin
	Declare
	@currentDateTime varchar(100)
	set @currentDateTime = CONVERT(VARCHAR(19), @DateLastJobRun, 120)
	
	update CSM_SCHEDULE set SCH_LASTJOBRAN_TIME = @currentDateTime, SCH_NEXTJOBRAN_TIME = 
	( Case
		When (lower(SCH_DURATION) = 'seconds' or lower(SCH_DURATION) = 'second' or lower(SCH_DURATION) = 'sec' or lower(SCH_DURATION) = 's') then dateadd(SECOND, SCH_INTERVAL,@currentDateTime)
		When (lower(SCH_DURATION) = 'minutes' or lower(SCH_DURATION) = 'minute' or lower(SCH_DURATION) = 'min' or lower(SCH_DURATION) = 'm') then dateadd(MINUTE, SCH_INTERVAL,@currentDateTime)
		When (lower(SCH_DURATION) = 'hours' or lower(SCH_DURATION) = 'hour' or lower(SCH_DURATION) = 'hr' or lower(SCH_DURATION) = 'h') then dateadd(HOUR, SCH_INTERVAL,@currentDateTime)
		When (lower(SCH_DURATION) = 'days' or lower(SCH_DURATION) = 'day' or lower(SCH_DURATION) = 'dy' or lower(SCH_DURATION) = 'd') then dateadd(DAY, SCH_INTERVAL,@currentDateTime)
		When (lower(SCH_DURATION) = 'weeks' or lower(SCH_DURATION) = 'week' or lower(SCH_DURATION) = 'wk' or lower(SCH_DURATION) = 'w') then dateadd(WEEK, SCH_INTERVAL,@currentDateTime)
		When (lower(SCH_DURATION) = 'months' or lower(SCH_DURATION) = 'month' or lower(SCH_DURATION) = 'mn' or lower(SCH_DURATION) = 'm') then dateadd(MONTH, SCH_INTERVAL,@currentDateTime)
	   End
	)
	where SCH_ID = @SchedulerID
End	