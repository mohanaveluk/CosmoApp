
/****** Object:  StoredProcedure [dbo].[CWT_SetSubscriptionNextJobRunTime]    Script Date: 09/13/2016 22:14:39 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_SetSubscriptionNextJobRunTime]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_SetSubscriptionNextJobRunTime]
GO

/****** Object:  StoredProcedure [dbo].[CWT_SetSubscriptionNextJobRunTime]    Script Date: 09/13/2016 22:14:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[CWT_SetSubscriptionNextJobRunTime]
(
	@ID int
	,@LASTJOBRANTIME datetime
	,@NEXTJOBRUNTIME datetime
)
As 
Begin
	update [dbo].[CSM_REPORT_SUBSCRIPTION] set
	SCH_LASTJOBRAN_TIME = @LASTJOBRANTIME
	,SCH_NEXTJOBRAN_TIME = @NEXTJOBRUNTIME
	where [SUBSCRIPTION_ID] = @ID
End
GO


