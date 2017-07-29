/* SCRIPT: CREATE_DB.sql */
/* BUILD A DATABASE */

-- This is the main caller for each script
SET NOCOUNT ON
GO


PRINT 'DELETING PROCEDURES '
:r DeleteProceduresScript.sql
PRINT 'PROCEDURES DELETED'

/* SCRIPT: CREATE_TABLES.sql */
PRINT 'CREATING TABLES '
GO


:r CSM_ENVIRONEMENT.sql
:r CSM_CONFIGURATION.sql
:r CSM_SCHEDULE.sql 
:r CSM_MONITOR.sql
:r CSM_INCIDENT.sql
:r CSM_EMAIL_CONFIGURATION.sql
:r CSM_EMAIL_CONTENT.sql
:r CSM_EMAIL_USERLIST.sql
:r CSM_EMAIL_TRACKING.sql
:r CSM_LDAP_CONFIGURATION.sql
:r CSM_ACKNOWLEDGE.sql
:r CSM_LOG.sql
:r CSM_SERVICEBUILD,sql.sql
:r CSM_GROUP.sql
:r CSM_GROUP_DETAIL.sql
:r CSM_GROUP_SCHEDULE.sql
:r CSM_GROUP_SCHEDULE_DETAIL.sql
:r CSM_WINDOWS_SERVICES.sql
:r CSM_USER.sql
:r CSM_ROLES.sql
:r CSM_USER_ROLE.sql
:r CSM_MENUS.sql
:r CSM_ROLEMENU.sql
:r CSM_MON_DAILY_STATUS.sql
:r CSM_POERSONALIZE.sql
:r CSM_USER_ACCESS.sql
:r CSM_REPORT_SUBSCRIPTION.sql
:r CSM_REPORT_SUBSCRIPTION_DETAILS.sql
:r INSERT_USER_DATA.sql
:r CSM_URLCONFIGURATION.sql
:r CSM_PORTALMONITOR.sql
:r CSM_SERVERPERFORMANCE.sql
:r CSM_SERVERPERFORMANCE_DRIVE.sql
:r CSM_SERVERPERFORMANCE_SCHEDULE.sql






PRINT 'CREATING PROCEDURES'
:r CWT_UpdateSchedulerLastRunDateTime.sql
:r CWT_InsertBuildDetails.sql
:r CWT_InsertCSMLog.sql
:r CWT_InsertMailLog.sql
:r CWT_InsMonitorDailyStatus.sql
:r CWT_SetMonitorStatus.sql
:r CWT_SetServiceAcknowledge.sql
:r CWT_InsUpdScheduler.sql
:r CWT_InsUpdWindowsService.sql

:r CWT_InsUpdServerPerformanceSchedule.sql

:r CWT_InsUpdEnvironmentConfig.sql
:r CWT_InsUpdEnvironment.sql
:r CWT_InsUpdEmailUsers.sql
:r CWT_InsIncidentTracking.sql
:r CWT_getUserEmailList.sql
:r CWT_GetUpTime.sql
:r CWT_GetServiceAvailabilityForUptime.sql
:r CWT_GetServiceAvailabilityForDowntime.sql
:r CWT_GetServiceAvailability.sql
:r CWT_GetSendNotification.sql
:r CWT_GetSchedulerID.sql
:r CWT_GetSchedulerDetails.sql
:r CWT_GetMonitorStatus.sql
:r CWT_GetIncidentTracking.sql
:r CWT_GetEnvironmentList.sql
:r CWT_GetEnvironmentConfigList.sql
:r CWT_GetEnvID.sql
:r CWT_GetEnvConfigID.sql
:r CWT_GetDispatcherConfigID.sql
:r CWT_GetCurrentBuildReport.sql
:r CWT_GetConfigurationDetails.sql
:r CWT_GetAllScheduledServices.sql
:r CWT_GetAllSchedule_NextJobStartBefore.sql
:r CWT_GetAllIncident.sql
:r CWT_GetAllConfigEmail.sql
:r CWT_DeleteRecord.sql
:r CWT_GetConfigurationWithServiceName.sql
:r CWT_GetGroup.sql
:r CWT_GetGroupSchedule.sql
:r CWT_GetWindowsServiceDetails.sql
:r CWT_InsGroup.sql
:r CWT_GetGroupDetail.sql
:r CWT_GetGroupID.sql
:r CWT_GetGroupOpenScheduledServceDetails.sql
:r CWT_GetGroupScheduleServceDetails.sql
:r CWT_GetMonitorStatusWithServiceName.sql
:r CWT_GetMonitorStatusWithServiceName_ConID.sql
:r CWT_InsGroupDetail.sql
:r CWT_InsGroupSchedule.sql
:r CWT_UpdateGroupScheduleStatus.sql
:r CWT_UpdateServiceStatus.sql
:r CWT_GetWindowsServiceConfigurationOnDemand.sql
:r CWT_ISServiceExists.sql
:r CWT_GetMenuItems.sql
:r CWT_InsUpdUserRoles.sql
:r CWT_GetLogin.sql
:r CWT_GetUsers.sql
:r CWT_InsUpdUser.sql
:r CWT_GetUserRole.sql
:r CWT_UpdateUserPassword.sql
:r CWT_ReportRestartService.sql
:r CWT_InsMailServer.sql
:r CWT_GetMailServerDetail.sql
:r CWT_InsUpdPersonalize.sql
:r CWT_GetPersonaliseSetting.sql

:r CWT_GetEnvIDByConfigID.sql
:r CWT_ReportBuildHistory.sql
:r CWT_ReportServiceStatus.sql
:r CWT_InsUserAccess.sql
:r CWT_GetUserAccess.sql
:r CWT_GetServiceLastStatus.sql
:r CWT_GetSubscription.sql
:r CWT_GetSubscriptionDetail.sql
:r CWT_GetSubscriptionMonitorStatus.sql
:r CWT_GetSubscriptionUserEmail.sql
:r CWT_InsUpdSubscription.sql
:r CWT_SetSubscriptionNextJobRunTime.sql

:r CWT_GetCpuMemorySpace.sql
:r CWT_GetAverageUsedSpace.sql
:r CWT_GetUrlPerformance.sql

:r CWT_InsUpdServerPerformanceDrive.sql
:r CWT_InsUpdServerPerformance.sql
:r CWT_GetServerPerformanceSchedule.sql
:r CWT_GetUrlPerformanceLast24Hours.sql
:r CWT_InsertPortalStatus.sql
:r CWT_SetPortalNextJobRunTime.sql
:r CWT_GetPortelToMonitor.sql
:r CWT_ISUrlConfigExists.sql
:r CWT_GetUrlConfiguration.sql
:r CWT_InsUpdUrlConfiguration.sql



:r funcListToTableInt.sql
PRINT 'TEABLES & PROCEDURES CREATED SUCCESSFULLY'
GO
