
/****** Object:  StoredProcedure [dbo].[GetDispatcherConfigID]    Script Date: 2/9/2015 6:19:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[GetDispatcherConfigID](@CONFIGREFID int) As
Begin
	Select CONFIG_ID from CSM_CONFIGURATION where CONFIG_REF_ID = @CONFIGREFID
End