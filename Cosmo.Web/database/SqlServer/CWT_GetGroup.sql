/****** Object:  StoredProcedure [dbo].[CWT_GetGroup]    Script Date: 06/26/2015 14:18:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetGroup](@GRP_ID int) AS
  Begin
	if @GRP_ID =0 or @GRP_ID is null
	Begin
		Select 
			grp.GROUP_ID, 
			grp.GROUP_NAME,
			grp.GROUP_COMMENTS 
			from CSM_GROUP grp
			where grp.GROUP_IS_ACTIVE = 'true'
				and grp.GROUP_ID in 
				(
					select schedule.GROUP_ID from [dbo].CSM_GROUP_SCHEDULE schedule 
					where 
						(schedule.GROUP_SCH_TIME <= GETDATE() or schedule.GROUP_SCH_TIME is null)
						--and schedule.GROUP_SCH_STATUS <> 'O'
				)
				and grp.GROUP_NAME <> 'OnDemand'
				
		
	End
	else if @GRP_ID > 0
	Begin
		Select 
			grp.GROUP_ID, 
			grp.GROUP_NAME,
			grp.GROUP_COMMENTS 
			from CSM_GROUP grp
			where GROUP_IS_ACTIVE = 'true'
				and grp.GROUP_ID in 
				(
					select schedule.GROUP_ID from [dbo].CSM_GROUP_SCHEDULE schedule 
					where 
						(schedule.GROUP_SCH_TIME <= GETDATE() or schedule.GROUP_SCH_TIME is null)
						--and schedule.GROUP_SCH_STATUS <> 'O'
				)
				and grp.GROUP_ID = @GRP_ID
				and grp.GROUP_NAME <> 'OnDemand'
	End
  End



GO


