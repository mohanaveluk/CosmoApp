IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_GetUserAccess]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_GetUserAccess]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[CWT_GetUserAccess]
As
Begin
	Select top 1
		[ACCESS_ID]
		,[ACCESS_CODE]
		,[ACCESS_FIRSTNAME] 
		,[ACCESS_LASTTNAME] 
		,[ACCESS_EMAIL] 
		,[ACCESS_MOBILE]
		,[DATE_CREATED] 
	From [dbo].[CSM_USER_ACCESS] 
	Order by [ACCESS_ID] Desc
End


GO


