SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_InsUpdPersonalize](
	@PERS_ID int
	,@USER_ID int
	,@PERS_DB_REFRESHTIME int
	,@PERS_ISACTIVE bit
	,@PERS_CREATEDDATE datetime
	,@PERS_CREATEDBY varchar(10)
	,@PERS_SORTORDER varchar(200)
)
As 
declare
	@Character CHAR(1)
	,@StartIndex INT
	,@EndIndex INT
	,@Input varchar(max)
	,@temp_@PERS_Count int
	,@tempSortOrder int
	,@tempCount int
	
Begin
	set @Character = ','
	set @Input = @PERS_SORTORDER
	SET @StartIndex = 1
	set @tempCount = 0
	
	if @USER_ID <> '' AND @USER_ID > 0 
	Begin
		select @temp_@PERS_Count = COUNT(*)	
		from CSM_POERSONALIZE
		Where USER_ID = @USER_ID
		
		if @temp_@PERS_Count = 0
		Begin
			insert into CSM_POERSONALIZE
			(
				User_ID
				,PERS_DB_REFRESHTIME
				,PERS_ISACTIVE
				,PERS_CREATEDDATE
				,PERS_CREATEDBY
			)
			values
			(
				@USER_ID
				,@PERS_DB_REFRESHTIME
				,@PERS_ISACTIVE
				,@PERS_CREATEDDATE
				,@PERS_CREATEDBY
			)
		End
		else if @temp_@PERS_Count =1 
		Begin
			Update CSM_POERSONALIZE set
				PERS_DB_REFRESHTIME = @PERS_DB_REFRESHTIME
				,PERS_ISACTIVE = @PERS_ISACTIVE
				,PERS_UPDATEDDATE = @PERS_CREATEDDATE
				,PERS_UPDATEDBY = @PERS_CREATEDBY
				Where USER_ID = @USER_ID
		End
		
		IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
		BEGIN
			SET @Input = @Input + @Character
		END
		WHILE CHARINDEX(@Character, @Input) > 0
		Begin	
			set @EndIndex = CHARINDEX(@Character, @Input)
			set @tempSortOrder = SUBSTRING(@Input, @StartIndex, @EndIndex - 1)	
			set @tempCount = @tempCount + 1
			update dbo.CSM_ENVIRONEMENT set [ENV_SORTORDER] = @tempCount where ENV_ID = @tempSortOrder
			
			set @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
		End	
		
	End
End



GO


