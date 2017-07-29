/****** Object:  StoredProcedure [dbo].[CWT_UpdateUserPassword]    Script Date: 10/04/2015 14:04:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_UpdateUserPassword]
(
	@USERID int
	,@USEREMAIL varchar(200)
	,@CURRENTPASSWORD varchar(MAX)
	,@NEWPASSWORD varchar(MAX)
	,@SCOPE_OUTPUT int output
)
As
Declare
	@tempUserCount int
Begin
	if @USERID >= 1
	Begin
		select @tempUserCount = COUNT(*) from [CSM_USER] 
		Where [USER_ID] = @USERID
			and [USER_PASSWORD] = @CURRENTPASSWORD
			and [USER_EMAIL_ADDRESS] = @USEREMAIL
		if @tempUserCount >=1
		Begin
			Update [CSM_USER] set 
				[USER_PASSWORD] = @NEWPASSWORD
			Where [USER_ID] = @USERID
				and [USER_EMAIL_ADDRESS] = @USEREMAIL
			set @SCOPE_OUTPUT = 1
		End
		Else
		Begin
			set @SCOPE_OUTPUT = 0
		End
	End
	Else
	Begin
		set @SCOPE_OUTPUT = -1
	End
End

GO


