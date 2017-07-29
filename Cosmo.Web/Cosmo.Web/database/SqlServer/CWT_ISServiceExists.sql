/****** Object:  StoredProcedure [dbo].[CWT_ISServiceExists]    Script Date: 08/29/2015 17:09:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_ISServiceExists]
(
	@ENV_HOST_IP_ADDRESS varchar(max)
	,@ENV_PORT varchar(max)
	,@SCOPE_OUTPUT varchar(max) output
) As
Declare 
	@EnvirontmentDetails_ID varchar(10)
	,@Environtment_Name varchar(max)
	,@Environtment_ID varchar(10)
	
Begin try

	select @EnvirontmentDetails_ID = con.[CONFIG_ID] 
		,@Environtment_Name = env.ENV_NAME 
		,@Environtment_ID = env.ENV_ID
		from [CSM_CONFIGURATION] con
		inner join [CSM_ENVIRONEMENT] env on env.ENV_ID = con.ENV_ID
		where lower([CONFIG_HOST_IP]) = lower(@ENV_HOST_IP_ADDRESS)
		and lower([CONFIG_PORT_NUMBER]) = lower(@ENV_PORT)
		and CONFIG_IS_ACTIVE = 'True'
		and CONFIG_ISPRIMARY = 'True'

	if @EnvirontmentDetails_ID > 0
	Begin
		set @SCOPE_OUTPUT = 'Service already exists under ' + @Environtment_Name + ' Environment|' + @Environtment_ID + '|' + @EnvirontmentDetails_ID
		--print @SCOPE_OUTPUT
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

--exec CWT_ISServiceExists 'snowflake1','9300','@SCOPE_OUTPUT'

GO


