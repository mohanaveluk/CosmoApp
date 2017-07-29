SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[CWT_DeleteRecord](
	@sID int,
	@sType varchar(20)
)
AS
Begin
	if lower(@sType) = 'env'
	Begin
		Update CSM_ENVIRONEMENT set ENV_ISACTIVE = 'False' 
			where ENV_ID = @sID
		Update [CSM_CONFIGURATION] set [CONFIG_IS_ACTIVE] = 'False' 
			where ([ENV_ID] = @sID)
	End
	else if lower(@sType) = 'cfg'
	Begin
		Update [CSM_CONFIGURATION] set [CONFIG_IS_ACTIVE] = 'False' 
			where ([CONFIG_ID] = @sID or [CONFIG_REF_ID] = @sID)
	End
	else if lower(@sType) = 'other'
	Begin
		Update CSM_ENVIRONEMENT set ENV_ISACTIVE = 'False' 
			where ENV_ID = @sID
	End
	else if lower(@sType) = 'email'
	Begin
		Update [CSM_EMAIL_USERLIST] set [USRLST_IS_ACTIVE] = 'False' 
			where [USRLST_ID] = @sID
	End
	else if lower(@sType) = 'ss_notify_stop'
	Begin
		Update [CSM_CONFIGURATION] set [CONFIG_ISNOTIFY] = 'False' 
			where [CONFIG_ID] = @sID or [CONFIG_REF_ID] = @sID
	End
	else if lower(@sType) = 'ss_notify_start'
	Begin
		Update [CSM_CONFIGURATION] set [CONFIG_ISNOTIFY] = 'True'
			where [CONFIG_ID] = @sID or [CONFIG_REF_ID] = @sID
	End
	else if lower(@sType) = 'user'
	Begin
		Update [CSM_USER] set [USER_IS_DELETED] = 'true'
			where [CSM_USER].USER_ID = @sID
		update [CSM_USER_ROLE] set [CSM_USER_ROLE].USER_ROLE_ISACTIVE = 'False'
			where [CSM_USER_ROLE].USER_ID = @sID
	End
	else if lower(@sType) = 'grpsch'
	Begin
		update CSM_GROUP_SCHEDULE set GROUP_SCH_STATUS = 'C', GROUP_SCH_UPDATED_DATETIME = GETDATE()
		where GROUP_SCH_ID = @sID

		update CSM_GROUP_SCHEDULE_DETAIL  set GROUP_SCH_STATUS = 'C' 
		where GROUP_SCH_ID = @sID
	End
	else if lower(@sType) = 'urlconfig'
	Begin
		Update [CSM_URLCONFIGURATION] set [URL_ISACTIVE] = 'False'
			where [URL_ID] = @sID
	End
End


GO

