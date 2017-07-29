SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE procedure [dbo].[CWT_GetUrlPerformance](@ENVID int)
As
Begin
	if @ENVID > 0
	Begin
		select uc.[URL_ID]
				  ,uc.[ENV_ID]
				  ,env.ENV_NAME
				  ,uc.[URL_TYPE]
				  ,uc.[URL_ADDRESS]
				  ,uc.[URL_DISPLAYNAME]
				  ,(select top 1 CONVERT(DECIMAL(5,2), pm.PMON_RESPONSETIME/1000.0) as ResponseTime	
					from dbo.[CSM_PORTALMONITOR] pm 
					where pm.ENV_ID = uc.[ENV_ID]
					and pm.URL_ID in (uc.[URL_ID])
					and pm.PMON_RESPONSETIME > 0 
					order by pm.PMON_CREATEDDATE desc) RESPONSETIME
				  ,(select CONVERT(DECIMAL(5,2), avg(pm.PMON_RESPONSETIME)/1000.0) from dbo.[CSM_PORTALMONITOR] pm 
					where pm.PMON_RESPONSETIME > 0 and 
					pm.PMON_CREATEDDATE >= dateadd(hour, -1,GETDATE()) 
					and  pm.PMON_CREATEDDATE <= GETDATE()
					and pm.ENV_ID = uc.[ENV_ID]
					and pm.URL_ID in (uc.[URL_ID])) 	RESPONSETIMEINHOUR,
					(select max(pm.PMON_CREATEDDATE) from dbo.[CSM_PORTALMONITOR] pm
					where  pm.ENV_ID = uc.[ENV_ID]
					and pm.URL_ID in (uc.[URL_ID])) LASTPINGDATETIME
		from dbo.[CSM_URLCONFIGURATION] uc
		left join dbo.CSM_ENVIRONEMENT env
			  On env.ENV_ID = uc.ENV_ID
			  where uc.[ENV_ID] = @ENVID
			  
	End
	else
	Begin
		select uc.[URL_ID]
				  ,uc.[ENV_ID]
				  ,env.ENV_NAME
				  ,uc.[URL_TYPE]
				  ,uc.[URL_ADDRESS]
				  ,uc.[URL_DISPLAYNAME]
				  ,(select top 1 CONVERT(DECIMAL(5,2), pm.PMON_RESPONSETIME/1000.0) as ResponseTime	
					from dbo.[CSM_PORTALMONITOR] pm 
					where pm.ENV_ID = uc.[ENV_ID]
					and pm.URL_ID in (uc.[URL_ID])
					and pm.PMON_RESPONSETIME > 0
					order by pm.PMON_CREATEDDATE desc) RESPONSETIME
				  ,(select CONVERT(DECIMAL(5,2), avg(pm.PMON_RESPONSETIME)/1000.0) from dbo.[CSM_PORTALMONITOR] pm 
					where pm.PMON_RESPONSETIME > 0 and 
					pm.PMON_CREATEDDATE >= dateadd(hour, -1,GETDATE()) 
					and  pm.PMON_CREATEDDATE <= GETDATE()
					and pm.ENV_ID = uc.[ENV_ID]
					and pm.URL_ID in (uc.[URL_ID])) 	RESPONSETIMEINHOUR,
					(select max(pm.PMON_CREATEDDATE) from dbo.[CSM_PORTALMONITOR] pm
					where  pm.ENV_ID = uc.[ENV_ID]
					and pm.URL_ID in (uc.[URL_ID])) LASTPINGDATETIME 
		from dbo.[CSM_URLCONFIGURATION] uc
		left join dbo.CSM_ENVIRONEMENT env
			  On env.ENV_ID = uc.ENV_ID
	End
End


GO


