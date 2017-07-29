/****** Object:  StoredProcedure [dbo].[CWT_InsUpdWindowsService]    Script Date: 08/23/2015 19:02:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsUpdWindowsService]
(
	@ENV_ID int,
	@CONFIG_ID int,
	@SERVICENAME varchar(200),
	@COMMENTS varchar(2000),
	@CREATED_BY varchar(100),
	@CREATED_DATE datetime
	
 )
As 
Begin
	Declare 
		@WS_ID int
		select @WS_ID = [WIN_SERVICE_ID] from [CSM_WINDOWS_SERVICES] 
					where [ENV_ID] = @ENV_ID
							and [CONFIG_ID] = @CONFIG_ID
		if @WS_ID is null or @WS_ID = 0 or @WS_ID = ''
		Begin
			insert into [CSM_WINDOWS_SERVICES]
			(
				ENV_ID
				,CONFIG_ID
				,WIN_SERVICENAME
				,WIN_COMMENTS
				,WIN_CREATED_BY
				,WIN_CREATED_DATE
			)
			values
			(
				@ENV_ID
				,@CONFIG_ID
				,@SERVICENAME
				,@COMMENTS
				,@CREATED_BY
				,@CREATED_DATE
			)
		End
		if @WS_ID > 0
		Begin
			Update [CSM_WINDOWS_SERVICES] set 
				WIN_SERVICENAME = @SERVICENAME
				,WIN_COMMENTS = @COMMENTS
				,WIN_CREATED_BY = @CREATED_BY
				,WIN_CREATED_DATE = @CREATED_DATE
				where WIN_SERVICE_ID = @WS_ID
				--ENV_ID = @ENV_ID and 
				--	CONFIG_ID = @CONFIG_ID
		End
End

--exec CWT_InsUpdWindowsService 1,1,'sdsd','sdsdsd','2015-05-10 10:10;10 PM'

GO


