/****** Object:  StoredProcedure [dbo].[CWT_GetDispatcherConfigID]    Script Date: 2/9/2015 6:08:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetDispatcherConfigID](@CONFIGREFID int, @CMURL varchar(500), @STYPE varchar(10)) As
Begin
	if(@STYPE='REFID')
	Begin
		Select CONFIG_ID from CSM_CONFIGURATION where CONFIG_REF_ID = @CONFIGREFID
	End
	else if(@STYPE='CMURL')
	Begin
		Select CONFIG_ID from CSM_CONFIGURATION where CONFIG_URL_ADDRESS like @CMURL +'%' and CONFIG_URL_ADDRESS != @CMURL
	End
End


--Exec [CWT_GetDispatcherConfigID] 1,'http://snowflake:9300/p2pd/servlet','CMURL'