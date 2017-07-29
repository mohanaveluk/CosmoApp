--Insert record into table CSM_USER
insert into CSM_USER 
(USER_LOGIN_ID
	,USER_FIRST_NAME
	,USER_LAST_NAME
	,USER_EMAIL_ADDRESS
	,USER_ROLE
	,USER_PASSWORD
	,USER_IS_ACTIVE
	,USER_IS_DELETED
	,USER_COMMENTS)
values
('admin@cosmo.com'
	,'Cosmo'
	,'Administrator'
	,'admin@cosmo.com'
	,1
	,'YWRtaW5jb3Ntbw=='
	,1
	,0
	,'Admin user id and not to be deleted at any cost');



---Insert record into table CSM_ROLES
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Administrator ', 'Administrator ', 'A', 1);
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Dashboard', 'Dashboard', 'U', 1);
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Setup Environment', 'Setup Environment', 'U', 1);
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Restart Services', 'Restart Services', 'U', 1);
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Reports Only', 'Reports Only', 'U', 1);
Insert into CSM_ROLES  (ROLE_NAME, ROLE_DESCRIPTION, ROLE_TYPE, ROLE_ISACTIVE) values('Mobile', 'Mobile', 'U', 1);





--Insert record into table CSM_USER_ROLE
  insert into CSM_USER_ROLE
  ( USER_ID
      ,ROLE_ID
      ,USER_ROLE_ISACTIVE
      ,USER_COMMENTS  )
  values
  (	1
	,1
	,1
	,'Admin user id and not to be deleted at any cost'  );




--Insert record into table CSM_MENUS
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Home', 'Home', 'Home.aspx', 0, 1, '1', '1');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Dashboard', 'Dashboard', 'Dashboard.aspx', 0, 1, '2', '1');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Setup', 'Environment', 'Environment.aspx', 0, 1, '3', '1');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Setup', 'URL Monitor', 'UrlConfiguration.aspx', 0, 1, '3', '2');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Setup', 'Seperator', '-', 0, 1, '3', '3');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Setup', 'Notification', 'EnvironmentEmail.aspx', 0, 1, '3', '4');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Action', 'Incident Tracker', 'Incident.aspx', 0, 1, '4', '1');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Action', 'Seperator', '-', 0, 1, '4', '2');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Action', 'Service Restart', 'WinServiceOperation.aspx', 0, 1, '4', '3');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Build Report - Current', 'ReportCurrentBuild.aspx', 0, 1, '5', '1');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Build Report - History', 'ReportBuildHistory.aspx', 0, 1, '5', '2');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Seperator', '-', 0, 1, '5', '3');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Services Status Report', 'ReportStatusHistory.aspx', 0, 1, '5', '4');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Seperator', '-', 0, 1, '5', '5');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Incident Report', 'ReportIncidentHistory.aspx', 0, 1, '5', '6');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Services Stop/Start Report', 'ReportServiceRestart.aspx', 0, 1, '5', '7');
insert into CSM_MENUS (MENU_MAIN,MENU_SUB, MENU_PATH,MENU_ISPOPUP,MENU_ISACTIVE, MENU_MAIN_ORDER, MENU_SUB_ORDER) values ('Reports', 'Daily Report Subscription', 'modalSubscriptionSetting', 1, 1, '5', '8');




--Insert record into table CSM_ROLEMENU

insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '1', 0);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '2', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '3', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '4', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '5', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '6', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '7', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '8', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '9', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '10', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '11', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '12', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '13', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '14', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '15', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '16', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('1', '17', 1);

insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('2', '2', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '3', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '4', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '5', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('3', '6', 1);

insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('4', '7', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('4', '8', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('4', '15', 1);

insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '9', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '10', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '11', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '12', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '13', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '14', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '15', 1);
insert into CSM_ROLEMENU (ROLE_ID, MENU_ID, RM_ISACTIVE) values ('5', '16', 1);



--Insert  record into CSM_POERSONALIZE
insert into CSM_POERSONALIZE (User_ID, PERS_DB_REFRESHTIME, PERS_ISACTIVE, PERS_CREATEDDATE, PERS_CREATEDBY) values(1,2,1,CURRENT_TIMESTAMP,1);

exit;