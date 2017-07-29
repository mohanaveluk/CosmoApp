--delete from csm_user;
--delete from csm_roles;
--delete from CSM_USER_ROLE;
--delete from CSM_MENUS;
--delete from CSM_ROLEMENU;
--delete from CSM_POERSONALIZE;

declare
	tableCount int;
Begin	
	select count(*) into tableCount from user_tables where table_name = upper('CSM_ACKNOWLEDGE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_ACKNOWLEDGE';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_EMAIL_CONFIGURATION');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_EMAIL_CONFIGURATION';
	end if;
	
	select count(*) into tableCount from user_tables where table_name = upper('CSM_EMAIL_CONTENT');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_EMAIL_CONTENT';
	end if;
	
	select count(*) into tableCount from user_tables where table_name = upper('CSM_EMAIL_TRACKING');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_EMAIL_TRACKING';
	end if;
	
	select count(*) into tableCount from user_tables where table_name = upper('CSM_LOG');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_LOG';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_SCHEDULE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_SCHEDULE';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_INCIDENT');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_INCIDENT';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_MONITOR');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_MONITOR';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_CONFIGURATION');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_CONFIGURATION';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_ENVIRONEMENT');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_ENVIRONEMENT';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_LDAP_CONFIGURATION');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_LDAP_CONFIGURATION';
	end if;
	
	select count(*) into tableCount from user_tables where table_name = upper('CSM_GROUP_SCHEDULE_DETAIL');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_GROUP_SCHEDULE_DETAIL';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_GROUP_SCHEDULE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_GROUP_SCHEDULE';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_GROUP_DETAIL');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_GROUP_DETAIL';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_GROUP');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_GROUP';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_USER_ROLE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_USER_ROLE';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_ROLEMENU');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_ROLEMENU';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_MENUS');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_MENUS';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_USER');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_USER';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_MON_DAILY_STATUS');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_MON_DAILY_STATUS';
	end if;

	select count(*) into tableCount from user_tables where table_name = upper('CSM_POERSONALIZE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_POERSONALIZE';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_EMAIL_USERLIST');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_EMAIL_USERLIST';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_REPORT_SUBS_DETAILS');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_REPORT_SUBS_DETAILS';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_REPORT_SUBSCRIPTION');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_REPORT_SUBSCRIPTION';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_PORTALMONITOR');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_PORTALMONITOR';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_ROLES');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_ROLES';
	end if;	
	
	select count(*) into tableCount from user_tables where table_name = upper('CSM_SERVERPERFORMANCE_DRIVE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_SERVERPERFORMANCE_DRIVE';
	end if;		

	select count(*) into tableCount from user_tables where table_name = upper('CSM_SERVERPERFORMANCE_SCHEDULE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_SERVERPERFORMANCE_SCHEDULE';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_SERVERPERFORMANCE');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_SERVERPERFORMANCE';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_SERVICEBUILD');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_SERVICEBUILD';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_URLCONFIGURATION');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_URLCONFIGURATION';
	end if;		

	select count(*) into tableCount from user_tables where table_name = upper('CSM_USER_ACCESS');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_USER_ACCESS';
	end if;	

	select count(*) into tableCount from user_tables where table_name = upper('CSM_WINDOWS_SERVICES');
	if (tableCount >= 1) then
		execute immediate 'DROP TABLE CSM_WINDOWS_SERVICES';
	end if;	

	end;
	
/

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_ENVIRONEMENT_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_CONFIGURATION_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	


	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_ACKNOWLEDGE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_EMAIL_CONFIGURATION_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_EMAIL_CONTENT_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_EMAIL_TRACKING_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_EMAIL_USERLIST_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	
	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_GROUP_DETAIL_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	
	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_GROUP_SCHEDULE_DETAIL_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_GROUP_SCHEDULE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_GROUP_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/
	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_INCIDENT_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/
	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_LDAP_CONFIGURATION_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	
	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_LOG_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	
	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_MENUS_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_MON_DAILY_STATUS_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_MONITOR_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_POERSONALIZE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_PORTALMONITOR_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_REPORT_SUBS_DETAILS_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_REPORT_SUBSCRIPTION_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_ROLEMENU_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_ROLES_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_SCHEDULE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_SERVERPERFORMANCE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_SERVICEBUILD_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_SP_DRIVE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_SP_SCHEDULE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_URLCONFIGURATION_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_USER_ACCESS_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_USER_ROLE_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_USER_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP SEQUENCE CSM_WINDOWS_SERVICES_SEQ';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -2289 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP FUNCTION funcListToTableInt_orcl';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP PACKAGE COSMO_COMMON_PACKAGE';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP PACKAGE COSMO_ENVIRONMENT_PACKAGE';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP PACKAGE COSMO_MONITOR_PACKAGE';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP PACKAGE COSMO_SETUP_PACKAGE';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	


	BEGIN
	  EXECUTE IMMEDIATE 'DROP PACKAGE COSMO_USER_PACKAGE';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP type t_role_type';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

	BEGIN
	  EXECUTE IMMEDIATE 'DROP type t_role';
	EXCEPTION
	  WHEN OTHERS THEN
		IF SQLCODE != -4043 THEN
		  RAISE;
		END IF;
	END;	
/	

exit;