

/****** Object:  StoredProcedure [dbo].[CWT_GetSubscriptionDetail]    Script Date: 09/13/2016 22:13:16 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_GetSubscriptionDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_GetSubscriptionDetail]
GO


/****** Object:  StoredProcedure [dbo].[CWT_GetSubscriptionDetail]    Script Date: 09/13/2016 22:13:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetSubscriptionDetail](@ID int)
As
Begin
	If @ID > 0
	Begin
		select 
			s.SUBSCRIPTION_DETAIL_ID
			,s.SUBSCRIPTION_ID
			,s.SUBSCRIPTION_EMAILID
			,s.USRLST_ID
			,s.SUBSCRIPTION_ISACTIVE
		from [dbo].[CSM_REPORT_SUBSCRIPTION_DETAILS] s
		where s.SUBSCRIPTION_ID = @ID
	End
End		
GO


