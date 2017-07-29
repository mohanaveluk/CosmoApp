
/****** Object:  StoredProcedure [dbo].[CWT_GetUsers]    Script Date: 09/24/2015 22:50:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetUsers](@USERID int)   AS
Begin
	if @USERID is null or @USERID <=0
	Begin
	  SELECT cuser.[USER_ID]
		  ,[USER_LOGIN_ID]
		  ,[USER_FIRST_NAME]
		  ,[USER_LAST_NAME]
		  ,[USER_EMAIL_ADDRESS]
		  ,[USER_ROLE]
		  ,[USER_PASSWORD]
		  ,[USER_IS_LDAP_USER]
		  ,[USER_IS_ACTIVE]
		  ,[USER_IS_DELETED]
		  ,[USER_ATTEMPS]
		  ,[USER_ISLOCKED]
		  ,[USER_LOCKEDTIME]
		  ,cuser.[USER_CREATED_BY]
		  ,cuser.[USER_CREATED_DATE]
		  ,cuser.[USER_UPDATED_BY]
		  ,cuser.[USER_UPDATED_DATE]
		  ,cuser.[USER_COMMENTS]
	  FROM [CSM_USER] cuser
	  WHERE [USER_IS_DELETED] = 'false'
			and [USER_EMAIL_ADDRESS] <> 'admin@cosmo.com'
	End
	else
	Begin
	  SELECT cuser.[USER_ID]
		  ,[USER_LOGIN_ID]
		  ,[USER_FIRST_NAME]
		  ,[USER_LAST_NAME]
		  ,[USER_EMAIL_ADDRESS]
		  ,[USER_ROLE]
		  ,[USER_PASSWORD]
		  ,[USER_IS_LDAP_USER]
		  ,[USER_IS_ACTIVE]
		  ,[USER_IS_DELETED]
		  ,[USER_ATTEMPS]
		  ,[USER_ISLOCKED]
		  ,[USER_LOCKEDTIME]
		  ,cuser.[USER_CREATED_BY]
		  ,cuser.[USER_CREATED_DATE]
		  ,cuser.[USER_UPDATED_BY]
		  ,cuser.[USER_UPDATED_DATE]
		  ,cuser.[USER_COMMENTS]
	  FROM [CSM_USER] cuser
	  WHERE [USER_IS_DELETED] = 'false'
			and cuser.USER_ID = @USERID
			--and [USER_EMAIL_ADDRESS] <> 'admin@cosmo.com'
	End
End

GO


