SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_ISUrlConfigExists]
(
	@ENVID int
	,@URL_TYPE varchar(max)
	,@URL_ADDRESS varchar(max)
	,@SCOPE_OUTPUT varchar(max) output
) As
Declare 
	@urlId varchar(10)
	,@Environtment_Name varchar(max)
	,@Environtment_ID varchar(10)
	
Begin try

	select @urlId = con.[URL_ID] 
		,@Environtment_Name = env.ENV_NAME
		from [CSM_URLCONFIGURATION] con
		inner join [CSM_ENVIRONEMENT] env 
		on env.ENV_ID = con.ENV_ID
		where lower(URL_TYPE) = lower(@URL_TYPE)
		and con.ENV_ID = @ENVID
		and URL_ISACTIVE = 'True'
	--
	if @urlId > 0
	Begin
		print @urlId
		set @SCOPE_OUTPUT = 'Url Configuration already exists under ' + @Environtment_Name + ' with Type ' + @URL_TYPE + ', Please click on edit icon to update details|' + @URL_TYPE + '|' + @urlId
		print @SCOPE_OUTPUT
	End

End try
begin catch
 DECLARE
  @ErMessage NVARCHAR(2048),
  @ErSeverity INT,
  @ErState INT
  
SELECT
  @ErMessage = ERROR_MESSAGE(),
  @ErSeverity = ERROR_SEVERITY(),
  @ErState = ERROR_STATE()
  
  RAISERROR(@ErMessage,@ErSeverity,@ErState)
end catch

/*
declare
@SCOPE_OUTPUT varchar(1000)
exec [CWT_ISUrlConfigExists] 1, 'Cognos Portal URL', '', @SCOPE_OUTPUT output
select @SCOPE_OUTPUT
*/

GO


