--Insert record into table [CSM_USER]
insert into [CSM_USER] 
(
	[USER_LOGIN_ID]
	,[USER_FIRST_NAME]
	,[USER_LAST_NAME]
	,[USER_EMAIL_ADDRESS]
	,[USER_ROLE]
	,[USER_PASSWORD]
	,[USER_IS_ACTIVE]
	,[USER_IS_DELETED]
	,[USER_COMMENTS]
)
values
(
	'admin@cosmo.com'
	,'Cosmo'
	,'Administrator'
	,'admin@cosmo.com'
	,1
	,'YWRtaW5jb3Ntbw=='
	,'true'
	,'false'
	,'Admin user id and not to be deleted at any cost'
	
)
GO


---Insert record into table [CSM_ROLES]
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Administrator ', 'Administrator ', 'A', 'true')
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Dashboard', 'Dashboard', 'U', 'true')
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Setup Environment', 'Setup Environment', 'U', 'true')
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Restart Services', 'Restart Services', 'U', 'true')
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Reports Only', 'Reports Only', 'U', 'true')
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Mobile', 'Mobile', 'U', 'true')

GO

--Insert record into table [CSM_USER_ROLE]
  insert into [CSM_USER_ROLE]
  (
      [USER_ID]
      ,[ROLE_ID]
      ,[USER_ROLE_ISACTIVE]
      ,[USER_COMMENTS]
  )
  values
  (
	1
	,1
	,'true'
	,'Admin user id and not to be deleted at any cost'
  )
GO


--Insert record into table [CSM_MENUS]
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Home', 'Home', 'Home.aspx', 'FALSE', 'TRUE', '1', '1')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Dashboard', 'Dashboard', 'Dashboard.aspx', 'FALSE', 'TRUE', '2', '1')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Setup', 'Environment', 'Environment.aspx', 'FALSE', 'TRUE', '3', '1')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Setup', 'URL Monitor', 'UrlConfiguration.aspx', 'FALSE', 'TRUE', '3', '2')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Setup', 'Seperator', '-', 'FALSE', 'TRUE', '3', '3')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Setup', 'Notification', 'EnvironmentEmail.aspx', 'FALSE', 'TRUE', '3', '4')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Action', 'Incident Tracker', 'Incident.aspx', 'FALSE', 'TRUE', '4', '1')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Action', 'Seperator', '-', 'FALSE', 'TRUE', '4', '2')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Action', 'Service Restart', 'WinServiceOperation.aspx', 'FALSE', 'TRUE', '4', '3')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Build Report - Current', 'ReportCurrentBuild.aspx', 'FALSE', 'TRUE', '5', '1')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Build Report - History', 'ReportBuildHistory.aspx', 'FALSE', 'TRUE', '5', '2')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Seperator', '-', 'FALSE', 'TRUE', '5', '3')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Services Status Report', 'ReportStatusHistory.aspx', 'FALSE', 'TRUE', '5', '4')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Seperator', '-', 'FALSE', 'TRUE', '5', '5')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Incident Report', 'ReportIncidentHistory.aspx', 'FALSE', 'TRUE', '5', '6')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Services Stop/Start Report', 'ReportServiceRestart.aspx', 'FALSE', 'TRUE', '5', '7')
insert into [CSM_MENUS] ([MENU_MAIN],[MENU_SUB], [MENU_PATH],[MENU_ISPOPUP],[MENU_ISACTIVE], [MENU_MAIN_ORDER], [MENU_SUB_ORDER]) values ('Reports', 'Daily Report Subscription', 'modalSubscriptionSetting', 'TRUE', 'TRUE', '5', '8')

GO

--Insert record into table [CSM_ROLEMENU]

insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '1', 'FALSE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '2', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '3', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '4', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '5', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '6', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '7', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '8', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '9', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '10', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '11', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '12', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '13', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '14', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '15', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '16', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '17', 'TRUE')

insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('2', '2', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '3', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '4', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '5', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '6', 'TRUE')

insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('4', '7', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('4', '8', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('4', '15', 'TRUE')

insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '9', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '10', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '11', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '12', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '13', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '14', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '15', 'TRUE')
insert into [CSM_ROLEMENU] (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '16', 'TRUE')
GO

--Insert  record into CSM_POERSONALIZE
insert into CSM_POERSONALIZE ([User_ID], [PERS_DB_REFRESHTIME], [PERS_ISACTIVE], [PERS_CREATEDDATE], [PERS_CREATEDBY]) values(1,2,1,GETDATE(),1)
GO
