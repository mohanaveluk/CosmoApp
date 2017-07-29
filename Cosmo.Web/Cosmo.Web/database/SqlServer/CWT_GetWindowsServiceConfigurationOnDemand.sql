/****** Object:  StoredProcedure [dbo].[CWT_GetWindowsServiceConfigurationOnDemand]    Script Date: 08/23/2015 18:56:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


Create procedure [dbo].[CWT_GetWindowsServiceConfigurationOnDemand](@WIN_SERVICE_ID int) As
Begin
	SELECT ws.[WIN_SERVICE_ID]
      ,ws.[ENV_ID]
      ,ws.[CONFIG_ID]
      ,ws.[WIN_SERVICENAME]
      ,env.ENV_ID
      ,env.ENV_NAME
      ,con.CONFIG_HOST_IP
      ,con.CONFIG_PORT_NUMBER
      ,con.CONFIG_SERVICE_TYPE
      ,(case con.CONFIG_SERVICE_TYPE when '1' then 'Content Manager' when '2' then 'Dispatcher' end) SERVICETYPE
  FROM [CSM_WINDOWS_SERVICES] ws
  inner join CSM_CONFIGURATION con on con.CONFIG_ID = ws.CONFIG_ID
  inner join CSM_ENVIRONEMENT env on env.ENV_ID = con.ENV_ID
  where ws.WIN_SERVICE_ID = @WIN_SERVICE_ID
  
ENd  
GO


