/****** Object:  StoredProcedure [dbo].[CWT_GetLogin]    Script Date: 09/24/2015 22:45:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetLogin]
(
	@USERID varchar(200)
	,@PASSWORD varchar(MAX)
	,@SCOPE_OUTPUT int output
)
As
Declare
	@tempUserCount int
	,@tempUser int
Begin
	if @USERID is not null and @USERID <> ''
	Begin
		Select @tempUserCount = count(*)  from [CSM_USER] cuser 
			WHERE [USER_IS_ACTIVE] = 'true'
			  and 	USER_EMAIL_ADDRESS = @USERID
		if @tempUserCount > 0
		Begin 
			SELECT @tempUser = cuser.USER_ID 
			FROM [CSM_USER] cuser
			  WHERE [USER_IS_ACTIVE] = 'true'
			  and USER_IS_DELETED = 'false'
			  and 	USER_EMAIL_ADDRESS = @USERID 
			  and USER_PASSWORD = @PASSWORD
			if @tempUser > 0
			Begin  
			/*
				SELECT  cuser.[USER_ID]
					  ,[USER_LOGIN_ID]
					  ,[USER_FIRST_NAME]
					  ,[USER_LAST_NAME]
					  ,[USER_EMAIL_ADDRESS]
					  ,[USER_ROLE]
					  ,[USER_PASSWORD]
					  ,[USER_IS_LDAP_USER]
					  ,[USER_IS_ACTIVE]
					  ,[USER_ATTEMPS]
					  ,[USER_ISLOCKED]
					  ,[USER_LOCKEDTIME]
					  ,cuser.[USER_CREATED_BY]
					  ,cuser.[USER_CREATED_DATE]
					  ,cuser.[USER_UPDATED_BY]
					  ,cuser.[USER_UPDATED_DATE]
					  ,cuser.[USER_COMMENTS]
				FROM [CSM_USER] cuser
				  WHERE [USER_IS_ACTIVE] = 'true'
				  and 	USER_EMAIL_ADDRESS = @USERID  
				  and USER_PASSWORD = @PASSWORD
			*/
				  set @SCOPE_OUTPUT = @tempUser --valid user
			End
			else
			Begin
				set @SCOPE_OUTPUT = -1 --Password mismatch not present
			End
		End
		else
		Begin
			set @SCOPE_OUTPUT = 0 --user not present
		End
	End	  
End




