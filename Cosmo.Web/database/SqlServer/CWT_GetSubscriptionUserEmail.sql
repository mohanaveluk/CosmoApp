SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


  CREATE Procedure [dbo].[CWT_GetSubscriptionUserEmail](@EnvId int) 
  As
  Begin
	if @EnvId > 0
	Begin
		Select distinct 
			eu.USRLST_EMAIL_ADDRESS
			,rsd.SUBSCRIPTION_ID
			,rs.SUBSCRIPTION_TYPE
			,rs.SUBSCRIPTION_TIME
			,rs.SUBSCRIPTION_ISACTIVE
			,rsd.SUBSCRIPTION_DETAIL_ID
			,rsd.[SUBSCRIPTION_DETAIL_ID]
			,rsd.[USRLST_ID] as [SUBSCRIPTION_USRLST_ID]
			,rsd.[SUBSCRIPTION_EMAILID]
		from [dbo].[CSM_EMAIL_USERLIST] eu
		LEFT JOIN [dbo].[CSM_REPORT_SUBSCRIPTION_DETAILS] rsd 
		on rsd.SUBSCRIPTION_EMAILID = eu.USRLST_EMAIL_ADDRESS
		LEFT JOIN  [dbo].[CSM_REPORT_SUBSCRIPTION] rs 
  		ON rs.[SUBSCRIPTION_ID] = rsd.[SUBSCRIPTION_ID]
  		LEFT JOIN  [dbo].CSM_ENVIRONEMENT en
  		on en.ENV_ID = eu.ENV_ID
  		where en.ENV_ISACTIVE = 1
  		and eu.[USRLST_IS_ACTIVE] = 1	
  		and en.ENV_ID = @EnvId	
		order by [USRLST_EMAIL_ADDRESS]			
	End
	Else
	Begin
		Select distinct 
			eu.USRLST_EMAIL_ADDRESS
			,rsd.SUBSCRIPTION_ID
			,rs.SUBSCRIPTION_TYPE
			,rs.SUBSCRIPTION_TIME
			,rs.SUBSCRIPTION_ISACTIVE
			,rsd.SUBSCRIPTION_DETAIL_ID
			,rsd.[USRLST_ID] as [SUBSCRIPTION_USRLST_ID]
			,rsd.[SUBSCRIPTION_EMAILID]
		from [dbo].[CSM_EMAIL_USERLIST] eu
		LEFT JOIN [dbo].[CSM_REPORT_SUBSCRIPTION_DETAILS] rsd 
		on rsd.SUBSCRIPTION_EMAILID = eu.USRLST_EMAIL_ADDRESS
		LEFT JOIN  [dbo].[CSM_REPORT_SUBSCRIPTION] rs 
  		ON rs.[SUBSCRIPTION_ID] = rsd.[SUBSCRIPTION_ID]
  		LEFT JOIN  [dbo].CSM_ENVIRONEMENT en
  		on en.ENV_ID = eu.ENV_ID
  		where en.ENV_ISACTIVE = 1
  		and eu.[USRLST_IS_ACTIVE] = 1		
		order by [USRLST_EMAIL_ADDRESS]
	End
  End	

GO
