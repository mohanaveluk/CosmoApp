/****** Object:  StoredProcedure [dbo].[CWT_GetGroupScheduleServceDetails]    Script Date: 08/09/2015 00:19:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetGroupScheduleServceDetails]
(
	 @GROUP_SCH_ID int
	,@Category varchar(10)
	,@ENV_ID int
	,@GROUP_SCH_STATUS char(1)
)
As
Begin
	if @Category = 'env'
	Begin
	SELECT  distinct cgs.[GROUP_SCH_ID]
		  ,cgs.[GROUP_ID]
		  ,cgsd.ENV_ID
		  ,env.ENV_NAME

	  FROM [CSM_GROUP_SCHEDULE] cgs
	  join [CSM_GROUP_SCHEDULE_DETAIL] cgsd on cgsd.[GROUP_SCH_ID] = cgs.[GROUP_SCH_ID]
	  --join [CSM_CONFIGURATION] con on con.CONFIG_ID = cgsd.CONFIG_ID
	  join [CSM_ENVIRONEMENT] env on env.ENV_ID = cgsd.ENV_ID
	  join [CSM_WINDOWS_SERVICES] win on win.CONFIG_ID = cgsd.CONFIG_ID
	  where cgs.[GROUP_SCH_ID] = @GROUP_SCH_ID
			--and con.CONFIG_IS_ACTIVE = 'true'
			--and cgsd.ENV_ID = @ENV_ID
	End		
	else if @Category = 'cfg'
	Begin
	SELECT  cgs.[GROUP_SCH_ID]
		  ,cgs.[GROUP_ID]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
		  ,cgs.[GROUP_SCH_ACTION]
		  ,cgs.[GROUP_SCH_STATUS]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_COMPLETED_TIME], 120) [GROUP_SCH_COMPLETED_TIME]
		  ,cgs.[GROUP_SCH_COMMENTS]
		  ,cgs.[GROUP_SCH_CREATED_BY]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_CREATED_DATETIME], 120) [GROUP_SCH_CREATED_DATETIME]
		  ,cgs.[GROUP_SCH_UPDATED_BY]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_UPDATED_DATETIME], 120) [GROUP_SCH_UPDATED_DATETIME]
		  ,cgs.GROUP_SCH_REQUESTSOURCE
		  ,cgsd.GROUP_SERVICE_SCH_ID
		  ,cgsd.CONFIG_ID
		  ,cgsd.GROUP_SCH_ISACTIVE
		  ,cgsd.GROUP_SCH_STATUS
		  ,con.CONFIG_SERVICE_TYPE
		  ,con.CONFIG_HOST_IP
		  ,con.CONFIG_PORT_NUMBER
		  ,con.ENV_ID
		  ,env.ENV_NAME
		  ,win.WIN_SERVICE_ID
		  ,win.WIN_SERVICENAME
		  ,usr.USER_FIRST_NAME USERFIRSTNAME
		  ,usr.USER_LAST_NAME USERLASTNAME
	  FROM [CSM_GROUP_SCHEDULE] cgs
	  join [CSM_GROUP_SCHEDULE_DETAIL] cgsd on cgsd.[GROUP_SCH_ID] = cgs.[GROUP_SCH_ID]
	  join [CSM_CONFIGURATION] con on con.CONFIG_ID = cgsd.CONFIG_ID
	  join [CSM_ENVIRONEMENT] env on env.ENV_ID = cgsd.ENV_ID
	  join [CSM_WINDOWS_SERVICES] win on win.CONFIG_ID = con.CONFIG_ID
	  join dbo.CSM_USER usr on usr.USER_ID = cgs.[GROUP_SCH_CREATED_BY]
	  where cgs.[GROUP_SCH_ID] = @GROUP_SCH_ID
			and env.ENV_ID = @ENV_ID
			and cgsd.GROUP_SCH_STATUS = @GROUP_SCH_STATUS
			--and con.CONFIG_IS_ACTIVE = 'true'
	End		
	else if @Category = 'sch'
	Begin
	SELECT  cgs.[GROUP_SCH_ID]
		  ,cgs.[GROUP_ID]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_TIME], 120) [GROUP_SCH_TIME]
		  ,cgs.[GROUP_SCH_ACTION]
		  ,cgs.[GROUP_SCH_STATUS]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_COMPLETED_TIME], 120) [GROUP_SCH_COMPLETED_TIME]
		  ,cgs.[GROUP_SCH_COMMENTS]
		  ,cgs.[GROUP_SCH_CREATED_BY]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_CREATED_DATETIME], 120) [GROUP_SCH_CREATED_DATETIME]
		  ,cgs.[GROUP_SCH_UPDATED_BY]
		  ,CONVERT(VARCHAR(19), cgs.[GROUP_SCH_UPDATED_DATETIME], 120) [GROUP_SCH_UPDATED_DATETIME]
		  ,cgs.GROUP_SCH_REQUESTSOURCE
		  ,grp.GROUP_NAME
		  ,usr.USER_FIRST_NAME USERFIRSTNAME
		  ,usr.USER_LAST_NAME USERLASTNAME
	  FROM [CSM_GROUP_SCHEDULE] cgs
	  join [CSM_GROUP] grp on grp.GROUP_ID = cgs.GROUP_ID
	  join dbo.CSM_USER usr on usr.USER_ID = cgs.[GROUP_SCH_CREATED_BY]
	  where cgs.[GROUP_SCH_ID] = @GROUP_SCH_ID
	End		
End
SET ANSI_NULLS ON

GO