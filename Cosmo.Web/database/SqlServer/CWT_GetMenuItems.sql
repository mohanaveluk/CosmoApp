/****** Object:  StoredProcedure [dbo].[CWT_GetMenuItems]    Script Date: 10/04/2015 14:05:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetMenuItems](@ROLEID varchar(200)) As
Begin
	if @ROLEID = '1'
	Begin
		Select crm.RM_ID
			,crm.ROLE_ID
			,crm.MENU_ID
			,crm.RM_ISACTIVE
			,role.ROLE_NAME
			,role.ROLE_TYPE
			,[MENU_MAIN]
			,[MENU_SUB]
			,[MENU_PATH]
			,[MENU_ISPOPUP]
			,[MENU_ISACTIVE]		
			,[MENU_MAIN_ORDER]
			,[MENU_SUB_ORDER]
		From [CSM_ROLEMENU] crm 
		inner join CSM_ROLES role on role.ROLE_ID = crm.ROLE_ID
		inner join CSM_MENUS menu on menu.MENU_ID = crm.MENU_ID
		Where RM_ISACTIVE = 'True'
			and [MENU_ISACTIVE] = 'true'
		order by [MENU_MAIN_ORDER], [MENU_SUB_ORDER]
	End
	else
	Begin
		Select crm.RM_ID
			,crm.ROLE_ID
			,crm.MENU_ID
			,crm.RM_ISACTIVE
			,role.ROLE_NAME
			,role.ROLE_TYPE
			,[MENU_MAIN]
			,[MENU_SUB]
			,[MENU_PATH]
			,[MENU_ISPOPUP]
			,[MENU_ISACTIVE]		
			,[MENU_MAIN_ORDER]
			,[MENU_SUB_ORDER]
		From [CSM_ROLEMENU] crm 
		inner join CSM_ROLES role on role.ROLE_ID = crm.ROLE_ID
		inner join CSM_MENUS menu on menu.MENU_ID = crm.MENU_ID
		Where RM_ISACTIVE = 'True'
			and [MENU_ISACTIVE] = 'true'
			--and crm.ROLE_ID in (Replace(@ROLEID,'''',''))
			--and CONVERT(varchar(10), crm.ROLE_ID) in (@ROLEID)
			and crm.ROLE_ID in (
				select value from funcListToTableInt(@ROLEID,',')
			)
		order by [MENU_MAIN_ORDER], [MENU_SUB_ORDER]
	End
End	

GO


