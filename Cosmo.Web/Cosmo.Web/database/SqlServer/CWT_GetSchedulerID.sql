/****** Object:  StoredProcedure [dbo].[CWT_GetSchedulerID]    Script Date: 2/9/2015 6:14:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE  procedure [dbo].[CWT_GetSchedulerID](@ENV_ID int, @CONFIG_ID int) AS
Begin
	Declare
		@SCHID int
		Begin
		select @SCHID = [SCH_ID] from [CSM_SCHEDULE] where
		[ENV_ID] = @ENV_ID AND
		[CONGIG_ID]	= @CONFIG_ID
		
		if rtrim(ltrim(@SCHID)) = '' or @SCHID <= 0 
			print   'Null'
		else
			Print  replace(@SCHID, ' ', '')
		End 
End
