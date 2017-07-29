/****** Object:  StoredProcedure [dbo].[CWT_UpdateGroupScheduleStatus]    Script Date: 08/09/2015 00:09:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_UpdateGroupScheduleStatus]
(
	@GROUP_SCH_ID int
	,@GROUP_SCH_STATUS char(1)
	,@GROUP_SCH_COMPLETED_TIME datetime
	,@GROUP_SCH_UPDATED_BY varchar(10)
	,@GROUP_SCH_UPDATED_DATETIME datetime
)
As
Begin

	if @GROUP_SCH_COMPLETED_TIME <> ''
	Begin
		UPDATE [CSM_GROUP_SCHEDULE] set 
			[GROUP_SCH_STATUS] = @GROUP_SCH_STATUS
			,[GROUP_SCH_COMPLETED_TIME] = @GROUP_SCH_COMPLETED_TIME
			,[GROUP_SCH_UPDATED_BY] = @GROUP_SCH_UPDATED_BY
			,[GROUP_SCH_UPDATED_DATETIME] = @GROUP_SCH_UPDATED_DATETIME
			Where [GROUP_SCH_ID] = @GROUP_SCH_ID
	End
	Else
	Begin
		UPDATE [CSM_GROUP_SCHEDULE] set 
			[GROUP_SCH_STATUS] = @GROUP_SCH_STATUS
			,[GROUP_SCH_UPDATED_BY] = @GROUP_SCH_UPDATED_BY
			,[GROUP_SCH_UPDATED_DATETIME] = @GROUP_SCH_UPDATED_DATETIME
			Where [GROUP_SCH_ID] = @GROUP_SCH_ID
	End

End
GO


