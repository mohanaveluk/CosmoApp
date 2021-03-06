/****** Object:  StoredProcedure [dbo].[CWT_GetEnvConfigID]    Script Date: 2/9/2015 6:08:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetEnvConfigID]
(
      @ENV_ID varchar(100),
      @ENV_HOST_IP_ADDRESS varchar(200),
      --@ENV_LOCATION varchar(200),
	  --@ENV_SERVICEURL varchar(500)
	  @ENV_PORT varchar(200),
      @ENV_SERVICETYPE varchar(10)
)
as
Begin
		select [CONFIG_ID] from [CSM_CONFIGURATION] 
			where [ENV_ID] = @ENV_ID and 
			lower([CONFIG_HOST_IP]) = lower(@ENV_HOST_IP_ADDRESS) and 
			--lower([CONFIG_LOCATION]) = lower(@ENV_LOCATION) and
			--lower([CONFIG_URL_ADDRESS]) = lower(@ENV_SERVICEURL) and
			lower([CONFIG_PORT_NUMBER]) = lower(@ENV_PORT) and
			CONFIG_SERVICE_TYPE = @ENV_SERVICETYPE and
			CONFIG_IS_ACTIVE = 'True'
End

