/****** Object:  StoredProcedure [dbo].[CWT_InsertCSMLog]    Script Date: 2/9/2015 6:16:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF (OBJECT_ID('CWT_InsertCSMLog') IS NOT NULL)
  DROP PROCEDURE CWT_InsertCSMLog
GO

  CREATE procedure [dbo].[CWT_InsertCSMLog](
	@SCH_ID int,
	@CONFIG_ID int,
	@ENV_ID int ,
	@LOGDESCRIPTION varchar(1000),
	@LOGERROR varchar(1000),
	@LOG_UPDATE_TATETIME datetime,
	@LOG_UPDATED_BY varchar(100)
  )
  AS
  Begin
  	insert into [CSM_LOG] (
		[SCH_ID]
		,[CONFIG_ID]
		,[ENV_ID]
		,[LOGDESCRIPTION]
		,[LOGERROR]
		,LOG_UPDATED_DATETIME
		,[LOG_UPDATED_BY]
      ) values(
		@SCH_ID,
		@CONFIG_ID,
		@ENV_ID,
		@LOGDESCRIPTION,
		@LOGERROR,
		@LOG_UPDATE_TATETIME,
		@LOG_UPDATED_BY      
	  )
  End
  