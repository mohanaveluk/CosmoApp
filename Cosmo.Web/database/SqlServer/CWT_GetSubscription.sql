
/****** Object:  StoredProcedure [dbo].[CWT_GetSubscription]    Script Date: 09/13/2016 22:12:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_GetSubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_GetSubscription]
GO


/****** Object:  StoredProcedure [dbo].[CWT_GetSubscription]    Script Date: 09/13/2016 22:12:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetSubscription](@ID int)
As
Begin
	If @ID > 0
		Begin
			select 
				s.SUBSCRIPTION_ID
				,s.SUBSCRIPTION_TYPE
				,s.SUBSCRIPTION_TIME
				,s.SUBSCRIPTION_ISACTIVE
				,s.CREATED_BY
				,u.USER_FIRST_NAME + ' ' + u.USER_LAST_NAME as CREATEDBY_NAME
				,s.CREATED_DATE
				,s.UPDATED_BY
				,u.USER_FIRST_NAME + ' ' + u.USER_LAST_NAME as UPDATEDBY_NAME
				,s.UPDATED_DATE
				,s.SCH_LASTJOBRAN_TIME
				,s.SCH_NEXTJOBRAN_TIME
			from [dbo].[CSM_REPORT_SUBSCRIPTION] s
			left join CSM_USER u on u.USER_ID = s.CREATED_BY
			left join CSM_USER uu on uu.USER_ID = s.UPDATED_BY
			Where s.SUBSCRIPTION_ID = @ID
		End
	Else
		Begin
			select 
				s.SUBSCRIPTION_ID
				,s.SUBSCRIPTION_TYPE
				,s.SUBSCRIPTION_TIME
				,s.SUBSCRIPTION_ISACTIVE
				,s.CREATED_BY
				,u.USER_FIRST_NAME + ' ' + u.USER_LAST_NAME as CREATEDBY_NAME
				,s.CREATED_DATE
				,s.UPDATED_BY
				,u.USER_FIRST_NAME + ' ' + u.USER_LAST_NAME as UPDATEDBY_NAME
				,s.UPDATED_DATE
				,s.SCH_LASTJOBRAN_TIME
				,s.SCH_NEXTJOBRAN_TIME
			from [dbo].[CSM_REPORT_SUBSCRIPTION] s
			left join CSM_USER u on u.USER_ID = s.CREATED_BY
			left join CSM_USER uu on uu.USER_ID = s.UPDATED_BY
		End
End	

GO


