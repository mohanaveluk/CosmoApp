/****** Object:  StoredProcedure [dbo].[CWT_InsMonitorDailyStatus]    Script Date: 10/12/2015 16:25:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_InsMonitorDailyStatus]
(
		@MON_ID int
		,@CONFIG_ID int
		,@ENV_ID int
		,@MON_TRACK_DATE datetime
		,@MON_TRACK_STATUS varchar(200)
		,@MON_TRACK_COMMENTS varchar(MAX)
)
As
Begin

	insert into CSM_MON_DAILY_STATUS 
	(
		[MON_ID]
		,[CONFIG_ID]
		,[ENV_ID]
		,[MON_TRACK_DATE]
		,[MON_TRACK_STATUS]
		,[MON_TRACK_COMMENTS]
	)
	values
	(
		@MON_ID
		,@CONFIG_ID
		,@ENV_ID
		,@MON_TRACK_DATE
		,@MON_TRACK_STATUS
		,@MON_TRACK_COMMENTS
	)	
End	
GO


