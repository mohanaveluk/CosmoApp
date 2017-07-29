/****** Object:  StoredProcedure [dbo].[CWT_GetGroupDetail]    Script Date: 06/26/2015 14:18:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetGroupDetail]
(
	@GROUP_ID int
	,@ENV_ID int
)As
Begin
	if @ENV_ID  > 0
	Begin
	  select 
		cgd.GROUP_DETAIL_ID
		,cgd.GROUP_ID 
		,cg.GROUP_NAME
		,cgd.ENV_ID
		,env.ENV_NAME
		,cgd.CONFIG_ID
		,cgd.GROUP_DETAIL_COMMENTS
		,con.CONFIG_HOST_IP
		,con.CONFIG_PORT_NUMBER
		,con.CONFIG_ISPRIMARY
		,con.CONFIG_LOCATION
		,con.CONFIG_SERVICE_TYPE
		,cws.WIN_SERVICE_ID
		,cws.WIN_SERVICENAME
	    ,usr.USER_FIRST_NAME USERFIRSTNAME
		,usr.USER_LAST_NAME USERLASTNAME
		from [CSM_GROUP_DETAIL] cgd
		inner join CSM_GROUP cg on cg.GROUP_ID = cgd.GROUP_ID
		inner join CSM_CONFIGURATION con on cgd.CONFIG_ID = con.CONFIG_ID
		inner join CSM_ENVIRONEMENT env on env.ENV_ID = cgd.ENV_ID
		left outer join CSM_WINDOWS_SERVICES cws on cws.CONFIG_ID = cgd.CONFIG_ID and cws.ENV_ID = con.ENV_ID
		join dbo.CSM_USER usr on usr.USER_ID = [CONFIG_CREATED_BY]
		where 
			cgd.GROUP_ISACTIVE = 'True'
			and cgd.GROUP_ID = @GROUP_ID
			and cgd.ENV_ID = @ENV_ID
			and con.CONFIG_ISPRIMARY = 'True'
		order by cg.GROUP_ID, con.CONFIG_HOST_IP, con.CONFIG_PORT_NUMBER
	End	
	else
	Begin
	  select 
		cgd.GROUP_DETAIL_ID
		,cgd.GROUP_ID 
		,cg.GROUP_NAME
		,cgd.ENV_ID
		,env.ENV_NAME
		,cgd.CONFIG_ID
		,cgd.GROUP_DETAIL_COMMENTS
		,con.CONFIG_HOST_IP
		,con.CONFIG_PORT_NUMBER
		,con.CONFIG_ISPRIMARY
		,con.CONFIG_LOCATION
		,con.CONFIG_SERVICE_TYPE
		,cws.WIN_SERVICE_ID
		,cws.WIN_SERVICENAME
	    ,usr.USER_FIRST_NAME USERFIRSTNAME
	    ,usr.USER_LAST_NAME USERLASTNAME
		from [CSM_GROUP_DETAIL] cgd
		inner join CSM_GROUP cg on cg.GROUP_ID = cgd.GROUP_ID
		inner join CSM_CONFIGURATION con on cgd.CONFIG_ID = con.CONFIG_ID
		inner join CSM_ENVIRONEMENT env on env.ENV_ID = cgd.ENV_ID
		left outer join CSM_WINDOWS_SERVICES cws on cws.CONFIG_ID = cgd.CONFIG_ID
		join dbo.CSM_USER usr on usr.USER_ID = [CONFIG_CREATED_BY]
		where 
			cgd.GROUP_ISACTIVE = 'True'
			and cgd.GROUP_ID = @GROUP_ID
			and con.CONFIG_ISPRIMARY = 'True'	
		order by cg.GROUP_ID, con.CONFIG_HOST_IP, con.CONFIG_PORT_NUMBER
	End
End

--exec [CWT_GetGroupDetail] 18,0

GO


