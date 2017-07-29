
/****** Object:  StoredProcedure [dbo].[CWT_GetUserRole]    Script Date: 09/24/2015 22:49:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE procedure [dbo].[CWT_GetUserRole](@USERID int) As
Begin
	if @USERID is null or @USERID <=0
	Begin
		SELECT cuser.[USER_ID]
			,curole.USER_ROLE_ID
			,curole.ROLE_ID
			,roles.ROLE_NAME
		  FROM [CSM_USER] cuser
		  inner join [CSM_USER_ROLE] curole on cuser.USER_ID = curole.USER_ID
		  inner join [CSM_ROLES] roles on roles.ROLE_ID = curole.ROLE_ID
		  WHERE curole.USER_ROLE_ISACTIVE = 'true'
			-- [USER_IS_ACTIVE] = 'true'
			--and curole.USER_ROLE_ISACTIVE = 'true'
			--and roles.ROLE_ISACTIVE = 'true'
	End
	Else
	Begin
		SELECT cuser.[USER_ID]
			,curole.USER_ROLE_ID
			,curole.ROLE_ID
			,roles.ROLE_NAME
		  FROM [CSM_USER] cuser
		  inner join [CSM_USER_ROLE] curole on cuser.USER_ID = curole.USER_ID
		  inner join [CSM_ROLES] roles on roles.ROLE_ID = curole.ROLE_ID
		  WHERE cuser.USER_ID = @USERID
			and curole.USER_ROLE_ISACTIVE = 'true'
			--[USER_IS_ACTIVE] = 'true'
			--and roles.ROLE_ISACTIVE = 'true'
			
	ENd
End
  
GO


