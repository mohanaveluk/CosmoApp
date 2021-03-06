/****** Object:  StoredProcedure [dbo].[CWT_getUserEmailList]    Script Date: 2/9/2015 6:16:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


  Create procedure [dbo].[CWT_GetUserEmailList](@Env_ID int, @MessageType varchar) As
  Begin
	/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [USRLST_ID]
      ,[ENV_ID]
      ,[USRLST_EMAIL_ADDRESS]
      ,[USRLST_TYPE]
      ,[USRLST_IS_ACTIVE]
      ,[USRLST_CREATED_BY]
      ,[USRLST_CREATED_DATE]
      ,[USRLST_UPDATED_BY]
      ,[USRLST_UPDATED_DATE]
      ,[USRLST_COMMENTS]
  FROM [CSM_EMAIL_USERLIST]
  Where [ENV_ID] = @Env_ID
		AND [USRLST_MESSAGETYPE] = @MessageType
		AND [USRLST_IS_ACTIVE] = 'True'
  End
SET ANSI_NULLS ON

GO
  