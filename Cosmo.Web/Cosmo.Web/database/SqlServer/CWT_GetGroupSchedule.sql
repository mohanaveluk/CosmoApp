SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[CWT_GetGroupSchedule]
(
	@GROUP_ID int
	,@GROUP_SCH_ID int
	,@CURRENTDATETIME datetime
)
As
Begin
	--Need to add case where history of group schedule
	
	if @GROUP_SCH_ID > 0 
	Begin
		SELECT  
			   grpSch.[GROUP_SCH_ID]
			  ,grp.[GROUP_ID]
			  ,grp.GROUP_NAME
			  ,grp.GROUP_COMMENTS
			  ,CONVERT(VARCHAR(19), grpSch.[GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
			  ,grpSch.[GROUP_SCH_ACTION]
			  ,grpSch.[GROUP_SCH_STATUS]
			  ,CONVERT(VARCHAR(19), grpSch.[GROUP_SCH_COMPLETED_TIME], 120) [GROUP_SCH_COMPLETED_TIME]
			  ,grpSch.[GROUP_SCH_COMMENTS]
			  ,grpSch.[GROUP_SCH_CREATED_BY]
			  ,grpSch.[GROUP_SCH_CREATED_DATETIME]
			  ,grpSch.[GROUP_SCH_UPDATED_BY]
			  ,grpSch.[GROUP_SCH_UPDATED_DATETIME]			  
			  ,grpSch.GROUP_SCH_REQUESTSOURCE
			  ,usr.USER_FIRST_NAME USERFIRSTNAME
			  ,usr.USER_LAST_NAME USERLASTNAME
		  FROM [CSM_GROUP_SCHEDULE] grpSch
		  right outer join [CSM_GROUP] grp on grp.GROUP_ID = grpSch.GROUP_ID
				and [GROUP_SCH_TIME] >= CONVERT(VARCHAR(19), @CURRENTDATETIME, 120)
				and grp.GROUP_IS_ACTIVE = 'True'
				and grpSch.GROUP_SCH_ISACTIVE = 'True'
				and [GROUP_SCH_ID] in
				(
					select [GROUP_SCH_ID] from [CSM_GROUP_SCHEDULE_DETAIL] 
					where CONFIG_ID in
					(
						select CONFIG_ID from CSM_CONFIGURATION 
						where CONFIG_IS_ACTIVE = 'True'
					)
				)
			left join dbo.CSM_USER usr on usr.USER_ID = grpSch.GROUP_SCH_CREATED_BY
			where [GROUP_SCH_ID] = @GROUP_SCH_ID
			and grp.GROUP_NAME <> 'OnDemand'
	End
	else
	Begin
		if @GROUP_ID > 0
		Begin
			SELECT  
				   grpSch.[GROUP_SCH_ID]
				  ,grp.[GROUP_ID]
				  ,grp.GROUP_NAME
				  ,grp.GROUP_COMMENTS
				  ,CONVERT(VARCHAR(19), grpSch.[GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
				  ,grpSch.[GROUP_SCH_ACTION]
				  ,grpSch.[GROUP_SCH_STATUS]
				  ,CONVERT(VARCHAR(19), grpSch.[GROUP_SCH_COMPLETED_TIME], 120) [GROUP_SCH_COMPLETED_TIME]
				  ,[GROUP_SCH_COMMENTS]
				  ,grpSch.[GROUP_SCH_CREATED_BY]
				  ,grpSch.[GROUP_SCH_CREATED_DATETIME]
				  ,grpSch.[GROUP_SCH_UPDATED_BY]
				  ,grpSch.[GROUP_SCH_UPDATED_DATETIME]	
				  ,grpSch.GROUP_SCH_REQUESTSOURCE		  
				  ,usr.USER_FIRST_NAME USERFIRSTNAME
				  ,usr.USER_LAST_NAME USERLASTNAME
			  FROM [CSM_GROUP_SCHEDULE] grpSch
			  right outer join [CSM_GROUP] grp on grp.GROUP_ID = grpSch.GROUP_ID
					and [GROUP_SCH_TIME] >= CONVERT(VARCHAR(19), @CURRENTDATETIME, 120)
					and grp.GROUP_IS_ACTIVE = 'True'	
					and grpSch.GROUP_SCH_ISACTIVE = 'True'
					and [GROUP_SCH_ID] in
					(
						select [GROUP_SCH_ID] from [CSM_GROUP_SCHEDULE_DETAIL] 
						where CONFIG_ID in
						(
							select CONFIG_ID from CSM_CONFIGURATION 
							where CONFIG_IS_ACTIVE = 'True'
						)
					)
				left join dbo.CSM_USER usr on usr.USER_ID = grpSch.GROUP_SCH_CREATED_BY					
				where grp.GROUP_ID = @GROUP_ID
				and grp.GROUP_NAME <> 'OnDemand'
		End
		else
		Begin
			SELECT  
				   grpSch.[GROUP_SCH_ID]
				  ,grp.[GROUP_ID]
				  ,grp.GROUP_NAME
				  ,grp.GROUP_COMMENTS
				  ,CONVERT(VARCHAR(19), grpSch.[GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
				  ,grpSch.[GROUP_SCH_ACTION]
				  ,grpSch.[GROUP_SCH_STATUS]
				  ,CONVERT(VARCHAR(19), grpSch.[GROUP_SCH_COMPLETED_TIME], 120) [GROUP_SCH_COMPLETED_TIME]
				  ,[GROUP_SCH_COMMENTS]
				  ,grpSch.[GROUP_SCH_CREATED_BY]
				  ,grpSch.[GROUP_SCH_CREATED_DATETIME]
				  ,grpSch.[GROUP_SCH_UPDATED_BY]
				  ,grpSch.[GROUP_SCH_UPDATED_DATETIME]	
				  ,grpSch.GROUP_SCH_REQUESTSOURCE
				  ,usr.USER_FIRST_NAME USERFIRSTNAME
				  ,usr.USER_LAST_NAME USERLASTNAME
			  FROM [CSM_GROUP_SCHEDULE] grpSch 
			  right OUTER join [CSM_GROUP] grp on grp.GROUP_ID = grpSch.GROUP_ID
					and [GROUP_SCH_TIME] >= CONVERT(VARCHAR(19), @CURRENTDATETIME, 120)
					and grp.GROUP_IS_ACTIVE = 'True'	
					and grpSch.GROUP_SCH_ISACTIVE = 'True'
					and grpSch.GROUP_SCH_STATUS <> 'C'
					and [GROUP_SCH_ID] in
					(
						select [GROUP_SCH_ID] from [CSM_GROUP_SCHEDULE_DETAIL] 
						where CONFIG_ID in
						(
							select CONFIG_ID from CSM_CONFIGURATION 
							where CONFIG_IS_ACTIVE = 'True'
						)
					)
				left join dbo.CSM_USER usr on usr.USER_ID = grpSch.GROUP_SCH_CREATED_BY
				where grp.GROUP_NAME <> 'OnDemand'
		End	
	End						
End		


GO




