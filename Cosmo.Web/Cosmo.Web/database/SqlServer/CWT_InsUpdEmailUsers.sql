/****** Object:  StoredProcedure [dbo].[CWT_InsUpdEmailUsers]    Script Date: 2/9/2015 6:17:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsUpdEmailUsers]
(
	@USRLST_ID int,
	@ENV_ID int,
	@USRLST_EMAIL_ADDRESS varchar(200),
	@USRLST_MESSAGETYPE varchar(1),
	@USRLST_TYPE varchar(10),
	@USRLST_IS_ACTIVE bit,
	@USRLST_CREATED_BY varchar(50),
	@USRLST_CREATED_DATE datetime,
	@USRLST_COMMENTS varchar(1000)
) AS
declare 
	@tempUSRLST_ID varchar(200)
Begin
	set @tempUSRLST_ID  = 0
	SELECT @tempUSRLST_ID =  USRLST_ID from CSM_EMAIL_USERLIST 
		where [USRLST_EMAIL_ADDRESS] = @USRLST_EMAIL_ADDRESS
			and [ENV_ID] = @ENV_ID
	
		
	if @tempUSRLST_ID > 0
	begin
		if(@USRLST_ID is null or @USRLST_ID <= 0)
		Begin
			set @USRLST_ID = @tempUSRLST_ID
		End
	end
	
	if @USRLST_ID is null or @USRLST_ID <=0
	Begin
      insert into CSM_EMAIL_USERLIST
      (
		   [ENV_ID]
		  ,[USRLST_EMAIL_ADDRESS]
		  ,[USRLST_MESSAGETYPE]
		  ,[USRLST_TYPE]
		  ,[USRLST_IS_ACTIVE]
		  ,[USRLST_CREATED_BY]
		  ,[USRLST_CREATED_DATE]
		  ,[USRLST_COMMENTS]
      )
      values
      (
		   @ENV_ID
		  ,@USRLST_EMAIL_ADDRESS
		  ,@USRLST_MESSAGETYPE
		  ,@USRLST_TYPE
		  ,@USRLST_IS_ACTIVE
		  ,@USRLST_CREATED_BY
		  ,@USRLST_CREATED_DATE
		  ,@USRLST_COMMENTS
      )
    End
    else if @USRLST_ID > 0
    Begin
		Update CSM_EMAIL_USERLIST set 
			[USRLST_EMAIL_ADDRESS] = @USRLST_EMAIL_ADDRESS,
			[USRLST_MESSAGETYPE] = @USRLST_MESSAGETYPE,
			[USRLST_TYPE] = @USRLST_TYPE,
			[USRLST_UPDATED_BY] = @USRLST_CREATED_BY,
			[USRLST_UPDATED_DATE] = @USRLST_CREATED_DATE,
			[USRLST_IS_ACTIVE] = @USRLST_IS_ACTIVE,
			[USRLST_COMMENTS] = @USRLST_COMMENTS
		Where [USRLST_ID] = @USRLST_ID
    End
 
  End
SET ANSI_NULLS ON

