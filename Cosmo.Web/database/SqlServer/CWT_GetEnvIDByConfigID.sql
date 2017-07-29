/****** Object:  StoredProcedure [dbo].[CWT_GetEnvIDByConfigID]    Script Date: 10/12/2015 15:49:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


Create procedure [dbo].[CWT_GetEnvIDByConfigID]
(
      @CONFIGID int
      --@ENV_HOST_IP_ADDRESS varchar(100),
      --@ENV_LOCATION varchar(100)
)
as
Begin
		select [ENV_ID] from [CSM_ENVIRONEMENT] 
			where ENV_ID in (select distinct ENV_ID from CSM_CONFIGURATION where CONFIG_ID = @CONFIGID)
			and ENV_ISACTIVE = 'True'
End

/****** Object:  StoredProcedure [dbo].[CWT_GetEnvConfigID]    Script Date: 2/9/2015 6:08:45 PM ******/
SET ANSI_NULLS ON

GO


