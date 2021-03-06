/****** Object:  StoredProcedure [dbo].[CWT_InsUpdEnvironment]    Script Date: 2/9/2015 6:17:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsUpdEnvironment]
(	  
	  @ENV_ID int,
	  @ENVDET_ID int,
      @ENV_NAME varchar(100),
      @ENV_HOST_IP_ADDRESS varchar(100),
      @ENV_LOCATION varchar(100),
      @ENV_IS_MONITOR bit,
      @ENV_IS_NOTIFY bit,
      @ENV_MAIL_FREQ varchar(100),
      @ENV_IS_CONSLTD_MAIL bit,
      @ENV_COMMENTS varchar(5000),
      @ENV_ISACTIVE bit,
      @ENV_CREATED_BY varchar(100),
      @ENV_CREATED_DATE datetime,
      @ENV_UPDATED_BY varchar(100),
      @ENV_UPDATED_DATE datetime,
      @CATEGORY varchar(20)
)
as
Begin
	Declare 
		@Environtment_ID int,
		@EnvirontmentDetails_ID int
		if(@ENV_ID is not null and @ENV_ID > 0)
			Begin
				if((@ENVDET_ID = '' or @ENVDET_ID is null or @ENVDET_ID <= 0) and @CATEGORY = 'add_ed')
				Begin
					insert into [CSM_ENVIRONEMENTDETAILS] (
					   [ENV_ID] 
					  ,[ENVDET_HOST_IP_ADDRESS]
					  ,[ENVDET_LOCATION]
					  ,[ENVDET_IS_MONITOR]
					  ,[ENVDET_IS_NOTIFY]
					  ,[ENVDET_CREATED_BY]
					  ,[ENVDET_CREATED_DATE]
					  ,[ENVDET_COMMENTS]
					  ,[ENVDET_MAIL_FREQ]
					  ,[ENVDET_IS_CONSLTD_MAIL]
					  ,ENVDET_ISACTIVE)
					  values(
						@ENV_ID
						,@ENV_HOST_IP_ADDRESS
						,@ENV_LOCATION
						,@ENV_IS_MONITOR
						,@ENV_IS_NOTIFY
						,@ENV_CREATED_BY
						,@ENV_CREATED_DATE
						,@ENV_COMMENTS
						,@ENV_MAIL_FREQ
						,@ENV_IS_CONSLTD_MAIL
						,@ENV_ISACTIVE
					  )
				End
				else if(@ENVDET_ID > 0 and @ENVDET_ID is not null  and @CATEGORY = 'modify_ed')
				Begin
					Update [CSM_ENVIRONEMENTDETAILS] set 
					   [ENV_ID] = @ENV_ID
					  ,[ENVDET_HOST_IP_ADDRESS] = @ENV_HOST_IP_ADDRESS
					  ,[ENVDET_LOCATION]		= @ENV_LOCATION
					  ,[ENVDET_IS_MONITOR]		= @ENV_IS_MONITOR
					  ,[ENVDET_IS_NOTIFY]		= @ENV_IS_NOTIFY
					  ,[ENVDET_CREATED_BY]		= @ENV_CREATED_BY
					  ,[ENVDET_CREATED_DATE]	= @ENV_CREATED_DATE
					  ,[ENVDET_COMMENTS]		= @ENV_COMMENTS
					  ,[ENVDET_MAIL_FREQ]		= @ENV_MAIL_FREQ
					  ,[ENVDET_IS_CONSLTD_MAIL]	= @ENV_IS_CONSLTD_MAIL
					  --,ENVDET_ISACTIVE			= @ENV_ISACTIVE
					  where [ENVDET_ID] = @ENVDET_ID
				End
				else if @CATEGORY = 'modify_en'
				Begin
					Update [CSM_ENVIRONEMENT] set
						   [ENV_NAME]				=  @ENV_NAME
						  --,[ENV_HOST_IP_ADDRESS]	=  @ENV_HOST_IP_ADDRESS
						  --,[ENV_LOCATION]			=  @ENV_LOCATION
						  ,[ENV_IS_MONITOR]			=  @ENV_IS_MONITOR
						  ,[ENV_IS_NOTIFY]			=  @ENV_IS_NOTIFY
						  ,[ENV_UPDATED_BY]			=  @ENV_UPDATED_BY
						  ,[ENV_UPDATED_DATE]		=  @ENV_UPDATED_DATE
						  ,[ENV_COMMENTS]			=  @ENV_COMMENTS
						  ,[ENV_MAIL_FREQ]			=  @ENV_MAIL_FREQ
						  ,[ENV_IS_CONSLTD_MAIL]	=  @ENV_IS_CONSLTD_MAIL
						where ENV_ID = @ENV_ID
				end
					
			End
		else
			Begin
				select @Environtment_ID = [ENV_ID] from [CSM_ENVIRONEMENT] 
					where lower([ENV_NAME]) = lower(@ENV_NAME) --and 
					--[ENV_HOST_IP_ADDRESS] = @ENV_HOST_IP_ADDRESS and 
					--[ENV_LOCATION] = @ENV_LOCATION
				--Insert / update environment 
				if (@Environtment_ID > 0 and @Environtment_ID is not null)
				Begin
					Begin tran
					Update [CSM_ENVIRONEMENT] set
					   [ENV_NAME]				=  @ENV_NAME
					 -- ,[ENV_HOST_IP_ADDRESS]	=  @ENV_HOST_IP_ADDRESS
					 -- ,[ENV_LOCATION]			=  @ENV_LOCATION
					  ,[ENV_IS_MONITOR]			=  @ENV_IS_MONITOR
					  ,[ENV_IS_NOTIFY]			=  @ENV_IS_NOTIFY
					  ,[ENV_UPDATED_BY]			=  @ENV_UPDATED_BY
					  ,[ENV_UPDATED_DATE]		=  @ENV_UPDATED_DATE
					  ,[ENV_COMMENTS]			=  @ENV_COMMENTS
					  ,[ENV_MAIL_FREQ]			=  @ENV_MAIL_FREQ
					  ,[ENV_IS_CONSLTD_MAIL]	=  @ENV_IS_CONSLTD_MAIL
					where ENV_ID = @Environtment_ID
					if(@@ERROR=0)
					  begin
						commit Tran
						select 0
					  end
					else
					  begin
						rollback Tran
						select -1
					  end
				End	
				else
				Begin
					Begin tran
					insert into [CSM_ENVIRONEMENT] (
					   [ENV_NAME]
					  --,[ENV_HOST_IP_ADDRESS]
					  --,[ENV_LOCATION]
					  ,[ENV_IS_MONITOR]
					  ,[ENV_IS_NOTIFY]
					  ,[ENV_CREATED_BY]
					  ,[ENV_CREATED_DATE]
					  --,[ENV_COMMENTS]
					  ,[ENV_MAIL_FREQ]
					  ,[ENV_IS_CONSLTD_MAIL]
					  ,[ENV_ISACTIVE]
					  ,[ENV_SORTORDER]
					)
					values
					(
						@ENV_NAME
					--  ,@ENV_HOST_IP_ADDRESS
					--  ,@ENV_LOCATION
					  ,@ENV_IS_MONITOR
					  ,@ENV_IS_NOTIFY
					  ,@ENV_CREATED_BY
					  ,@ENV_CREATED_DATE
					  --,@ENV_COMMENTS
					  ,@ENV_MAIL_FREQ
					  ,@ENV_IS_CONSLTD_MAIL
					  ,@ENV_ISACTIVE
					  ,(select MAX([ENV_SORTORDER]) + 1 from [CSM_ENVIRONEMENT])
					)
					if(@@ERROR=0)
					  begin
						commit Tran
						select 0
					  end
					else
					  begin
						rollback Tran
						select -1
					  end
				End
				--Insert / update environment details to details table
				select @EnvirontmentDetails_ID = [ENVDET_ID] from [CSM_ENVIRONEMENTDETAILS] 
					where lower([ENVDET_HOST_IP_ADDRESS]) = lower(@ENV_HOST_IP_ADDRESS) and 
					lower([ENVDET_LOCATION]) = lower(@ENV_LOCATION)
				select @Environtment_ID = [ENV_ID] from [CSM_ENVIRONEMENT] 
					where lower([ENV_NAME]) = lower(@ENV_NAME)
				if @EnvirontmentDetails_ID = '' or @EnvirontmentDetails_ID is null or @EnvirontmentDetails_ID <= 0
				Begin
					insert into [CSM_ENVIRONEMENTDETAILS] (
					   [ENV_ID] 
					  ,[ENVDET_HOST_IP_ADDRESS]
					  ,[ENVDET_LOCATION]
					  ,[ENVDET_IS_MONITOR]
					  ,[ENVDET_IS_NOTIFY]
					  ,[ENVDET_CREATED_BY]
					  ,[ENVDET_CREATED_DATE]
					  ,[ENVDET_COMMENTS]
					  ,[ENVDET_MAIL_FREQ]
					  ,[ENVDET_IS_CONSLTD_MAIL]
					  ,ENVDET_ISACTIVE)
					  values(
						@Environtment_ID
						,@ENV_HOST_IP_ADDRESS
						,@ENV_LOCATION
						,@ENV_IS_MONITOR
						,@ENV_IS_NOTIFY
						,@ENV_CREATED_BY
						,@ENV_CREATED_DATE
						,@ENV_COMMENTS
						,@ENV_MAIL_FREQ
						,@ENV_IS_CONSLTD_MAIL
						,@ENV_ISACTIVE
					  )
				ENd	
			End
	End


SET ANSI_NULLS ON

GO
