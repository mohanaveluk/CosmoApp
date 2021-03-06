
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure  [dbo].[CWT_GetAllConfigEmail](@ENV_ID int) As
Begin
	if @ENV_ID = null or @ENV_ID = 0
		Begin
		SELECT env.ENV_NAME
			  ,[USRLST_ID]
			  ,eml.[ENV_ID]
			  ,[USRLST_EMAIL_ADDRESS]
			  ,[USRLST_MESSAGETYPE]
			  ,[USRLST_TYPE]
			  ,[USRLST_IS_ACTIVE]
			  ,[USRLST_CREATED_BY]
			  ,[USRLST_CREATED_DATE]
			  ,[USRLST_UPDATED_BY]
			  ,[USRLST_UPDATED_DATE]
			  ,[USRLST_COMMENTS]
		  FROM [CSM_EMAIL_USERLIST] eml
		  inner join [CSM_ENVIRONEMENT] env on env.ENV_ID = eml.ENV_ID 
		  where env.ENV_ISACTIVE = 'True'
			and eml.USRLST_IS_ACTIVE = 'True'
			order by env.ENV_NAME
		End
	Else
		Begin
		SELECT env.ENV_NAME
			  ,[USRLST_ID]
			  ,eml.[ENV_ID]
			  ,[USRLST_EMAIL_ADDRESS]
			  ,[USRLST_MESSAGETYPE]
			  ,[USRLST_TYPE]
			  ,[USRLST_IS_ACTIVE]
			  ,[USRLST_CREATED_BY]
			  ,[USRLST_CREATED_DATE]
			  ,[USRLST_UPDATED_BY]
			  ,[USRLST_UPDATED_DATE]
			  ,[USRLST_COMMENTS]
		  FROM [CSM_EMAIL_USERLIST] eml
		  inner join [CSM_ENVIRONEMENT] env on env.ENV_ID = eml.ENV_ID 
		  where env.ENV_ISACTIVE = 'True'
			and eml.USRLST_IS_ACTIVE = 'True'
			and env.ENV_ID = @ENV_ID
			order by env.ENV_NAME
			
		End	
End

SET ANSI_NULLS ON

GO



