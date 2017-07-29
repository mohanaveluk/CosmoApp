
/****** Object:  StoredProcedure [dbo].[CWT_InsertBuildDetails]    Script Date: 3/15/2015 5:53:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_InsertBuildDetails](
	 @CONFIG_ID int
	,@ENV_ID int
	,@MON_CREATED_DATE datetime
	,@MON_COMMENTS varchar(500)

)
As
Begin
	Declare 
	@BuildID int
	,@Version varchar(100)
	,@StartIndex int
	,@EndIndex int

	if(CHARINDEX('Build',@MON_COMMENTS) > 0)
	Begin
		set @StartIndex = CHARINDEX('Build:',@MON_COMMENTS) +  + LEN('Build: ')
		set @EndIndex = CHARINDEX('Status',@MON_COMMENTS)
		set @Version = SUBSTRING(@MON_COMMENTS,@StartIndex,(@EndIndex - @StartIndex))
	End
	else if(CHARINDEX('version:',@MON_COMMENTS) > 0)
	Begin
		set @StartIndex = CHARINDEX('version:',@MON_COMMENTS) +  + LEN('version: ')	
		set @EndIndex = CHARINDEX('Dispatcher',@MON_COMMENTS)
		set @Version = SUBSTRING(@MON_COMMENTS,@StartIndex,(@EndIndex - @StartIndex))
	End

	if @Version is not null	 or @Version <> '' 
	Begin
		select @BuildID = BUILD_ID from CSM_SERVICEBUILD bld where bld.ENV_ID = @ENV_ID and bld.CONFIG_ID = @CONFIG_ID and LTRIM(RTRIM(bld.BUILD_VERSION)) = LTRIM(RTRIM(@Version))
		Print @BuildID
		if(@BuildID = '' or @BuildID is null)
		Begin
			insert into CSM_SERVICEBUILD
			(
				[ENV_ID],
				[CONFIG_ID],
				[BUILD_DATE],
				[BUILD_VERSION],
				[CREATED_DATE]
			)
			values
			(
				@ENV_ID
				,@CONFIG_ID
				,CONVERT(varchar,@MON_CREATED_DATE,101)
				,@Version
				,@MON_CREATED_DATE
			)
		End
		Else if @BuildID > 0
		Begin
			print @BuildID
		End
	End
End




GO


