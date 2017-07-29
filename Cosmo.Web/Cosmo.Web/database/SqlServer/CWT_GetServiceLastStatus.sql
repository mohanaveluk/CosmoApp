SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

 CREATE procedure [dbo].[CWT_GetServiceLastStatus](
	@SCH_ID int
	,@CONFIG_ID int
	,@ENV_ID int)
	As
  Begin
	Declare 
	@MonitorStatus varchar(100)
	,@MonitorID int
	
	select @MonitorID = max([mon_id]) from [CSM_MONITOR] where SCH_ID = @SCH_ID 
		and CONFIG_ID = @CONFIG_ID 
		and ENV_ID = @ENV_ID
		
	select MON_STATUS from [CSM_MONITOR] where [mon_id] = @MonitorID
	
End	

GO


