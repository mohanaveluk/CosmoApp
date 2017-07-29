
/****** Object:  StoredProcedure [dbo].[CWT_InsUpdUser]    Script Date: 09/24/2015 22:51:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsUpdUser]
( 
	@USERID int
	,@USERFIRSTNAME varchar(100)
	,@USERLASTNAME  varchar(100)
	,@USEREMAIL  varchar(100)
	,@USERPASSWORD varchar(MAX)
	,@USERROLES varchar(MAX)
	,@ISACTIVE bit
	--,@ISDELETED bit
	,@SCOPE_OUTPUT int output
) As
Declare
	@UserCount int
	,@tempUserId int
Begin
	Begin try
	If @USERID is null or @USERID <= 0
	Begin
		select @UserCount = COUNT(USER_EMAIL_ADDRESS)
		from CSM_USER 
		where CSM_USER.USER_EMAIL_ADDRESS = @USEREMAIL
			and USER_IS_ACTIVE = 'True'
		if @UserCount > 0
		Begin
			set @SCOPE_OUTPUT = -1
		End
		else
		Begin
			Insert into CSM_USER 
			(
				USER_LOGIN_ID
				,USER_FIRST_NAME
				,USER_LAST_NAME
				,USER_EMAIL_ADDRESS
				,USER_PASSWORD
				,USER_IS_ACTIVE
				--,USER_IS_DELETED
			)
			values
			(
				@USEREMAIL
				,@USERFIRSTNAME
				,@USERLASTNAME
				,@USEREMAIL
				,@USERPASSWORD
				,@ISACTIVE
				--,@ISDELETED
				
			)
			set @tempUserId = IDENT_CURRENT('CSM_USER')
			exec CWT_InsUpdUserRoles @tempUserId, @USERROLES
			insert into CSM_POERSONALIZE ([User_ID], [PERS_DB_REFRESHTIME], [PERS_ISACTIVE], [PERS_CREATEDDATE], [PERS_CREATEDBY]) values(@tempUserId,2,1,GETDATE(),1)
			set @SCOPE_OUTPUT = 1 --IDENT_CURRENT('CSM_USER')
		end
	End
	Else if @USERID is not null or @USERID > 0
	Begin
		Update CSM_USER set 
			USER_LOGIN_ID = @USEREMAIL
			,USER_FIRST_NAME = @USERFIRSTNAME
			,USER_LAST_NAME = @USERLASTNAME
			,USER_EMAIL_ADDRESS = @USEREMAIL
			,USER_PASSWORD = @USERPASSWORD
			,USER_IS_ACTIVE		= @ISACTIVE
			--,USER_IS_DELETED = @ISDELETED
		where [USER_ID] = @USERID
		
		set @tempUserId = @USERID
		exec CWT_InsUpdUserRoles @tempUserId, @USERROLES
		set @SCOPE_OUTPUT = 2
	End
	
	End try
	
	Begin catch
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

			DECLARE @ErrorNumber INT = ERROR_NUMBER();
			DECLARE @ErrorLine INT = ERROR_LINE();
			DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
			DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
			DECLARE @ErrorState INT = ERROR_STATE();

			PRINT 'Actual error number: ' + CAST(@ErrorNumber AS VARCHAR(10));
			PRINT 'Actual line number: ' + CAST(@ErrorLine AS VARCHAR(10));

			RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);		
	End catch
End

GO


