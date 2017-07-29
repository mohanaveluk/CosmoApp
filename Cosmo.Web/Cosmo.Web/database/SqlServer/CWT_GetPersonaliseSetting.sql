SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[CWT_GetPersonaliseSetting] 
(
	@User_ID int
)
as 		
Begin
	select 
		PERS_ID
		,PERS_DB_REFRESHTIME 
		,Case when 
			PERS_UPDATEDDATE = null then PERS_CREATEDDATE 
			else
			PERS_CREATEDDATE
		End 'PERS_UPDATEDDATE'
		from  CSM_POERSONALIZE
		Where PERS_ISACTIVE = 1
			AND User_ID = @User_ID
End
GO


