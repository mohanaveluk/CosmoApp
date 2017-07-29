

/****** Object:  StoredProcedure [dbo].[CWT_InsUpdSubscription]    Script Date: 09/13/2016 22:14:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CWT_InsUpdSubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CWT_InsUpdSubscription]
GO



/****** Object:  StoredProcedure [dbo].[CWT_InsUpdSubscription]    Script Date: 09/13/2016 22:14:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


  CREATE procedure [dbo].[CWT_InsUpdSubscription]
  (
	@SUBSCRIPTION_ID int
	,@SUBSCRIPTION_TYPE varchar (100)
	,@SUBSCRIPTION_TIME varchar(100) 
	,@SUBSCRIPTION_ISACTIVE bit
	,@CREATED_BY varchar(100)
	,@CREATED_DATE datetime
	,@UPDATED_BY varchar(100)
	,@UPDATED_DATE datetime
	,@NEXTJOBRUNTIME datetime
	,@SUBSCRIPTION_EMAILS varchar(max)
	,@SCOPE_OUTPUT int output
  ) 
  As
  Declare
	@Character CHAR(1)
	,@StartIndex INT
	,@EndIndex INT
	,@Input varchar(max)
	,@tempEmailId varchar(200)
	,@tempUserLst int
  BEGIN
		set @Character = ','
		set @Input = @SUBSCRIPTION_EMAILS
		SET @StartIndex = 1
  
		if @SUBSCRIPTION_ID > 0
		BEGIN
			Update [dbo].[CSM_REPORT_SUBSCRIPTION] set 
				[SUBSCRIPTION_TYPE] = @SUBSCRIPTION_TYPE
				,[SUBSCRIPTION_TIME] = @SUBSCRIPTION_TIME
				,[SUBSCRIPTION_ISACTIVE] = @SUBSCRIPTION_ISACTIVE
				,[UPDATED_BY] = @UPDATED_BY
				,[UPDATED_DATE] = @UPDATED_DATE
				Where [SUBSCRIPTION_ID] = @SUBSCRIPTION_ID
		END
		Else
		BEGIN
			Insert into [dbo].[CSM_REPORT_SUBSCRIPTION]
			(
				[SUBSCRIPTION_TYPE]
				,[SUBSCRIPTION_TIME]
				,[CREATED_BY]
				,[CREATED_DATE]
				,[SCH_NEXTJOBRAN_TIME]
			)
			Values
			(
				@SUBSCRIPTION_TYPE
				,@SUBSCRIPTION_TIME
				,@CREATED_BY
				,@CREATED_DATE
				,@NEXTJOBRUNTIME
			)
			set @SUBSCRIPTION_ID = IDENT_CURRENT('CSM_REPORT_SUBSCRIPTION')
		END

		if @SUBSCRIPTION_ID > 0 AND	EXISTS(Select * from [dbo].[CSM_REPORT_SUBSCRIPTION_DETAILS] where [SUBSCRIPTION_ID] = @SUBSCRIPTION_ID)
		BEGIN
			Delete from [dbo].[CSM_REPORT_SUBSCRIPTION_DETAILS] where [SUBSCRIPTION_ID] = @SUBSCRIPTION_ID
		END
		
		If @SUBSCRIPTION_EMAILS <> ''
		BEGIN
			IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
			BEGIN
				SET @Input = @Input + @Character
			END	
			
			WHILE CHARINDEX(@Character, @Input) > 0
			Begin
				set @EndIndex = CHARINDEX(@Character, @Input)
				set @tempEmailId = SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
				
				select @tempUserLst = USRLST_ID 
					from [dbo].[CSM_EMAIL_USERLIST]
					where USRLST_EMAIL_ADDRESS = @tempEmailId
					
				insert into [dbo].[CSM_REPORT_SUBSCRIPTION_DETAILS]
				(
				  [SUBSCRIPTION_ID]
				  ,[USRLST_ID]
				  ,[SUBSCRIPTION_EMAILID]
				  ,[SUBSCRIPTION_ISACTIVE]
				)
				Values
				(
					@SUBSCRIPTION_ID
					,@tempUserLst
					,@tempEmailId
					,'True'
				)
				
				set @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
			End
		END
		set @SCOPE_OUTPUT = @SUBSCRIPTION_ID
  END
  
GO


