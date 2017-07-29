IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_InsUserAccess]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_InsUserAccess]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[CWT_InsUserAccess]
(
	@ACCESS_CODE [varchar](max)

) As
Begin
	if @ACCESS_CODE is not null
	Begin
		insert into [dbo].[CSM_USER_ACCESS]
		(
			ACCESS_CODE
			,DATE_CREATED
		)
		values
		(
			@ACCESS_CODE
			,GETDATE()
		)
	End
	
End

GO


