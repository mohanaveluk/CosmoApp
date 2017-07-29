/****** Object:  StoredProcedure [dbo].[CWT_GetEnvID]    Script Date: 2/9/2015 6:13:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetEnvID]
(
      @ENV_NAME varchar(100)
      --@ENV_HOST_IP_ADDRESS varchar(100),
      --@ENV_LOCATION varchar(100)
)
as
Begin
		select [ENV_ID] from [CSM_ENVIRONEMENT] 
			where lower([ENV_NAME]) = lower(@ENV_NAME) and 
			--[ENV_HOST_IP_ADDRESS] = @ENV_HOST_IP_ADDRESS and 
			--[ENV_LOCATION] = @ENV_LOCATION and
			ENV_ISACTIVE = 'True'
End

