/****** Object:  StoredProcedure [dbo].[CWT_InsIncidentTracking]    Script Date: 2/9/2015 6:16:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsIncidentTracking]
(
		   @MON_ID int
		  ,@ENV_ID int
 		  ,@CONFIG_ID int
		  ,@TRK_ISSUE varchar(max)
		  ,@TRK_SOLUTION varchar(max)
		  ,@TRK_CREATED_BY varchar(20)
		  ,@TRK_CREATED_DATE datetime
		  ,@TRK_COMMENTS varchar(max)
) As
Begin
	Insert into [CSM_INCIDENT]
	(
		   [MON_ID]
		  ,[ENV_ID]
		  ,[CONFIG_ID]
		  ,[TRK_ISSUE]
		  ,[TRK_SOLUTION]
		  ,[TRK_CREATED_BY]
		  ,[TRK_CREATED_DATE]
		  ,[TRK_COMMENTS]
	  )
	  values
	  (
		   @MON_ID
		  ,@ENV_ID
		  ,@CONFIG_ID
		  ,@TRK_ISSUE
		  ,@TRK_SOLUTION
		  ,@TRK_CREATED_BY
		  ,@TRK_CREATED_DATE
		  ,@TRK_COMMENTS
	  )
End