SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_InsertPortalStatus]
(
	@URL_ID int
	,@ENV_ID int
	,@PMON_STATUS varchar(20)
	,@PMON_MESSAGE varchar(max)
	,@PMON_RESPONSETIME int
	,@PMON_EXCEPTION varchar(max)
)
As

Begin
	if @URL_ID > 0
	Begin
		insert into [dbo].CSM_PORTALMONITOR
		(
			[URL_ID]
			,[ENV_ID]
			,[PMON_STATUS]
			,[PMON_MESSAGE]
			,[PMON_RESPONSETIME]
			,[PMON_EXCEPTION]
		)
		values
		(
			@URL_ID
			,@ENV_ID
			,@PMON_STATUS
			,@PMON_MESSAGE
			,@PMON_RESPONSETIME
			,@PMON_EXCEPTION
		)
	End
End
GO


