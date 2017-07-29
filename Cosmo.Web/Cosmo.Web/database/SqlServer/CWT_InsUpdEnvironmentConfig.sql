SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE  procedure [dbo].[CWT_InsUpdEnvironmentConfig]
(	  
	  @ENV_ID int,
	  @CONFIG_ID int,
      @ENV_NAME varchar(100),
      @ENV_LOCATION varchar(100),
      @ENV_HOST_IP_ADDRESS varchar(100),
      @ENV_PORT varchar(100),
      @ENV_DESCRIPTION varchar(200),
      @ENV_SERVICETYPE varchar(100),
      @ENV_SERVICEURL varchar(100),
      @ENV_MAIL_FREQ varchar(100),
      @ENV_IS_MONITOR bit,
      @ENV_IS_NOTIFY bit,
      @ENV_IS_CONSLTD_MAIL bit,
      @ENV_COMMENTS varchar(5000),
      @ENV_ISACTIVE bit,
      @ENV_CREATED_BY varchar(100),
      @ENV_CREATED_DATE datetime,
      @ENV_UPDATED_BY varchar(100),
      @ENV_UPDATED_DATE datetime,
      @CATEGORY varchar(20),
	  @CONFIG_ISPRIMARY bit,
	  @CONFIG_REF_ID int,
	  @WINDOWS_SERVICE_NAME varchar(MAX),
	  @WINDOWS_SERVICE_ID int
)
as
Begin try
	Declare 
		@Environtment_ID int
		,@EnvirontmentDetails_ID int
		,@Environtment_Name varchar(max)
		,@CurrentTime datetime
		
		if(@ENV_ID is not null and @ENV_ID > 0)
			Begin
				if((@CONFIG_ID is null or @CONFIG_ID = '' or @CONFIG_ID <= 0) and @CATEGORY = 'add_ed')
				Begin
					insert into [CSM_CONFIGURATION] (
					   [ENV_ID]
					  ,[CONFIG_SERVICE_TYPE]
					  ,[CONFIG_PORT_NUMBER]
					  ,[CONFIG_URL_ADDRESS]
					  ,[CONFIG_DESCRIPTION]
					  ,[CONFIG_IS_VALIDATED]
					  ,[CONFIG_IS_ACTIVE]
					  ,[CONFIG_IS_MONITORED]
					  ,[CONFIG_IS_LOCKED]
					  ,[CONFIG_CREATED_BY]
					  ,[CONFIG_CREATED_DATE]
					  ,[CONFIG_COMMENTS]
					  ,[CONFIG_HOST_IP]
					  ,[CONFIG_MAIL_FREQ]
					  ,[CONFIG_LOCATION]
					  ,[CONFIG_ISCONSOLIDATED]
					  ,[CONFIG_ISNOTIFY]
					  ,[CONFIG_ISPRIMARY]
					  ,[CONFIG_REF_ID]
					  ,[CONFIG_ISNOTIFY_MAIN]
					  	)
					  values(
					   @ENV_ID
					  ,@ENV_SERVICETYPE
					  ,@ENV_PORT
					  ,@ENV_SERVICEURL
					  ,@ENV_DESCRIPTION
					  ,'true'
					  ,'true'
					  ,@ENV_IS_MONITOR
					  ,'true'
					  ,@ENV_CREATED_BY
					  ,@ENV_CREATED_DATE
					  ,@ENV_COMMENTS
					  ,@ENV_HOST_IP_ADDRESS
					  ,@ENV_MAIL_FREQ
					  ,@ENV_LOCATION
					  ,@ENV_IS_CONSLTD_MAIL
					  ,@ENV_IS_NOTIFY
					  ,@CONFIG_ISPRIMARY
					  ,@CONFIG_REF_ID
					  ,@ENV_IS_NOTIFY
					  )
					set @EnvirontmentDetails_ID = IDENT_CURRENT('CSM_CONFIGURATION')
				End
				else if(@CONFIG_ID is not null  and @CONFIG_ID > 0 and @CATEGORY = 'modify_ed')
				Begin
					Update [CSM_CONFIGURATION] set 
					   [ENV_ID] = @ENV_ID
					  ,[CONFIG_SERVICE_TYPE] = @ENV_SERVICETYPE
					  ,[CONFIG_PORT_NUMBER] = @ENV_PORT
					  ,[CONFIG_URL_ADDRESS] = @ENV_SERVICEURL
					  ,[CONFIG_DESCRIPTION] = @ENV_DESCRIPTION
					  ,[CONFIG_IS_VALIDATED] = 'true'
					  ,[CONFIG_IS_ACTIVE] = 'true'
					  ,[CONFIG_IS_MONITORED] = @ENV_IS_MONITOR
					  ,[CONFIG_IS_LOCKED] = 'true'
					  ,[CONFIG_UPDATED_BY] = @ENV_UPDATED_BY
					  ,[CONFIG_UPDATED_DATE] = @ENV_UPDATED_DATE
					  ,[CONFIG_COMMENTS] = @ENV_COMMENTS
					  ,[CONFIG_HOST_IP] = @ENV_HOST_IP_ADDRESS
					  ,[CONFIG_MAIL_FREQ] = @ENV_MAIL_FREQ
					  ,[CONFIG_LOCATION] = @ENV_LOCATION
					  ,[CONFIG_ISCONSOLIDATED] = @ENV_IS_CONSLTD_MAIL
					  ,[CONFIG_ISNOTIFY] = @ENV_IS_NOTIFY
					  ,[CONFIG_ISPRIMARY] = @CONFIG_ISPRIMARY
					  ,[CONFIG_REF_ID] = @CONFIG_REF_ID
					  ,[CONFIG_ISNOTIFY_MAIN] = @ENV_IS_NOTIFY
					  where [CONFIG_ID] = @CONFIG_ID
					  set @EnvirontmentDetails_ID = @CONFIG_ID
					  
					  if @ENV_SERVICETYPE = '2'
					  begin
						update [CSM_CONFIGURATION] set  [CONFIG_IS_ACTIVE] = 'false' 
							where  [CONFIG_REF_ID] = @CONFIG_ID 
								and [CONFIG_SERVICE_TYPE] = @ENV_SERVICETYPE
								and [CONFIG_URL_ADDRESS] = @ENV_SERVICEURL
					  end
					  
					  if @ENV_IS_NOTIFY = 'true'
					  Begin
						Update [CSM_ENVIRONEMENT] set [ENV_IS_NOTIFY] =  @ENV_IS_NOTIFY where ENV_ID = @ENV_ID
					  End
					  
					  if @ENV_IS_MONITOR = 'true'
					  Begin
						Update [CSM_ENVIRONEMENT] set [ENV_IS_MONITOR] =  @ENV_IS_MONITOR where ENV_ID = @ENV_ID
					  End

					  if @ENV_IS_CONSLTD_MAIL = 'true'
					  Begin
						Update [CSM_ENVIRONEMENT] set [ENV_IS_CONSLTD_MAIL] =  @ENV_IS_CONSLTD_MAIL where ENV_ID = @ENV_ID
					  End
					  
				End
				else if(@CONFIG_ID is not null  and @CONFIG_ID <= 0 and @CATEGORY = 'modify_ed')
				Begin
					insert into [CSM_CONFIGURATION] (
					   [ENV_ID]
					  ,[CONFIG_SERVICE_TYPE]
					  ,[CONFIG_PORT_NUMBER]
					  ,[CONFIG_URL_ADDRESS]
					  ,[CONFIG_DESCRIPTION]
					  ,[CONFIG_IS_VALIDATED]
					  ,[CONFIG_IS_ACTIVE]
					  ,[CONFIG_IS_MONITORED]
					  ,[CONFIG_IS_LOCKED]
					  ,[CONFIG_CREATED_BY]
					  ,[CONFIG_CREATED_DATE]
					  ,[CONFIG_COMMENTS]
					  ,[CONFIG_HOST_IP]
					  ,[CONFIG_MAIL_FREQ]
					  ,[CONFIG_LOCATION]
					  ,[CONFIG_ISCONSOLIDATED]
					  ,[CONFIG_ISNOTIFY]
					  ,[CONFIG_ISPRIMARY]
					  ,[CONFIG_REF_ID]
					  ,[CONFIG_ISNOTIFY_MAIN]
					  	)
					  values(
					   @ENV_ID
					  ,@ENV_SERVICETYPE
					  ,@ENV_PORT
					  ,@ENV_SERVICEURL
					  ,@ENV_DESCRIPTION
					  ,'true'
					  ,'true'
					  ,@ENV_IS_MONITOR
					  ,'true'
					  ,@ENV_CREATED_BY
					  ,@ENV_CREATED_DATE
					  ,@ENV_COMMENTS
					  ,@ENV_HOST_IP_ADDRESS
					  ,@ENV_MAIL_FREQ
					  ,@ENV_LOCATION
					  ,@ENV_IS_CONSLTD_MAIL
					  ,@ENV_IS_NOTIFY
					  ,@CONFIG_ISPRIMARY
					  ,@CONFIG_REF_ID
					  ,@ENV_IS_NOTIFY
					  )
					  set @EnvirontmentDetails_ID = IDENT_CURRENT('CSM_CONFIGURATION')
				End
				else if @CATEGORY = 'modify_en'
				Begin
					Update [CSM_ENVIRONEMENT] set
						   [ENV_NAME]				=  @ENV_NAME
						  ,[ENV_IS_MONITOR]			=  @ENV_IS_MONITOR
						  ,[ENV_IS_NOTIFY]			=  @ENV_IS_NOTIFY
						  ,[ENV_UPDATED_BY]			=  @ENV_UPDATED_BY
						  ,[ENV_UPDATED_DATE]		=  @ENV_UPDATED_DATE
						  ,[ENV_COMMENTS]			=  @ENV_COMMENTS
						  ,[ENV_MAIL_FREQ]			=  @ENV_MAIL_FREQ
						  ,[ENV_IS_CONSLTD_MAIL]	=  @ENV_IS_CONSLTD_MAIL
						where ENV_ID = @ENV_ID
						set @Environtment_ID  = @ENV_ID
						
					Update dbo.[CSM_CONFIGURATION] set
					CONFIG_IS_MONITORED = @ENV_IS_MONITOR,
					CONFIG_ISNOTIFY = @ENV_IS_NOTIFY,
					CONFIG_ISCONSOLIDATED = @ENV_IS_CONSLTD_MAIL
					where ENV_ID = @ENV_ID
				end
			set @Environtment_ID = @ENV_ID
			End
		else
			Begin
				select @Environtment_ID = [ENV_ID] from [CSM_ENVIRONEMENT] 
					where lower([ENV_NAME]) = lower(@ENV_NAME) 
					and ENV_ISACTIVE = 'True'
					--and 
					--[ENV_HOST_IP_ADDRESS] = @ENV_HOST_IP_ADDRESS and 
					--[ENV_LOCATION] = @ENV_LOCATION
				--Insert / update environment 
				if (@Environtment_ID > 0 and @Environtment_ID is not null)
				Begin
					Begin tran
					Update [CSM_ENVIRONEMENT] set
					   [ENV_NAME]				=  @ENV_NAME
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
					  ,(SELECT 
						  CASE WHEN MAX(ENV_SORTORDER)IS NULL THEN 1 ELSE MAX(ENV_SORTORDER)+1 END AS ENV_SORTORDER
						  FROM [dbo].[CSM_ENVIRONEMENT])
					)
					set @Environtment_ID  = IDENT_CURRENT('CSM_ENVIRONEMENT')
					
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
					
				if @EnvirontmentDetails_ID = '' or @EnvirontmentDetails_ID is null or @EnvirontmentDetails_ID <= 0
				Begin
					insert into [CSM_CONFIGURATION] (
					   [ENV_ID]
					  ,[CONFIG_SERVICE_TYPE]
					  ,[CONFIG_PORT_NUMBER]
					  ,[CONFIG_URL_ADDRESS]
					  ,[CONFIG_DESCRIPTION]
					  ,[CONFIG_IS_VALIDATED]
					  ,[CONFIG_IS_ACTIVE]
					  ,[CONFIG_IS_MONITORED]
					  ,[CONFIG_IS_LOCKED]
					  ,[CONFIG_CREATED_BY]
					  ,[CONFIG_CREATED_DATE]
					  ,[CONFIG_COMMENTS]
					  ,[CONFIG_HOST_IP]
					  ,[CONFIG_MAIL_FREQ]
					  ,[CONFIG_LOCATION]
					  ,[CONFIG_ISCONSOLIDATED]	
					  ,[CONFIG_ISNOTIFY]
					  ,[CONFIG_ISPRIMARY]
					  ,[CONFIG_REF_ID]
					  ,[CONFIG_ISNOTIFY_MAIN]
					   )
					  values(
					   @Environtment_ID
					  ,@ENV_SERVICETYPE
					  ,@ENV_PORT
					  ,@ENV_SERVICEURL
					  ,@ENV_DESCRIPTION
					  ,'true'
					  ,'true'
					  ,@ENV_IS_MONITOR
					  ,'true'
					  ,@ENV_CREATED_BY
					  ,@ENV_CREATED_DATE
					  ,@ENV_COMMENTS
					  ,@ENV_HOST_IP_ADDRESS
					  ,@ENV_MAIL_FREQ
					  ,@ENV_LOCATION
					  ,@ENV_IS_CONSLTD_MAIL
					  ,@ENV_IS_NOTIFY
					  ,@CONFIG_ISPRIMARY
					  ,@CONFIG_REF_ID
					  ,@ENV_IS_NOTIFY
					  )
					  set @EnvirontmentDetails_ID = IDENT_CURRENT('CSM_CONFIGURATION')
				ENd	

			End
			
		if @EnvirontmentDetails_ID > 0 and @CONFIG_ISPRIMARY = 'True'
		Begin
			set @CurrentTime = GETDATE()
			
			exec CWT_InsUpdWindowsService 
				@Environtment_ID
				,@EnvirontmentDetails_ID
				,@WINDOWS_SERVICE_NAME
				,''
				,@ENV_CREATED_BY
				,@ENV_CREATED_DATE
				
			exec CWT_InsUpdServerPerformanceSchedule 
				@Environtment_ID	
				,@EnvirontmentDetails_ID
				,@ENV_HOST_IP_ADDRESS
				,@ENV_PORT
				,@CurrentTime
				,@CurrentTime
				,'IS'
		ENd
		
End try
Begin catch
	 DECLARE
	  @ErMessage NVARCHAR(2048),
	  @ErSeverity INT,
	  @ErState INT
	  
	SELECT
	  @ErMessage = ERROR_MESSAGE(),
	  @ErSeverity = ERROR_SEVERITY(),
	  @ErState = ERROR_STATE()
	  
	  RAISERROR(@ErMessage,@ErSeverity,@ErState)

End catch

SET ANSI_NULLS ON


GO


