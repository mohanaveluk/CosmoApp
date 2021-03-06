/****** Object:  StoredProcedure [dbo].[CWT_InsUpdScheduler]    Script Date: 2/9/2015 6:17:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsUpdScheduler]
(
	@ENV_ID int,
	@CONFIG_ID int,
	@SCH_INTERVAL int,
	@SCH_DURATION varchar(100),
	@SCH_REPEATS varchar(100),
	@SCH_STARTBY datetime,
	@SCH_ENDAS varchar(100),
	@SCH_END_OCCURANCE int,
	@SCH_ENDBY datetime,
	@SCH_IS_ACTIVE bit,
	@SCH_CREATED_BY varchar(100),
	@SCH_CREATED_DATE datetime,
	@SCH_UPDATED_BY varchar(100),
	@SCH_UPDATED_DATE datetime,
	@SCH_COMMENTS varchar(1000)
)
As
Begin
	Declare
		@SCHID int, @CONFIG_ID_DISP int
		select @SCHID = [SCH_ID] from [CSM_SCHEDULE] where
		[ENV_ID] = @ENV_ID AND
		[CONGIG_ID]	= @CONFIG_ID

		-- to add scheduler details for dispatcher incase content manager available
		select @CONFIG_ID_DISP = CONFIG_ID from [dbo].[CSM_CONFIGURATION] 
		where CONFIG_URL_ADDRESS like (select CONFIG_URL_ADDRESS from [CSM_CONFIGURATION] where CONFIG_ID=@CONFIG_ID) +'%'
		and CONFIG_URL_ADDRESS not like (select CONFIG_URL_ADDRESS from [CSM_CONFIGURATION] where CONFIG_ID=@CONFIG_ID)

	if @SCHID is null or @SCHID <= 0 
	Begin
		insert into CSM_SCHEDULE (
		   [ENV_ID]
		  ,[CONGIG_ID]
		  ,[SCH_INTERVAL]
		  ,[SCH_DURATION]
		  ,[SCH_REPEATS]
		  ,[SCH_STARTBY]
		  ,[SCH_ENDAS]
		  ,[SCH_END_OCCURANCE]
		  ,[SCH_ENDBY]
		  ,[SCH_IS_ACTIVE]
		  ,[SCH_CREATED_BY]
		  ,[SCH_CREATED_DATE]
		  ,[SCH_COMMENTS]
		  )
		  values
		  (
		   @ENV_ID
		  ,@CONFIG_ID
		  ,@SCH_INTERVAL
		  ,@SCH_DURATION
		  ,@SCH_REPEATS
		  ,@SCH_STARTBY
		  ,@SCH_ENDAS
		  ,@SCH_END_OCCURANCE
		  ,@SCH_ENDBY
		  ,@SCH_IS_ACTIVE
		  ,@SCH_CREATED_BY
		  ,@SCH_CREATED_DATE
		  ,@SCH_COMMENTS
		  )
		  --to insert schedule detail for dispatch correponds to content manager
		  if(@CONFIG_ID_DISP !='' and @CONFIG_ID_DISP >0)
		  Begin
			insert into CSM_SCHEDULE (
			   [ENV_ID]
			  ,[CONGIG_ID]
			  ,[SCH_INTERVAL]
			  ,[SCH_DURATION]
			  ,[SCH_REPEATS]
			  ,[SCH_STARTBY]
			  ,[SCH_ENDAS]
			  ,[SCH_END_OCCURANCE]
			  ,[SCH_ENDBY]
			  ,[SCH_IS_ACTIVE]
			  ,[SCH_CREATED_BY]
			  ,[SCH_CREATED_DATE]
			  ,[SCH_COMMENTS]
			  )
			  values
			  (
			   @ENV_ID
			  ,@CONFIG_ID_DISP
			  ,@SCH_INTERVAL
			  ,@SCH_DURATION
			  ,@SCH_REPEATS
			  ,@SCH_STARTBY
			  ,@SCH_ENDAS
			  ,@SCH_END_OCCURANCE
			  ,@SCH_ENDBY
			  ,@SCH_IS_ACTIVE
			  ,@SCH_CREATED_BY
			  ,@SCH_CREATED_DATE
			  ,@SCH_COMMENTS
			  )		 
		  End
     End
     else
     Begin
		Update CSM_SCHEDULE set 
		   [ENV_ID]			=     @ENV_ID
		  ,[CONGIG_ID]		=	  @CONFIG_ID
		  ,[SCH_INTERVAL]	=	  @SCH_INTERVAL
		  ,[SCH_DURATION]	=	  @SCH_DURATION
		  ,[SCH_REPEATS]	=	  @SCH_REPEATS
		  ,[SCH_STARTBY]	=	  @SCH_STARTBY
		  ,[SCH_ENDAS]		=	  @SCH_ENDAS
		  ,[SCH_END_OCCURANCE]	= @SCH_END_OCCURANCE
		  ,[SCH_ENDBY]		=	  @SCH_ENDBY
		  ,[SCH_IS_ACTIVE]	=	  @SCH_IS_ACTIVE
		  ,[SCH_UPDATED_BY]	=	  @SCH_CREATED_BY
		  ,[SCH_UPDATED_DATE]	= @SCH_CREATED_DATE
		  ,[SCH_COMMENTS]	=	  @SCH_COMMENTS
		  where [SCH_ID] = @SCHID

		  --to update schedule detail for dispatch correponds to content manager
		  if(@CONFIG_ID_DISP is not null and @CONFIG_ID_DISP >0)
		  Begin
				select @SCHID = [SCH_ID] from [CSM_SCHEDULE] where
				[ENV_ID] = @ENV_ID AND
				[CONGIG_ID]	= @CONFIG_ID_DISP

			Update CSM_SCHEDULE set 
			   [ENV_ID]			=     @ENV_ID
			  ,[CONGIG_ID]		=	  @CONFIG_ID_DISP
			  ,[SCH_INTERVAL]	=	  @SCH_INTERVAL
			  ,[SCH_DURATION]	=	  @SCH_DURATION
			  ,[SCH_REPEATS]	=	  @SCH_REPEATS
			  ,[SCH_STARTBY]	=	  @SCH_STARTBY
			  ,[SCH_ENDAS]		=	  @SCH_ENDAS
			  ,[SCH_END_OCCURANCE]	= @SCH_END_OCCURANCE
			  ,[SCH_ENDBY]		=	  @SCH_ENDBY
			  ,[SCH_IS_ACTIVE]	=	  @SCH_IS_ACTIVE
			  ,[SCH_UPDATED_BY]	=	  @SCH_CREATED_BY
			  ,[SCH_UPDATED_DATE]	= @SCH_CREATED_DATE
			  ,[SCH_COMMENTS]	=	  @SCH_COMMENTS
			  where [SCH_ID] = @SCHID
		  End
     End
End      
