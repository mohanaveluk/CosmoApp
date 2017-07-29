SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_GetCurrentBuildReport](@ENV_ID int) As
Declare
	@BuildCount int
Begin

	select @BuildCount = count(*) from dbo.CSM_SERVICEBUILD where ENV_ID = @ENV_ID
	if @BuildCount > 0
	Begin
		select distinct
			mon.CONFIG_ID,
			mon.BUILD_VERSION MON_COMMENTS,
			mon.BUILD_DATE MON_BUILD_DATE,
			CONVERT(VARCHAR(19),mon.CREATED_DATE,120) MON_CREATED_DATE,
			con.CONFIG_SERVICE_TYPE,
			con.CONFIG_PORT_NUMBER,
			con.CONFIG_DESCRIPTION,
			lower(con.CONFIG_HOST_IP) CONFIG_HOST_IP,
			con.CONFIG_LOCATION,
			con.CONFIG_ISPRIMARY
		from [dbo].[CSM_SERVICEBUILD] mon
		inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
		where con.ENV_ID = @Env_ID
		and mon.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [dbo].[CSM_SERVICEBUILD] group by CONFIG_ID)
		--and con.CONFIG_ISPRIMARY = '1'
		order by MON_CREATED_DATE, CONFIG_ID
	End
	Else
	Begin
		select distinct
			mon.CONFIG_ID,
			mon.BUILD_VERSION MON_COMMENTS,
			mon.BUILD_DATE MON_BUILD_DATE,
			CONVERT(VARCHAR(19),mon.CREATED_DATE,120) MON_CREATED_DATE,
			con.CONFIG_SERVICE_TYPE,
			con.CONFIG_PORT_NUMBER,
			con.CONFIG_DESCRIPTION,
			lower(con.CONFIG_HOST_IP) CONFIG_HOST_IP,
			con.CONFIG_LOCATION,
			con.CONFIG_ISPRIMARY
		from [dbo].[CSM_SERVICEBUILD] mon
		inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
		where con.ENV_ID = @Env_ID
		--and con.CONFIG_ISPRIMARY = '1'
		order by MON_CREATED_DATE, CONFIG_ID	End
End
--exec [CWT_GetCurrentBuildReport] 2

GO


