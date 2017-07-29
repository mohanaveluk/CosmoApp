/****** Object:  StoredProcedure [dbo].[CWT_GetGroupID]    Script Date: 08/09/2015 00:14:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[CWT_GetGroupID](@GROUP_NAME varchar(MAX)) AS
  Begin
	Select 
		GROUP_ID, 
		GROUP_NAME,
		GROUP_COMMENTS 
		from CSM_GROUP 
		where GROUP_IS_ACTIVE = 'true'
			and GROUP_NAME = @GROUP_NAME
  End

GO


