SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/

CREATE procedure [dbo].[CWT_GetUrlConfiguration](@ENVID int, @URLID int) as 
Begin

	if @ENVID > 0
		Begin
			SELECT uc.[URL_ID]
			  ,uc.[ENV_ID]
			  ,env.ENV_NAME
			  ,uc.[URL_TYPE]
			  ,uc.[URL_ADDRESS]
			  ,uc.[URL_DISPLAYNAME]
			  ,uc.[URL_MATCHCONTENT]
			  ,uc.[URL_INTERVAL]
			  ,uc.[URL_USERNAME]
			  ,uc.[URL_PASSWORD]
			  ,uc.[URL_ISACTIVE]
			  ,uc.[URL_STATUS]
			  ,uc.[URL_CREATEDBY]
			  ,uc.[URL_CREATEDDATE]
			  ,uc.[URL_UPDATEDBY]
			  ,uc.[URL_UPDATEDDATE]
			  ,uc.[URL_COMMENTS]
		  FROM [dbo].[CSM_URLCONFIGURATION] uc
		  left join dbo.CSM_ENVIRONEMENT env
		  On env.ENV_ID = uc.ENV_ID
		  where uc.[ENV_ID] = @ENVID
		  and [URL_ISACTIVE] = 1
	  End
	 else if @URLID > 0
		Begin
			SELECT uc.[URL_ID]
			  ,uc.[ENV_ID]
			  ,env.ENV_NAME
			  ,uc.[URL_TYPE]
			  ,uc.[URL_ADDRESS]
			  ,uc.[URL_DISPLAYNAME]
			  ,uc.[URL_MATCHCONTENT]
			  ,uc.[URL_INTERVAL]
			  ,uc.[URL_USERNAME]
			  ,uc.[URL_PASSWORD]
			  ,uc.[URL_STATUS]
			  ,uc.[URL_ISACTIVE]
			  ,uc.[URL_CREATEDBY]
			  ,uc.[URL_CREATEDDATE]
			  ,uc.[URL_UPDATEDBY]
			  ,uc.[URL_UPDATEDDATE]
			  ,uc.[URL_COMMENTS]
		  FROM [dbo].[CSM_URLCONFIGURATION] uc
		  left join dbo.CSM_ENVIRONEMENT env
		  On env.ENV_ID = uc.ENV_ID
		  where [URL_ID] = @URLID
		  and [URL_ISACTIVE] = 1
	  End
	 else if @ENVID <= 0 and @URLID <= 0
		Begin
			SELECT uc.[URL_ID]
			  ,uc.[ENV_ID]
			  ,env.ENV_NAME
			  ,uc.[URL_TYPE]
			  ,uc.[URL_ADDRESS]
			  ,uc.[URL_DISPLAYNAME]
			  ,uc.[URL_MATCHCONTENT]
			  ,uc.[URL_INTERVAL]
			  ,uc.[URL_USERNAME]
			  ,uc.[URL_PASSWORD]
			  ,uc.[URL_STATUS]
			  ,uc.[URL_ISACTIVE]
			  ,uc.[URL_CREATEDBY]
			  ,uc.[URL_CREATEDDATE]
			  ,uc.[URL_UPDATEDBY]
			  ,uc.[URL_UPDATEDDATE]
			  ,uc.[URL_COMMENTS]
		  FROM [dbo].[CSM_URLCONFIGURATION] uc
		  left join dbo.CSM_ENVIRONEMENT env
		  On env.ENV_ID = uc.ENV_ID
		  where [URL_ISACTIVE] = 1
		  order by env.ENV_SORTORDER
	  End	  
End  
GO


