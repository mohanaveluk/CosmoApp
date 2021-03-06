SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsMailServer]
(
	@ServerName varchar(200)
	,@ServerPort varchar(10)
	,@Username varchar(100)
	,@Password varchar(200)
	,@SSLEnable bit
	,@IsActive bit
	,@FromEmailId varchar(100)
)
As
	declare 
		@EMSILSERVERID int
Begin
	set @EMSILSERVERID = 0
	select @EMSILSERVERID = [EMAIL_SERVER_ID] 
		FROM [dbo].[CSM_EMAIL_CONFIGURATION]
		WHERE [EMAIL_SERVER_NAME] = @ServerName
		and [EMAIL_AUTH_USER_ID] = @Username
	IF @EMSILSERVERID <= 0 or @EMSILSERVERID is null
	BEGIN
		delete from [dbo].[CSM_EMAIL_CONFIGURATION]
		INSERT INTO [dbo].[CSM_EMAIL_CONFIGURATION] 
		(
		  [EMAIL_SERVER_NAME]
		  ,[EMAIL_SERVER_IPADDRESS]
		  ,[EMAIL_SERVER_PORT]
		  ,[EMAIL_AUTH_USER_ID]
		  ,[EMAIL_AUTH_USER_PWD]
		  ,[EMAIL_SSL_ENABLE]
		  ,[EMAIL_IS_ACTIVE]
		  ,[EMAIL_CREATED_BY]
		  ,[EMAIL_CREATED_DATE]
		  ,[EMAIL_ADMIN_MAILADRESS]
		)
		values
		(
			@ServerName
			,@ServerName
			,@ServerPort
			,@Username
			,@Password
			,@SSLEnable
			,@IsActive
			,'Admin'
			,GETDATE()
			,@FromEmailId
		)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[CSM_EMAIL_CONFIGURATION] set
		  [EMAIL_SERVER_NAME] = @ServerName
		  ,[EMAIL_SERVER_IPADDRESS] = @ServerName
		  ,[EMAIL_SERVER_PORT] = @ServerPort
		  ,[EMAIL_AUTH_USER_ID] = @Username
		  ,[EMAIL_AUTH_USER_PWD] = @Password
		  ,[EMAIL_SSL_ENABLE] = @SSLEnable
		  ,[EMAIL_IS_ACTIVE] = @IsActive
		  ,[EMAIL_UPDATED_DATE] = GETDATE()
		  ,[EMAIL_ADMIN_MAILADRESS] = @FromEmailId
		WHERE [EMAIL_SERVER_ID] = @EMSILSERVERID
	END
End

GO
