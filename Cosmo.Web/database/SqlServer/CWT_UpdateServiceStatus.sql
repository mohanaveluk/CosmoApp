SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_UpdateServiceStatus]
(
	@GROUP_SERVICE_SCH_ID int
	,@GROUP_SERVICE_SCH_STATUS char(1)
	,@GROUP_SCH_UPDATEDTIME datetime
	,@GROUP_SCH_SERVICE_STARTTIME datetime
	,@GROUP_SCH_SERVICE_COMPLETEDTIME datetime
	--,@GROUP_SCH_COMPLETESTATUS varchar(50)
)
As
Declare
	@tempCompleteStatus varchar(50)
Begin
	if @GROUP_SERVICE_SCH_STATUS = 'S'
		set @tempCompleteStatus = 'Successful'
	else if @GROUP_SERVICE_SCH_STATUS = 'T'
		set @tempCompleteStatus = 'Timed out'
	else if @GROUP_SERVICE_SCH_STATUS = 'C'
		set @tempCompleteStatus = 'Cancelled'
	else if @GROUP_SERVICE_SCH_STATUS = 'A'
		set @tempCompleteStatus = 'Abonded'
	else if @GROUP_SERVICE_SCH_STATUS = 'U'
		set @tempCompleteStatus = 'Unsuccessful'		
	else if @GROUP_SERVICE_SCH_STATUS = 'N'
		set @tempCompleteStatus = 'Skipped'
	else
		set @tempCompleteStatus = 'Nothing'

	if @GROUP_SCH_SERVICE_STARTTIME <> ''
	Begin
		UPDATE [CSM_GROUP_SCHEDULE_DETAIL] set 
			[GROUP_SCH_STATUS] = @GROUP_SERVICE_SCH_STATUS
			,[GROUP_SCH_UPDATEDTIME] = GETDATE()
			,[GROUP_SCH_RESULT] = @tempCompleteStatus
			,[GROUP_SCH_SERVICE_STARTTIME] = @GROUP_SCH_SERVICE_STARTTIME
			,[GROUP_SCH_SERVICE_COMPLETEDTIME] = @GROUP_SCH_SERVICE_COMPLETEDTIME
			--,[ENV_ID] = @ENV_ID
			Where [GROUP_SERVICE_SCH_ID] = @GROUP_SERVICE_SCH_ID
	End
	Else
	Begin
		UPDATE [CSM_GROUP_SCHEDULE_DETAIL] set 
			[GROUP_SCH_STATUS] = @GROUP_SERVICE_SCH_STATUS
			,[GROUP_SCH_RESULT] = @tempCompleteStatus
			--,[ENV_ID] = @ENV_ID
			Where [GROUP_SERVICE_SCH_ID] = @GROUP_SERVICE_SCH_ID
	End

End


GO