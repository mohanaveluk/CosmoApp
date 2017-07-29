IF OBJECT_ID('CSM_ACKNOWLEDGE') IS NOT NULL
	DROP TABLE DBO.CSM_ACKNOWLEDGE
GO

IF OBJECT_ID('CSM_EMAIL_CONFIGURATION') IS NOT NULL
	DROP TABLE DBO.CSM_EMAIL_CONFIGURATION
GO

IF OBJECT_ID('CSM_EMAIL_CONTENT') IS NOT NULL
	DROP TABLE DBO.CSM_EMAIL_CONTENT
GO

IF OBJECT_ID('CSM_EMAIL_TRACKING') IS NOT NULL
	DROP TABLE DBO.CSM_EMAIL_TRACKING
GO

IF OBJECT_ID('CSM_SCHEDULE') IS NOT NULL
	DROP TABLE DBO.CSM_SCHEDULE
GO

IF OBJECT_ID('CSM_INCIDENT') IS NOT NULL
	DROP TABLE DBO.CSM_INCIDENT
GO

IF OBJECT_ID('CSM_MONITOR') IS NOT NULL
	DROP TABLE DBO.CSM_MONITOR
GO
IF OBJECT_ID('CSM_CONFIGURATION') IS NOT NULL
	DROP TABLE DBO.CSM_CONFIGURATION
GO

IF OBJECT_ID('CSM_ENVIRONEMENT') IS NOT NULL
	DROP TABLE DBO.CSM_ENVIRONEMENT
GO

IF OBJECT_ID('CSM_LDAP_CONFIGURATION') IS NOT NULL
	DROP TABLE DBO.CSM_LDAP_CONFIGURATION
GO
IF OBJECT_ID('CSM_GROUP_SCHEDULE_DETAIL') IS NOT NULL
	DROP TABLE DBO.CSM_GROUP_SCHEDULE_DETAIL
GO

IF OBJECT_ID('CSM_GROUP_SCHEDULE') IS NOT NULL
	DROP TABLE DBO.CSM_GROUP_SCHEDULE
GO

IF OBJECT_ID('CSM_GROUP_DETAIL') IS NOT NULL
	DROP TABLE DBO.CSM_GROUP_DETAIL
GO

IF OBJECT_ID('CSM_GROUP') IS NOT NULL
	DROP TABLE DBO.CSM_GROUP
GO

IF OBJECT_ID('CSM_USER_ROLE') IS NOT NULL
	DROP TABLE DBO.CSM_USER_ROLE
GO

IF OBJECT_ID('CSM_ROLEMENU') IS NOT NULL
	DROP TABLE DBO.CSM_ROLEMENU
GO

IF OBJECT_ID('CSM_MENUS') IS NOT NULL
	DROP TABLE DBO.CSM_MENUS
GO

IF OBJECT_ID('CSM_USER') IS NOT NULL
	DROP TABLE DBO.CSM_USER
GO

IF OBJECT_ID('CSM_MON_DAILY_STATUS') IS NOT NULL
	DROP TABLE DBO.CSM_MON_DAILY_STATUS
GO

IF OBJECT_ID('CSM_POERSONALIZE') IS NOT NULL
	DROP TABLE DBO.CSM_POERSONALIZE
GO

IF OBJECT_ID('CSM_REPORT_SUBSCRIPTION_DETAILS') IS NOT NULL
	DROP TABLE DBO.CSM_REPORT_SUBSCRIPTION_DETAILS
GO

IF OBJECT_ID('CSM_EMAIL_USERLIST') IS NOT NULL
	DROP TABLE DBO.CSM_EMAIL_USERLIST
GO

IF OBJECT_ID('CSM_REPORT_SUBSCRIPTION') IS NOT NULL
	DROP TABLE DBO.CSM_REPORT_SUBSCRIPTION
GO

IF OBJECT_ID('CSM_URLCONFIGURATION') IS NOT NULL
	DROP TABLE DBO.CSM_URLCONFIGURATION
GO

IF OBJECT_ID('CSM_PORTALMONITOR') IS NOT NULL
	DROP TABLE DBO.CSM_PORTALMONITOR
GO

IF OBJECT_ID('CSM_SERVERPERFORMANCE_SCHEDULE') IS NOT NULL
	DROP TABLE DBO.CSM_SERVERPERFORMANCE_SCHEDULE
GO

IF OBJECT_ID('CSM_SERVERPERFORMANCE_DRIVE') IS NOT NULL
	DROP TABLE DBO.CSM_SERVERPERFORMANCE_DRIVE
GO

IF OBJECT_ID('CSM_SERVERPERFORMANCE') IS NOT NULL
	DROP TABLE DBO.CSM_SERVERPERFORMANCE
GO

IF OBJECT_ID('CSM_LOG') IS NOT NULL
	DROP TABLE DBO.CSM_LOG
GO

IF OBJECT_ID('CSM_ROLES') IS NOT NULL
	DROP TABLE DBO.CSM_ROLES
GO

IF OBJECT_ID('CSM_SERVICEBUILD') IS NOT NULL
	DROP TABLE DBO.CSM_SERVICEBUILD
GO

IF OBJECT_ID('CSM_USER_ACCESS') IS NOT NULL
	DROP TABLE DBO.CSM_USER_ACCESS
GO

IF OBJECT_ID('CSM_WINDOWS_SERVICES') IS NOT NULL
	DROP TABLE DBO.CSM_WINDOWS_SERVICES
GO

IF OBJECT_ID('CSM_LDAP_CONFIGURATIONCSM_LDAP_CONFIGURATION') IS NOT NULL
	DROP TABLE DBO.CSM_LDAP_CONFIGURATIONCSM_LDAP_CONFIGURATION
GO




IF (OBJECT_ID('CWT_DeleteRecord') IS NOT NULL)
	DROP PROCEDURE CWT_DeleteRecord
GO

IF (OBJECT_ID('CWT_GetAllConfigEmail') IS NOT NULL)
	DROP PROCEDURE CWT_GetAllConfigEmail
GO

IF (OBJECT_ID('CWT_GetAllIncident') IS NOT NULL)
	DROP PROCEDURE CWT_GetAllIncident
GO

IF (OBJECT_ID('CWT_GetAllSchedule_NextJobStartBefore') IS NOT NULL)
	DROP PROCEDURE CWT_GetAllSchedule_NextJobStartBefore
GO

IF (OBJECT_ID('CWT_GetAllScheduledServices') IS NOT NULL)
	DROP PROCEDURE CWT_GetAllScheduledServices
GO

IF (OBJECT_ID('CWT_GetConfigurationDetails') IS NOT NULL)
	DROP PROCEDURE CWT_GetConfigurationDetails
GO

IF (OBJECT_ID('CWT_GetConfigurationWithServiceName') IS NOT NULL)
	DROP PROCEDURE CWT_GetConfigurationWithServiceName
GO

IF (OBJECT_ID('CWT_GetCurrentBuildReport') IS NOT NULL)
	DROP PROCEDURE CWT_GetCurrentBuildReport
GO

IF (OBJECT_ID('CWT_GetDispatcherConfigID') IS NOT NULL)
	DROP PROCEDURE CWT_GetDispatcherConfigID
GO

IF (OBJECT_ID('CWT_GetEnvConfigID') IS NOT NULL)
	DROP PROCEDURE CWT_GetEnvConfigID
GO

IF (OBJECT_ID('CWT_GetEnvID') IS NOT NULL)
	DROP PROCEDURE CWT_GetEnvID
GO

IF (OBJECT_ID('CWT_GetEnvironmentConfigList') IS NOT NULL)
	DROP PROCEDURE CWT_GetEnvironmentConfigList
GO

IF (OBJECT_ID('CWT_GetEnvironmentList') IS NOT NULL)
	DROP PROCEDURE CWT_GetEnvironmentList
GO

IF (OBJECT_ID('CWT_GetGroup') IS NOT NULL)
	DROP PROCEDURE CWT_GetGroup
GO

IF (OBJECT_ID('CWT_GetGroupDetail') IS NOT NULL)
	DROP PROCEDURE CWT_GetGroupDetail
GO

IF (OBJECT_ID('CWT_GetGroupID') IS NOT NULL)
	DROP PROCEDURE CWT_GetGroupID
GO

IF (OBJECT_ID('CWT_GetGroupOpenScheduledServceDetails') IS NOT NULL)
	DROP PROCEDURE CWT_GetGroupOpenScheduledServceDetails
GO

IF (OBJECT_ID('CWT_GetGroupSchedule') IS NOT NULL)
	DROP PROCEDURE CWT_GetGroupSchedule
GO

IF (OBJECT_ID('CWT_GetGroupScheduleServceDetails') IS NOT NULL)
	DROP PROCEDURE CWT_GetGroupScheduleServceDetails
GO

IF (OBJECT_ID('CWT_GetIncidentTracking') IS NOT NULL)
	DROP PROCEDURE CWT_GetIncidentTracking
GO

IF (OBJECT_ID('CWT_GetLogin') IS NOT NULL)
	DROP PROCEDURE CWT_GetLogin
GO

IF (OBJECT_ID('CWT_GetMenuItems') IS NOT NULL)
	DROP PROCEDURE CWT_GetMenuItems
GO

IF (OBJECT_ID('CWT_GetMonitorStatus') IS NOT NULL)
	DROP PROCEDURE CWT_GetMonitorStatus
GO

IF (OBJECT_ID('CWT_GetMonitorStatusWithServiceName') IS NOT NULL)
	DROP PROCEDURE CWT_GetMonitorStatusWithServiceName
GO

IF (OBJECT_ID('CWT_GetMonitorStatusWithServiceName_ConID') IS NOT NULL)
	DROP PROCEDURE CWT_GetMonitorStatusWithServiceName_ConID
GO

IF (OBJECT_ID('CWT_GetSchedulerDetails') IS NOT NULL)
	DROP PROCEDURE CWT_GetSchedulerDetails
GO

IF (OBJECT_ID('CWT_GetSchedulerID') IS NOT NULL)
	DROP PROCEDURE CWT_GetSchedulerID
GO

IF (OBJECT_ID('CWT_GetSendNotification') IS NOT NULL)
	DROP PROCEDURE CWT_GetSendNotification
GO

IF (OBJECT_ID('CWT_GetServiceAvailability') IS NOT NULL)
	DROP PROCEDURE CWT_GetServiceAvailability
GO

IF (OBJECT_ID('CWT_GetServiceAvailabilityForDowntime') IS NOT NULL)
	DROP PROCEDURE CWT_GetServiceAvailabilityForDowntime
GO

IF (OBJECT_ID('CWT_GetServiceAvailabilityForUptime') IS NOT NULL)
	DROP PROCEDURE CWT_GetServiceAvailabilityForUptime
GO

IF (OBJECT_ID('CWT_GetUpTime') IS NOT NULL)
	DROP PROCEDURE CWT_GetUpTime
GO

IF (OBJECT_ID('CWT_getUserEmailList') IS NOT NULL)
	DROP PROCEDURE CWT_getUserEmailList
GO

IF (OBJECT_ID('CWT_GetUserRole') IS NOT NULL)
	DROP PROCEDURE CWT_GetUserRole
GO

IF (OBJECT_ID('CWT_GetUsers') IS NOT NULL)
	DROP PROCEDURE CWT_GetUsers
GO

IF (OBJECT_ID('CWT_GetWindowsServiceConfigurationOnDemand') IS NOT NULL)
	DROP PROCEDURE CWT_GetWindowsServiceConfigurationOnDemand
GO

IF (OBJECT_ID('CWT_GetWindowsServiceDetails') IS NOT NULL)
	DROP PROCEDURE CWT_GetWindowsServiceDetails
GO

IF (OBJECT_ID('CWT_InsertBuildDetails') IS NOT NULL)
	DROP PROCEDURE CWT_InsertBuildDetails
GO

IF (OBJECT_ID('CWT_InsertCSMLog') IS NOT NULL)
	DROP PROCEDURE CWT_InsertCSMLog
GO

IF (OBJECT_ID('CWT_InsertMailLog') IS NOT NULL)
	DROP PROCEDURE CWT_InsertMailLog
GO

IF (OBJECT_ID('CWT_InsGroup') IS NOT NULL)
	DROP PROCEDURE CWT_InsGroup
GO

IF (OBJECT_ID('CWT_InsGroupDetail') IS NOT NULL)
	DROP PROCEDURE CWT_InsGroupDetail
GO

IF (OBJECT_ID('CWT_InsGroupSchedule') IS NOT NULL)
	DROP PROCEDURE CWT_InsGroupSchedule
GO

IF (OBJECT_ID('CWT_InsIncidentTracking') IS NOT NULL)
	DROP PROCEDURE CWT_InsIncidentTracking
GO

IF (OBJECT_ID('CWT_InsUpdEmailUsers') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdEmailUsers
GO

IF (OBJECT_ID('CWT_InsUpdEnvironment') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdEnvironment
GO

IF (OBJECT_ID('CWT_InsUpdEnvironmentConfig') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdEnvironmentConfig
GO

IF (OBJECT_ID('CWT_InsUpdScheduler') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdScheduler
GO

IF (OBJECT_ID('CWT_InsUpdUser') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdUser
GO

IF (OBJECT_ID('CWT_InsUpdUserRoles') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdUserRoles
GO

IF (OBJECT_ID('CWT_InsUpdWindowsService') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdWindowsService
GO

IF (OBJECT_ID('CWT_ISServiceExists') IS NOT NULL)
	DROP PROCEDURE CWT_ISServiceExists
GO

IF (OBJECT_ID('CWT_SetMonitorStatus') IS NOT NULL)
	DROP PROCEDURE CWT_SetMonitorStatus
GO

IF (OBJECT_ID('CWT_SetServiceAcknowledge') IS NOT NULL)
	DROP PROCEDURE CWT_SetServiceAcknowledge
GO

IF (OBJECT_ID('CWT_UpdateGroupScheduleStatus') IS NOT NULL)
	DROP PROCEDURE CWT_UpdateGroupScheduleStatus
GO

IF (OBJECT_ID('CWT_UpdateSchedulerLastRunDateTime') IS NOT NULL)
	DROP PROCEDURE CWT_UpdateSchedulerLastRunDateTime
GO

IF (OBJECT_ID('CWT_UpdateServiceStatus') IS NOT NULL)
	DROP PROCEDURE CWT_UpdateServiceStatus
GO

IF (OBJECT_ID('CWT_UpdateUserPassword') IS NOT NULL)
	DROP PROCEDURE CWT_UpdateUserPassword
GO

IF (OBJECT_ID('CWT_ReportRestartService') IS NOT NULL)
	DROP PROCEDURE CWT_ReportRestartService
GO

IF (OBJECT_ID('CWT_GetEnvIDByConfigID') IS NOT NULL)
	DROP PROCEDURE CWT_GetEnvIDByConfigID
GO

IF (OBJECT_ID('CWT_ReportBuildHistory') IS NOT NULL)
	DROP PROCEDURE CWT_ReportBuildHistory
GO

IF (OBJECT_ID('CWT_ReportBuildHistory') IS NOT NULL)
	DROP PROCEDURE CWT_ReportBuildHistory
GO

IF (OBJECT_ID('CWT_InsMonitorDailyStatus') IS NOT NULL)
	DROP PROCEDURE CWT_InsMonitorDailyStatus
GO

IF (OBJECT_ID('CWT_GetCurrentBuildReport') IS NOT NULL)
	DROP PROCEDURE CWT_GetCurrentBuildReport
GO

IF (OBJECT_ID('CWT_ReportServiceStatus') IS NOT NULL)
	DROP PROCEDURE CWT_ReportServiceStatus
GO

IF (OBJECT_ID('CWT_InsMailServer') IS NOT NULL)
	DROP PROCEDURE CWT_InsMailServer
GO

IF (OBJECT_ID('CWT_GetMailServerDetail') IS NOT NULL)
	DROP PROCEDURE CWT_GetMailServerDetail
GO

IF (OBJECT_ID('CWT_InsUpdPersonalize') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdPersonalize
GO

IF (OBJECT_ID('CWT_GetPersonaliseSetting') IS NOT NULL)
	DROP PROCEDURE CWT_GetPersonaliseSetting
GO

IF (OBJECT_ID('CWT_GetServiceLastStatus') IS NOT NULL)
	DROP PROCEDURE CWT_GetServiceLastStatus
GO

IF (OBJECT_ID('CWT_GetSubscriptionUserEmail') IS NOT NULL)
	DROP PROCEDURE CWT_GetSubscriptionUserEmail
GO

IF (OBJECT_ID('CWT_GetSubscriptionDetail') IS NOT NULL)
	DROP PROCEDURE CWT_GetSubscriptionDetail
GO

IF (OBJECT_ID('CWT_GetSubscription') IS NOT NULL)
	DROP PROCEDURE CWT_GetSubscription
GO

IF (OBJECT_ID('CWT_SetSubscriptionNextJobRunTime') IS NOT NULL)
	DROP PROCEDURE CWT_SetSubscriptionNextJobRunTime
GO

IF (OBJECT_ID('CWT_InsUpdSubcription') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdSubcription
GO

IF (OBJECT_ID('CWT_GetSubscriptionMonitorStatus') IS NOT NULL)
	DROP PROCEDURE CWT_GetSubscriptionMonitorStatus
GO

IF (OBJECT_ID('CWT_InsUpdUrlConfiguration') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdUrlConfiguration
GO

IF (OBJECT_ID('CWT_GetUrlConfiguration') IS NOT NULL)
	DROP PROCEDURE CWT_GetUrlConfiguration
GO

IF (OBJECT_ID('CWT_ISUrlConfigExists') IS NOT NULL)
	DROP PROCEDURE CWT_ISUrlConfigExists
GO

IF (OBJECT_ID('CWT_GetPortelToMonitor') IS NOT NULL)
	DROP PROCEDURE CWT_GetPortelToMonitor
GO

IF (OBJECT_ID('CWT_SetPortalNextJobRunTime') IS NOT NULL)
	DROP PROCEDURE CWT_SetPortalNextJobRunTime
GO

IF (OBJECT_ID('CWT_InsertPortalStatus') IS NOT NULL)
	DROP PROCEDURE CWT_InsertPortalStatus
GO

IF (OBJECT_ID('CWT_GetUrlPerformanceLast24Hours') IS NOT NULL)
	DROP PROCEDURE CWT_GetUrlPerformanceLast24Hours
GO

IF (OBJECT_ID('CWT_GetServerPerformanceSchedule') IS NOT NULL)
	DROP PROCEDURE CWT_GetServerPerformanceSchedule
GO

IF (OBJECT_ID('CWT_InsUpdServerPerformance') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdServerPerformance
GO

IF (OBJECT_ID('CWT_InsUpdServerPerformanceDrive') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdServerPerformanceDrive
GO

IF (OBJECT_ID('CWT_InsUpdEnvironmentConfig') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdEnvironmentConfig
GO

IF (OBJECT_ID('CWT_InsUpdServerPerformanceSchedule') IS NOT NULL)
	DROP PROCEDURE CWT_InsUpdServerPerformanceSchedule
GO

IF (OBJECT_ID('CWT_GetUrlPerformance') IS NOT NULL)
	DROP PROCEDURE CWT_GetUrlPerformance
GO

IF (OBJECT_ID('CWT_GetAverageUsedSpace') IS NOT NULL)
	DROP PROCEDURE CWT_GetAverageUsedSpace
GO

IF (OBJECT_ID('CWT_GetCpuMemorySpace') IS NOT NULL)
	DROP PROCEDURE CWT_GetCpuMemorySpace
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[funcListToTableInt]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[funcListToTableInt]
GO