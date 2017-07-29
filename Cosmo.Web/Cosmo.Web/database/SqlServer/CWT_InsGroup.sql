/****** Object:  StoredProcedure [dbo].[CWT_InsGroup]    Script Date: 06/26/2015 14:20:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[CWT_InsGroup]
(
	@GROUP_ID int
    ,@GROUP_NAME varchar(200)
    ,@GROUP_CREATED_BY varchar(20)
    ,@GROUP_CREATED_DATE datetime
    ,@GROUP_COMMENTS varchar(2000)
    ,@GROUP_ISACTIVE bit
) As
Declare 
	@tempGroupID int
Begin

	if @GROUP_ID is null or @GROUP_ID =0
	Begin
		select @tempGroupID = GROUP_ID from CSM_GROUP where lower(GROUP_NAME) = LOWER(@GROUP_NAME)
		if @tempGroupID is null	or @tempGroupID = 0
		Begin
		Insert into CSM_GROUP 
		(
		  [GROUP_NAME]
		  ,[GROUP_CREATED_BY]
		  ,[GROUP_CREATED_DATE]
		  ,[GROUP_COMMENTS]
		  ,[GROUP_IS_ACTIVE]
		)
		values
		(
			@GROUP_NAME
			,@GROUP_CREATED_BY
			,@GROUP_CREATED_DATE
			,@GROUP_COMMENTS
			,@GROUP_ISACTIVE
		)
		End
		else
		Begin
			Update CSM_GROUP set  
			[GROUP_NAME] = @GROUP_NAME
			,[GROUP_UPDATED_BY] = @GROUP_CREATED_BY
			,[GROUP_UPDATED_DATE] = @GROUP_CREATED_DATE
			,[GROUP_COMMENTS] = @GROUP_COMMENTS
			,[GROUP_IS_ACTIVE] = @GROUP_ISACTIVE
			where GROUP_id = @GROUP_ID
		End
	End
	Else
	Begin
		Update CSM_GROUP set
			[GROUP_NAME] = @GROUP_NAME
		  ,[GROUP_UPDATED_BY] = @GROUP_CREATED_BY
		  ,[GROUP_UPDATED_DATE] = @GROUP_CREATED_DATE
		  ,[GROUP_COMMENTS] = @GROUP_COMMENTS
		  ,[GROUP_IS_ACTIVE] = @GROUP_ISACTIVE
		where GROUP_id = @GROUP_ID
	End

End
GO


