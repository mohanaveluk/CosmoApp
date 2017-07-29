SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*
select * from  [dbo].[CSM_URLCONFIGURATION] uc

delete from [dbo].[CSM_URLCONFIGURATION]
*/
CREATE procedure [dbo].[CWT_InsUpdUrlConfiguration]
(
	@CATEGORY varchar(10)
	,@URL_ID int
	,@ENVID int
	,@URL_TYPE varchar(50)
	,@URL_ADDRESS varchar(500)
	,@URL_DISPLAYNAME varchar(500)
	,@URL_MATCHCONTENT varchar(500)
	,@URL_INTERVAL int
	,@URL_USERNAME varchar(500)
	,@URL_PASSWORD varchar(500)
	,@URL_STATUS bit
	,@URL_CREATEDBY varchar(100)
	,@URL_UPDATEDBY varchar(100)
	,@URL_COMMENTS varchar(max)
	,@SCOPE_OUTPUT int output
)
As 
Declare
	@urlId int
Begin
	select @urlId  = URL_ID from [dbo].[CSM_URLCONFIGURATION] where lower([URL_ADDRESS]) = lower(@URL_ADDRESS)
	
	if @CATEGORY = 'add_en' and (@URL_ID = 0 or @URL_ID = '')
	Begin
		insert into [dbo].[CSM_URLCONFIGURATION] (
				[ENV_ID]
			  ,[URL_TYPE]
			  ,[URL_ADDRESS]
			  ,[URL_DISPLAYNAME]
			  ,[URL_MATCHCONTENT]
			  ,[URL_INTERVAL]
			  ,[URL_USERNAME]
			  ,[URL_PASSWORD]
			  ,[URL_STATUS]
			  ,[URL_CREATEDBY]
			  ,[URL_COMMENTS]
			  )
			  values
			  (
				  @ENVID
				  ,@URL_TYPE
				  ,@URL_ADDRESS
				  ,@URL_DISPLAYNAME
				  ,@URL_MATCHCONTENT
				  ,@URL_INTERVAL
				  ,@URL_USERNAME
				  ,@URL_PASSWORD
				  ,@URL_STATUS
				  ,@URL_CREATEDBY
				  ,@URL_COMMENTS
			  )
			  set @SCOPE_OUTPUT = IDENT_CURRENT('CSM_URLCONFIGURATION')
	End
	else if @URL_ID > 0 and @CATEGORY = 'modify_ed'
	Begin
		update [dbo].[CSM_URLCONFIGURATION] set 
			[URL_TYPE] = @URL_TYPE
			,[URL_ADDRESS] = @URL_ADDRESS
			,[URL_DISPLAYNAME] = @URL_DISPLAYNAME
			,[URL_MATCHCONTENT] = @URL_MATCHCONTENT
			,[URL_INTERVAL] = @URL_INTERVAL
			,[URL_USERNAME] = @URL_USERNAME
			,[URL_PASSWORD] = @URL_PASSWORD
			,[URL_STATUS] = @URL_STATUS
			,[URL_UPDATEDBY] = @URL_UPDATEDBY
			,[URL_UPDATEDDATE] = GETDATE()
			,[URL_COMMENTS] = @URL_COMMENTS
		where URL_ID = @URL_ID
		set @SCOPE_OUTPUT = @URL_ID
	End
End			  
GO


