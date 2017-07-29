/****** Object:  StoredProcedure [dbo].[CWT_InsGroupDetail]    Script Date: 08/09/2015 00:14:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsGroupDetail]
(
	@GROUP_ID int
	,@ENV_ID int
	,@SERVICE_IDS varchar(max)
	,@WIN_SERVICE_ID int
	,@GROUP_DETAIL_COMMENTS varchar(2000)
	,@GROUP_CREATED_BY varchar(20)
	,@GROUP_CREATED_DATE datetime
	,@GROUP_ISACTIVE bit
)As
  Declare
	@Character CHAR(1)
	,@StartIndex INT
	,@EndIndex INT
	,@Input varchar(max)
	,@tempGroupDetailsID int
	,@tempConfigID int
Begin
	set @Character = ','
	set @Input = @SERVICE_IDS
	SET @StartIndex = 1
	
	IF SUBSTRING(@Input, LEN(@Input) - 0, LEN(@Input)) <> @Character
    BEGIN
			print SUBSTRING(@Input, LEN(@Input) - 0, LEN(@Input))
            SET @Input = @Input + @Character
    END
    --print @Input
	if @GROUP_ID > 0
	Begin
		
		--Delete from [CSM_GROUP_DETAIL] where GROUP_ID = @GROUP_ID and ENV_ID = @ENV_ID 
		update CSM_GROUP_DETAIL set [GROUP_ISACTIVE] = 'false' 
			where GROUP_ID = @GROUP_ID --and ENV_ID = @ENV_ID 
			
		if @SERVICE_IDS <> ''
		Begin
			WHILE CHARINDEX(@Character, @Input) > 0
			Begin
				set @tempGroupDetailsID = 0
				SET @EndIndex = CHARINDEX(@Character, @Input)
				set @tempConfigID = SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
				--print @tempConfigID --SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
				select @tempGroupDetailsID = [GROUP_DETAIL_ID] from CSM_GROUP_DETAIL
					where GROUP_ID = @GROUP_ID 
						and ENV_ID in (select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = @tempConfigID) 
						and CONFIG_ID = @tempConfigID
				--print '@tempGroupDetailsID :' + Convert(varchar(10),@tempGroupDetailsID)
				if @tempGroupDetailsID > 0 
				Begin
					update CSM_GROUP_DETAIL set [GROUP_ISACTIVE] = 'true' 
					where GROUP_DETAIL_ID = @tempGroupDetailsID
				End						
				else
				Begin
					insert into [CSM_GROUP_DETAIL]
					(
						[GROUP_ID]
						,[ENV_ID]
						,[CONFIG_ID]
						,[WIN_SERVICE_ID]
						,[GROUP_CREATED_BY]
						,[GROUP_CREATED_DATE]
						,[GROUP_DETAIL_COMMENTS]
						,[GROUP_ISACTIVE]
					)
					values
					(	
						@GROUP_ID
						,(select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = @tempConfigID)
						,@tempConfigID
						,@WIN_SERVICE_ID
						,@GROUP_CREATED_BY
						,@GROUP_CREATED_DATE
						,@GROUP_DETAIL_COMMENTS
						,'True'
					)
				End
											
				SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
			End		
		End
	End
End




GO


