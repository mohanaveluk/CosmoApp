SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


  Create procedure [dbo].[CWT_InsUpdServerPerformance]
  (
		@ENVID int
      ,@CONFIGID int
      ,@PER_HOSTIP varchar(250)
      ,@PER_CPU_USAGE float
      ,@PER_AVAILABLEMEMORY float
      ,@PER_TOTALMEMORY float
      ,@PER_CREATED_BY varchar(50)
      ,@PER_COMMENTS varchar(max)
      ,@SCOPE_OUTPUT int output
  )
  As
  Begin
	insert into [dbo].[CSM_SERVERPERFORMANCE]
	(
	[ENVID]
      ,[CONFIGID]
      ,[PER_HOSTIP]
      ,[PER_CPU_USAGE]
      ,[PER_AVAILABLEMEMORY]
      ,[PER_TOTALMEMORY]
      ,[PER_CREATED_BY]
      ,[PER_COMMENTS]
      )
	values
	(
		@ENVID
      ,@CONFIGID
      ,@PER_HOSTIP
      ,@PER_CPU_USAGE
      ,@PER_AVAILABLEMEMORY
      ,@PER_TOTALMEMORY
      ,@PER_CREATED_BY
      ,@PER_COMMENTS
     )
      set @SCOPE_OUTPUT = IDENT_CURRENT('CSM_SERVERPERFORMANCE')
      
  End
GO


