SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


  CREATE procedure [dbo].[CWT_InsUpdServerPerformanceDrive]
  (
	   @PER_ID int
      ,@DRIVE_NAME varchar(50)
      ,@DRIVE_LABEL varchar(100)
      ,@DRIVE_FORMAT varchar(50)
      ,@DRIVE_TYPE varchar(50)
      ,@DRIVE_FREESPACE float
      ,@DRIVE_USEDSPACE float
      ,@DRIVE_TOTALSPACE float
      ,@DRIVE_COMMENTS varchar(max)
  )
  As
  Begin
	insert into [dbo].[CSM_SERVERPERFORMANCE_DRIVE]
	(
		[PER_ID]
      ,[DRIVE_NAME]
      ,[DRIVE_LABEL]
      ,[DRIVE_FORMAT]
      ,[DRIVE_TYPE]
      ,[DRIVE_FREESPACE]
      ,[DRIVE_USEDSPACE]
      ,[DRIVE_TOTALSPACE]
      ,[DRIVE_COMMENTS]
	)
	values
	(
		@PER_ID
      ,@DRIVE_NAME
      ,@DRIVE_LABEL
      ,@DRIVE_FORMAT
      ,@DRIVE_TYPE
      ,@DRIVE_FREESPACE
      ,@DRIVE_USEDSPACE
      ,@DRIVE_TOTALSPACE
      ,@DRIVE_COMMENTS
	)
  End
  
GO


