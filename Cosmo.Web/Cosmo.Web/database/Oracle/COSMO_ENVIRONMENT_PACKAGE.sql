--select * from CSM_ENVIRONEMENT;

CREATE OR REPLACE PACKAGE "COSMO_ENVIRONMENT_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    FUNCTION FN_CWT_GetEnvId(p_ENV_NAME IN VARCHAR2)  RETURN INTEGER;
    FUNCTION FN_CWT_GetEnvironmentList(p_ENV_ID IN NUMBER)  RETURN o_Cursor;
    FUNCTION FN_CWT_GetEnvConfigID(p_ENV_ID IN NUMBER,
                                  p_ENV_HOST_IP_ADDRESS IN VARCHAR2,
                                  p_ENV_PORT IN VARCHAR2,
                                  p_ENV_SERVICETYPE IN VARCHAR2) RETURN NUMBER;
    FUNCTION FN_CWT_GetEnvIDByConfigId(p_CONFIGID IN NUMBER) RETURN NUMBER;

    FUNCTION FN_CWT_GetEnvConfigList(p_CONFIG_ID IN NUMBER,
                                              p_ENV_ID IN NUMBER) RETURN o_Cursor;

    FUNCTION FN_CWT_GetAllConfigEmail(p_ENV_ID IN NUMBER) RETURN o_Cursor;

    FUNCTION FN_CWT_GetSubsUserEmail(p_ENV_ID IN NUMBER) RETURN o_Cursor;

    FUNCTION FN_CWT_ISServiceExists(p_ENV_HOST_IP_ADDRESS varchar
                                    ,p_ENV_PORT varchar) RETURN VARCHAR2;

    FUNCTION FN_CWT_GetDispatcherConfigID(p_CONFIGREFID number, 
                                        p_CMURL varchar2, 
                                        p_STYPE varchar2) RETURN NUMBER;
    
    FUNCTION FN_CWT_GetConfigurationDetails(p_ENV_ID number) RETURN o_Cursor;
    
    FUNCTION FN_CWT_GetUrlConfiguration(p_ENVID number, p_URLID number) RETURN o_Cursor;
    
    FUNCTION FN_CWT_ISUrlConfigExists(p_ENVID number
                                    ,p_URL_TYPE varchar2
                                    ,p_URL_ADDRESS varchar2) RETURN VARCHAR2;
                                    
    PROCEDURE SP_CWT_DeleteRecord(p_sID IN NUMBER,
                                  p_sType IN VARCHAR2);
    
    procedure SP_CWT_InsUpdEnvironmentConfig(p_ENV_ID number,
                                              p_CONFIG_ID number,
                                              p_ENV_NAME varchar2,
                                              p_ENV_LOCATION varchar2,
                                              p_ENV_HOST_IP_ADDRESS varchar2,
                                              p_ENV_PORT varchar2,
                                              p_ENV_DESCRIPTION varchar2,
                                              p_ENV_SERVICETYPE varchar2,
                                              p_ENV_SERVICEURL varchar2,
                                              p_ENV_MAIL_FREQ varchar2,
                                              p_ENV_IS_MONITOR number,
                                              p_ENV_IS_NOTIFY number,
                                              p_ENV_IS_CONSLTD_MAIL number,
                                              p_ENV_COMMENTS varchar2,
                                              p_ENV_ISACTIVE number,
                                              p_ENV_CREATED_BY varchar2,
                                              p_ENV_CREATED_DATE timestamp,
                                              p_ENV_UPDATED_BY varchar2,
                                              p_ENV_UPDATED_DATE timestamp,
                                              p_CATEGORY varchar2,
                                              p_CONFIG_ISPRIMARY number,
                                              p_CONFIG_REF_ID number,
                                              p_WINDOWS_SERVICE_NAME varchar,
                                              p_WINDOWS_SERVICE_ID number);--, cur OUT o_Cursor);

    procedure SP_CWT_InsUpdEnvironment1(p_ENV_ID number,
                                              p_CONFIG_ID number
                                              ,p_CATEGORY varchar2);

    procedure SP_CWT_InsUpdEmailUsers(
                                    p_USRLST_ID number,
                                    p_ENV_ID number,
                                    p_USRLST_EMAIL_ADDRESS varchar2,
                                    p_USRLST_MESSAGETYPE varchar2,
                                    p_USRLST_TYPE varchar2,
                                    p_USRLST_IS_ACTIVE number,
                                    p_USRLST_CREATED_BY varchar2,
                                    p_USRLST_CREATED_DATE timestamp,
                                    p_USRLST_COMMENTS varchar2
                                );      
    PROCEDURE SP_CWT_InsUpdWindowsService(p_ENV_ID number,
                                  p_CONFIG_ID number,
                                  p_SERVICENAME varchar2,
                                  p_COMMENTS varchar2,
                                  p_CREATED_BY varchar2,
                                  p_CREATED_DATE timestamp);                                
    
    procedure SP_CWT_InsUpdServerPerfSch(p_ENVID number
                                            ,p_CONFIGID number
                                            ,p_HOSTIP varchar2
                                            ,p_PORT varchar2
                                            ,p_LASTJOBRUNTIME timestamp
                                            ,p_NEXTJOBRUNTIME timestamp
                                            ,p_MODE varchar2
                                                  );
    procedure SP_CWT_InsUpdUrlConfiguration
                                        (p_CATEGORY varchar2
                                        ,p_URL_ID number
                                        ,p_ENVID number
                                        ,p_URL_TYPE varchar2
                                        ,p_URL_ADDRESS varchar2
                                        ,p_URL_DISPLAYNAME varchar2
                                        ,p_URL_MATCHCONTENT varchar2
                                        ,p_URL_INTERVAL number
                                        ,p_URL_USERNAME varchar2
                                        ,p_URL_PASSWORD varchar2
                                        ,p_URL_STATUS number
                                        ,p_URL_CREATEDBY varchar2
                                        ,p_URL_UPDATEDBY varchar2
                                        ,p_URL_COMMENTS varchar2
                                        );                                        
                                
    procedure SP_CWT_InsUpdSubscription  (
                                        p_SUBSCRIPTION_ID number
                                        ,p_SUBSCRIPTION_TYPE varchar2 
                                        ,p_SUBSCRIPTION_TIME varchar2 
                                        ,p_SUBSCRIPTION_ISACTIVE number
                                        ,p_CREATED_BY varchar2
                                        ,p_CREATED_DATE timestamp
                                        ,p_UPDATED_BY varchar2
                                        ,p_UPDATED_DATE timestamp
                                        ,p_NEXTJOBRUNTIME timestamp
                                        ,p_SUBSCRIPTION_EMAILS varchar
                                        ,p_SCOPE_OUTPUT out number 
                                      );
END COSMO_ENVIRONMENT_PACKAGE;
/

create or replace PACKAGE BODY "COSMO_ENVIRONMENT_PACKAGE" AS
    FUNCTION FN_CWT_GetEnvId(p_ENV_NAME IN VARCHAR2)  RETURN INTEGER IS
    v_envId integer;
    v_envCount integer;
    BEGIN
        select count(*) into v_envCount from  CSM_ENVIRONEMENT 
			where lower(ENV_NAME) = lower(p_ENV_NAME) 
			and ENV_ISACTIVE = 1;
        if v_envCount > 0 then
            select ENV_ID into v_envId from CSM_ENVIRONEMENT 
                where lower(ENV_NAME) = lower(p_ENV_NAME) 
                and ENV_ISACTIVE = 1;
        else
            v_envId := 0;
        end if;
        
        RETURN v_envId;
    END;

    FUNCTION FN_CWT_GetEnvironmentList(p_ENV_ID IN NUMBER)  RETURN o_Cursor IS
    CUR O_Cursor;
    BEGIN
        if(p_ENV_ID=0 or p_ENV_ID='0' or p_ENV_ID is null)
        Then
            open CUR for select ENV_ID 
            ,ENV_NAME
          --,[ENV_HOST_IP_ADDRESS]
          --,[ENV_LOCATION]
          ,ENV_IS_MONITOR
          ,ENV_IS_NOTIFY
          ,ENV_CREATED_BY
          ,ENV_CREATED_DATE
          ,ENV_UPDATED_BY
          ,ENV_UPDATED_DATE
          ,ENV_COMMENTS
          ,ENV_MAIL_FREQ
          ,ENV_IS_CONSLTD_MAIL		
          ,ENV_SORTORDER
          from CSM_ENVIRONEMENT where ENV_ISACTIVE = 1 order by ENV_SORTORDER;
        else
            open CUR for select ENV_ID 
            ,ENV_NAME
          --,[ENV_HOST_IP_ADDRESS]
          --,[ENV_LOCATION]
          ,ENV_IS_MONITOR
          ,ENV_IS_NOTIFY
          ,ENV_CREATED_BY
          ,ENV_CREATED_DATE
          ,ENV_UPDATED_BY
          ,ENV_UPDATED_DATE
          ,ENV_COMMENTS
          ,ENV_MAIL_FREQ
          ,ENV_IS_CONSLTD_MAIL	
          ,ENV_SORTORDER	
          from CSM_ENVIRONEMENT 
          where ENV_ID = p_ENV_ID and ENV_ISACTIVE = 1  order by ENV_SORTORDER;
        End if;   
        return CUR;
    END;

    FUNCTION FN_CWT_GetEnvConfigID(p_ENV_ID IN NUMBER,
                                  p_ENV_HOST_IP_ADDRESS IN VARCHAR2,
                                  p_ENV_PORT IN VARCHAR2,
                                  p_ENV_SERVICETYPE IN VARCHAR2) RETURN NUMBER IS
    v_ConfigId number;
    v_count number;
    BEGIN
        select count(*) into v_count from CSM_CONFIGURATION 
			where ENV_ID = p_ENV_ID and 
			lower(CONFIG_HOST_IP) = lower(p_ENV_HOST_IP_ADDRESS) and 
			lower(CONFIG_PORT_NUMBER) = lower(p_ENV_PORT) and
			CONFIG_SERVICE_TYPE = p_ENV_SERVICETYPE and
			CONFIG_IS_ACTIVE = 1;
        if(v_count > 0 ) then
            select NVL(CONFIG_ID,0) into v_ConfigId from CSM_CONFIGURATION 
			where ENV_ID = p_ENV_ID and 
			lower(CONFIG_HOST_IP) = lower(p_ENV_HOST_IP_ADDRESS) and 
			--lower([CONFIG_LOCATION]) = lower(@ENV_LOCATION) and
			--lower([CONFIG_URL_ADDRESS]) = lower(@ENV_SERVICEURL) and
			lower(CONFIG_PORT_NUMBER) = lower(p_ENV_PORT) and
			CONFIG_SERVICE_TYPE = p_ENV_SERVICETYPE and
			CONFIG_IS_ACTIVE = 1; 
        else
            v_ConfigId:= 0;
        end if;
        return v_ConfigId;
    END;

    FUNCTION FN_CWT_GetEnvIDByConfigId(p_CONFIGID IN NUMBER) RETURN NUMBER IS
    v_EnvId number;
    BEGIN
		select NVL(ENV_ID, 0) into v_EnvId from CSM_ENVIRONEMENT 
			where ENV_ID in (select distinct ENV_ID from CSM_CONFIGURATION where CONFIG_ID = p_CONFIGID)
			and ENV_ISACTIVE = 1;      
            
        return v_EnvId;
    END;

    FUNCTION FN_CWT_GetEnvConfigList(p_CONFIG_ID IN NUMBER,
                                              p_ENV_ID IN NUMBER) RETURN o_Cursor IS
    CUR O_Cursor;
    BEGIN
        if(p_CONFIG_ID>0 and p_CONFIG_ID is not null)
        Then
            open CUR for select 
           CONFIG_ID
          ,CSM_CONFIGURATION.ENV_ID
          ,CSM_ENVIRONEMENT.ENV_NAME
          ,CONFIG_SERVICE_TYPE
          ,CONFIG_PORT_NUMBER
          ,CONFIG_URL_ADDRESS
          ,CONFIG_DESCRIPTION
          ,CONFIG_IS_VALIDATED
          ,CONFIG_IS_ACTIVE
          ,CONFIG_IS_MONITORED
          ,CONFIG_IS_LOCKED
          ,CONFIG_CREATED_BY
          ,CONFIG_CREATED_DATE
          ,CONFIG_UPDATED_BY
          ,CONFIG_UPDATED_DATE
          ,CONFIG_COMMENTS
          ,CONFIG_HOST_IP
          ,CONFIG_MAIL_FREQ
          ,CONFIG_LOCATION
          ,CONFIG_ISCONSOLIDATED
          ,CONFIG_ISNOTIFY
          ,CONFIG_ISPRIMARY
          ,CONFIG_REF_ID
          ,CSM_SCHEDULE.SCH_ID
          ,CSM_SCHEDULE.SCH_COMMENTS
          ,(Select WIN_SERVICENAME from CSM_WINDOWS_SERVICES win where win.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID) WIN_SERVICENAME
          ,(Select WIN_SERVICE_ID from CSM_WINDOWS_SERVICES win where win.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID) WIN_SERVICE_ID
          ,usr.USER_FIRST_NAME USERFIRSTNAME
          ,usr.USER_LAST_NAME USERLASTNAME
          from CSM_CONFIGURATION 
          inner join CSM_ENVIRONEMENT on
            CSM_CONFIGURATION.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
          left outer join CSM_SCHEDULE on CSM_SCHEDULE.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
              and  CSM_SCHEDULE.ENV_ID = CSM_CONFIGURATION.ENV_ID
              and CSM_SCHEDULE.CONGIG_ID =  CSM_CONFIGURATION.CONFIG_ID
              and CSM_SCHEDULE.SCH_ENDBY >= SYSTIMESTAMP
          join CSM_USER usr on usr.USER_ID = CONFIG_CREATED_BY
          where CONFIG_IS_ACTIVE = 1 and
            CSM_CONFIGURATION.CONFIG_ISPRIMARY = 1 
            and	CONFIG_ID = p_CONFIG_ID;

        else
            if(p_ENV_ID is not null and p_ENV_ID>0)
            Then
                open CUR for select 
                   CONFIG_ID
                  ,CSM_CONFIGURATION.ENV_ID
                  ,CSM_ENVIRONEMENT.ENV_NAME
                  ,CONFIG_SERVICE_TYPE
                  ,CONFIG_PORT_NUMBER
                  ,CONFIG_URL_ADDRESS
                  ,CONFIG_DESCRIPTION
                  ,CONFIG_IS_VALIDATED
                  ,CONFIG_IS_ACTIVE
                  ,CONFIG_IS_MONITORED
                  ,CONFIG_IS_LOCKED
                  ,CONFIG_CREATED_BY
                  ,CONFIG_CREATED_DATE
                  ,CONFIG_UPDATED_BY
                  ,CONFIG_UPDATED_DATE
                  ,CONFIG_COMMENTS
                  ,CONFIG_HOST_IP
                  ,CONFIG_MAIL_FREQ
                  ,CONFIG_LOCATION
                  ,CONFIG_ISCONSOLIDATED
                  ,CONFIG_ISNOTIFY
                  ,CONFIG_ISPRIMARY
                  ,CONFIG_REF_ID
                  ,CSM_SCHEDULE.SCH_ID
                  ,CSM_SCHEDULE.SCH_COMMENTS
                  ,(Select WIN_SERVICENAME from CSM_WINDOWS_SERVICES win where win.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID) WIN_SERVICENAME
                  ,(Select WIN_SERVICE_ID from CSM_WINDOWS_SERVICES win where win.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID) WIN_SERVICE_ID
                  ,usr.USER_FIRST_NAME USERFIRSTNAME
                  ,usr.USER_LAST_NAME USERLASTNAME
              from CSM_CONFIGURATION 
              inner join CSM_ENVIRONEMENT 
              on CSM_CONFIGURATION.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
              left join CSM_SCHEDULE on CSM_SCHEDULE.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
              and  CSM_SCHEDULE.ENV_ID = CSM_CONFIGURATION.ENV_ID
              and CSM_SCHEDULE.CONGIG_ID =  CSM_CONFIGURATION.CONFIG_ID
              and CSM_SCHEDULE.SCH_ENDBY >= SYSTIMESTAMP
              join CSM_USER usr on usr.USER_ID = CONFIG_CREATED_BY
              where CONFIG_IS_ACTIVE = 1 and 	
                CSM_CONFIGURATION.CONFIG_ISPRIMARY = 1 
                and CSM_CONFIGURATION.ENV_ID = p_ENV_ID;	
            else
                open CUR for select
                   CONFIG_ID
                  ,CSM_CONFIGURATION.ENV_ID
                  ,CSM_ENVIRONEMENT.ENV_NAME
                  ,CONFIG_SERVICE_TYPE
                  ,CONFIG_PORT_NUMBER
                  ,CONFIG_URL_ADDRESS
                  ,CONFIG_DESCRIPTION
                  ,CONFIG_IS_VALIDATED
                  ,CONFIG_IS_ACTIVE
                  ,CONFIG_IS_MONITORED
                  ,CONFIG_IS_LOCKED
                  ,CONFIG_CREATED_BY
                  ,CONFIG_CREATED_DATE
                  ,CONFIG_UPDATED_BY
                  ,CONFIG_UPDATED_DATE
                  ,CONFIG_COMMENTS
                  ,CONFIG_HOST_IP
                  ,CONFIG_MAIL_FREQ
                  ,CONFIG_LOCATION
                  ,CONFIG_ISCONSOLIDATED
                  ,CONFIG_ISNOTIFY
                  ,CONFIG_ISPRIMARY
                  ,CONFIG_REF_ID
                  ,CSM_SCHEDULE.SCH_ID
                  ,CSM_SCHEDULE.SCH_COMMENTS
                  ,(Select WIN_SERVICENAME from CSM_WINDOWS_SERVICES win where win.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID) WIN_SERVICENAME
                  ,(Select WIN_SERVICE_ID from CSM_WINDOWS_SERVICES win where win.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID) WIN_SERVICE_ID
                  ,usr.USER_FIRST_NAME USERFIRSTNAME
                  ,usr.USER_LAST_NAME USERLASTNAME
                  from CSM_CONFIGURATION 
              inner join CSM_ENVIRONEMENT on
                CSM_CONFIGURATION.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
              left outer join CSM_SCHEDULE on CSM_SCHEDULE.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
              and  CSM_SCHEDULE.ENV_ID = CSM_CONFIGURATION.ENV_ID
              and CSM_SCHEDULE.CONGIG_ID =  CSM_CONFIGURATION.CONFIG_ID
              and CSM_SCHEDULE.SCH_ENDBY >= SYSTIMESTAMP
              join CSM_USER usr on usr.USER_ID = CONFIG_CREATED_BY
              where CONFIG_IS_ACTIVE = 1 and
                CSM_CONFIGURATION.CONFIG_ISPRIMARY = 1;
            End if;
        End if;    
        return CUR;
    END;

    FUNCTION FN_CWT_GetAllConfigEmail(p_ENV_ID IN NUMBER) RETURN o_Cursor Is
    CUR O_Cursor;
    Begin
    
        if p_ENV_ID is null or p_ENV_ID = 0
            Then
            OPEN CUR FOR SELECT env.ENV_NAME
                  ,USRLST_ID
                  ,eml.ENV_ID
                  ,USRLST_EMAIL_ADDRESS
                  ,USRLST_MESSAGETYPE
                  ,USRLST_TYPE
                  ,USRLST_IS_ACTIVE
                  ,USRLST_CREATED_BY
                  ,USRLST_CREATED_DATE
                  ,USRLST_UPDATED_BY
                  ,USRLST_UPDATED_DATE
                  ,USRLST_COMMENTS
              FROM CSM_EMAIL_USERLIST eml
              inner join CSM_ENVIRONEMENT env on env.ENV_ID = eml.ENV_ID 
              where env.ENV_ISACTIVE = 1
                and eml.USRLST_IS_ACTIVE = 1
                order by env.ENV_NAME;
        Else
            OPEN CUR FOR SELECT env.ENV_NAME
                  ,USRLST_ID
                  ,eml.ENV_ID
                  ,USRLST_EMAIL_ADDRESS
                  ,USRLST_MESSAGETYPE
                  ,USRLST_TYPE
                  ,USRLST_IS_ACTIVE
                  ,USRLST_CREATED_BY
                  ,USRLST_CREATED_DATE
                  ,USRLST_UPDATED_BY
                  ,USRLST_UPDATED_DATE
                  ,USRLST_COMMENTS
              FROM CSM_EMAIL_USERLIST eml
              inner join CSM_ENVIRONEMENT env on env.ENV_ID = eml.ENV_ID 
              where env.ENV_ISACTIVE = 1
                and eml.USRLST_IS_ACTIVE = 1
                and env.ENV_ID = p_ENV_ID
                order by env.ENV_NAME;
                
        End if;	
        return CUR;
    End;

    Function FN_CWT_GetSubsUserEmail(p_ENV_ID IN NUMBER) RETURN o_Cursor Is
    CUR o_Cursor;
    Begin
		if p_ENV_ID > 0
        Then
            Open CUR FOR Select distinct 
                eu.USRLST_EMAIL_ADDRESS
                ,rsd.SUBSCRIPTION_ID
                ,rs.SUBSCRIPTION_TYPE
                ,rs.SUBSCRIPTION_TIME
                ,rs.SUBSCRIPTION_ISACTIVE
                ,rsd.SUBSCRIPTION_DETAIL_ID
                ,rsd.SUBSCRIPTION_DETAIL_ID
                ,rsd.USRLST_ID as SUBSCRIPTION_USRLST_ID
                ,rsd.SUBSCRIPTION_EMAILID
            from CSM_EMAIL_USERLIST eu
            LEFT JOIN CSM_REPORT_SUBS_DETAILS rsd 
            on rsd.SUBSCRIPTION_EMAILID = eu.USRLST_EMAIL_ADDRESS
            LEFT JOIN  CSM_REPORT_SUBSCRIPTION rs 
            ON rs.SUBSCRIPTION_ID = rsd.SUBSCRIPTION_ID
            LEFT JOIN  CSM_ENVIRONEMENT en
            on en.ENV_ID = eu.ENV_ID
            where en.ENV_ISACTIVE = 1
            and eu.USRLST_IS_ACTIVE = 1	
            and en.ENV_ID = p_ENV_ID	
            order by USRLST_EMAIL_ADDRESS;			
        Else
            Open CUR FOR Select distinct 
                eu.USRLST_EMAIL_ADDRESS
                ,rsd.SUBSCRIPTION_ID
                ,rs.SUBSCRIPTION_TYPE
                ,rs.SUBSCRIPTION_TIME
                ,rs.SUBSCRIPTION_ISACTIVE
                ,rsd.SUBSCRIPTION_DETAIL_ID
                ,rsd.USRLST_ID as SUBSCRIPTION_USRLST_ID
                ,rsd.SUBSCRIPTION_EMAILID
            from CSM_EMAIL_USERLIST eu
            LEFT JOIN CSM_REPORT_SUBS_DETAILS rsd 
            on rsd.SUBSCRIPTION_EMAILID = eu.USRLST_EMAIL_ADDRESS
            LEFT JOIN  CSM_REPORT_SUBSCRIPTION rs 
            ON rs.SUBSCRIPTION_ID = rsd.SUBSCRIPTION_ID
            LEFT JOIN  CSM_ENVIRONEMENT en
            on en.ENV_ID = eu.ENV_ID
            where en.ENV_ISACTIVE = 1
            and eu.USRLST_IS_ACTIVE = 1		
            order by USRLST_EMAIL_ADDRESS;
        End if;        
        return CUR;
    End;
    
    FUNCTION FN_CWT_ISServiceExists(p_ENV_HOST_IP_ADDRESS varchar
                                    ,p_ENV_PORT varchar) RETURN VARCHAR2 IS
    
    v_EnvirontmentDetails_ID number;
    v_Environtment_Name varchar2(500);
    v_Environtment_ID number;
    p_SCOPE_OUTPUT varchar2(500):='';
    
    Begin
        
        select con.CONFIG_ID
		, env.ENV_NAME 
		, env.ENV_ID into v_EnvirontmentDetails_ID, v_Environtment_Name, v_Environtment_ID
		from CSM_CONFIGURATION con
		inner join CSM_ENVIRONEMENT env on env.ENV_ID = con.ENV_ID
		where lower(CONFIG_HOST_IP) = lower(p_ENV_HOST_IP_ADDRESS)
		and lower(CONFIG_PORT_NUMBER) = lower(p_ENV_PORT)
		and CONFIG_IS_ACTIVE = 1
		and CONFIG_ISPRIMARY = 1;
        
        if v_EnvirontmentDetails_ID > 0
        Then
            p_SCOPE_OUTPUT := 'Service already exists under ' || v_Environtment_Name || ' Environment|' || v_Environtment_ID || '|' || v_EnvirontmentDetails_ID;
        Else
            p_SCOPE_OUTPUT := '';
        End if;
        
        return p_SCOPE_OUTPUT;

        Exception when others then
            return '';
            
    End;
    
    FUNCTION FN_CWT_GetDispatcherConfigID(p_CONFIGREFID number, 
                                        p_CMURL varchar2, 
                                        p_STYPE varchar2) RETURN NUMBER IS
    v_count NUMBER := 0;   
    v_CONDIGID NUMBER := 0;
    Begin

        if(p_STYPE='REFID')
        Then
            Select count(*) into v_count from CSM_CONFIGURATION where CONFIG_REF_ID = p_CONFIGREFID;
            If v_count > 0 then
                Select CONFIG_ID into v_CONDIGID from CSM_CONFIGURATION where CONFIG_REF_ID = p_CONFIGREFID;
            End if;
        else if(p_STYPE='CMURL')
        Then
            Select count(*) into v_count from CSM_CONFIGURATION where CONFIG_URL_ADDRESS like p_CMURL || '%' and CONFIG_URL_ADDRESS != p_CMURL;
            If v_count > 0 then
                Select CONFIG_ID into v_CONDIGID from CSM_CONFIGURATION where CONFIG_URL_ADDRESS like p_CMURL || '%' and CONFIG_URL_ADDRESS != p_CMURL;
            End if;
        End if;
        end if;
    
        return v_CONDIGID;
    End;
                
    FUNCTION FN_CWT_GetConfigurationDetails(p_ENV_ID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if(p_ENV_ID>0 or p_ENV_ID is not null)
        Then
            open CUR for select 
               con.CONFIG_ID
              ,con.ENV_ID
              ,CSM_ENVIRONEMENT.ENV_NAME
              ,CONFIG_SERVICE_TYPE
              ,CONFIG_PORT_NUMBER
              ,CONFIG_URL_ADDRESS
              ,CONFIG_DESCRIPTION
              ,CONFIG_IS_VALIDATED
              ,CONFIG_IS_ACTIVE
              ,CONFIG_IS_MONITORED
              ,CONFIG_IS_LOCKED
              ,CONFIG_CREATED_BY
              ,CONFIG_CREATED_DATE
              ,CONFIG_UPDATED_BY
              ,CONFIG_UPDATED_DATE
              ,CONFIG_COMMENTS
              ,CONFIG_HOST_IP
              ,CONFIG_MAIL_FREQ
              ,CONFIG_LOCATION
              ,CONFIG_ISCONSOLIDATED
              ,CONFIG_ISNOTIFY
              ,CONFIG_ISPRIMARY
              ,win.WIN_SERVICENAME
              ,win.WIN_SERVICE_ID
              ,usr.USER_FIRST_NAME USERFIRSTNAME
              ,usr.USER_LAST_NAME USERLASTNAME
          from CSM_CONFIGURATION con
          join CSM_ENVIRONEMENT on con.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
          left outer join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
          join CSM_USER usr on usr.USER_ID = CONFIG_CREATED_BY
            where CONFIG_IS_ACTIVE = 1 and 	
            con.ENV_ID = p_ENV_ID;
            
        End if;    
        
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetUrlConfiguration(p_ENVID number, p_URLID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if p_ENVID > 0
            Then
                OPEN CUR FOR SELECT uc.URL_ID
                  ,uc.ENV_ID
                  ,env.ENV_NAME
                  ,uc.URL_TYPE
                  ,uc.URL_ADDRESS
                  ,uc.URL_DISPLAYNAME
                  ,uc.URL_MATCHCONTENT
                  ,uc.URL_INTERVAL
                  ,uc.URL_USERNAME
                  ,uc.URL_PASSWORD
                  ,uc.URL_ISACTIVE
                  ,uc.URL_STATUS
                  ,uc.URL_CREATEDBY
                  ,uc.URL_CREATEDDATE
                  ,uc.URL_UPDATEDBY
                  ,uc.URL_UPDATEDDATE
                  ,uc.URL_COMMENTS
              FROM CSM_URLCONFIGURATION uc
              left join CSM_ENVIRONEMENT env
              On env.ENV_ID = uc.ENV_ID
              where uc.ENV_ID = p_ENVID
              and URL_ISACTIVE = 1;
         else if p_URLID > 0
            Then
                OPEN CUR FOR SELECT uc.URL_ID
                  ,uc.ENV_ID
                  ,env.ENV_NAME
                  ,uc.URL_TYPE
                  ,uc.URL_ADDRESS
                  ,uc.URL_DISPLAYNAME
                  ,uc.URL_MATCHCONTENT
                  ,uc.URL_INTERVAL
                  ,uc.URL_USERNAME
                  ,uc.URL_PASSWORD
                  ,uc.URL_STATUS
                  ,uc.URL_ISACTIVE
                  ,uc.URL_CREATEDBY
                  ,uc.URL_CREATEDDATE
                  ,uc.URL_UPDATEDBY
                  ,uc.URL_UPDATEDDATE
                  ,uc.URL_COMMENTS
              FROM CSM_URLCONFIGURATION uc
              left join CSM_ENVIRONEMENT env
              On env.ENV_ID = uc.ENV_ID
              where URL_ID = p_URLID
              and URL_ISACTIVE = 1;
         else if p_ENVID <= 0 and p_URLID <= 0
            Then
                OPEN CUR FOR SELECT uc.URL_ID
                  ,uc.ENV_ID
                  ,env.ENV_NAME
                  ,uc.URL_TYPE
                  ,uc.URL_ADDRESS
                  ,uc.URL_DISPLAYNAME
                  ,uc.URL_MATCHCONTENT
                  ,uc.URL_INTERVAL
                  ,uc.URL_USERNAME
                  ,uc.URL_PASSWORD
                  ,uc.URL_STATUS
                  ,uc.URL_ISACTIVE
                  ,uc.URL_CREATEDBY
                  ,uc.URL_CREATEDDATE
                  ,uc.URL_UPDATEDBY
                  ,uc.URL_UPDATEDDATE
                  ,uc.URL_COMMENTS
              FROM CSM_URLCONFIGURATION uc
              left join CSM_ENVIRONEMENT env
              On env.ENV_ID = uc.ENV_ID
              where URL_ISACTIVE = 1
              order by env.ENV_SORTORDER;
          End if;
        end if;
        end if;	  
        
        RETURN CUR;    
    End;

    FUNCTION FN_CWT_ISUrlConfigExists(p_ENVID number
                                    ,p_URL_TYPE varchar2
                                    ,p_URL_ADDRESS varchar2) RETURN VARCHAR2 IS
	v_urlId varchar2(10);
	v_Environtment_Name varchar(1000);
	v_Environtment_ID varchar2(10);
    v_count NUMBER;
    p_ScopeOutput varchar2(300) := '';
    Begin
        
        select count(*) into v_count
            from CSM_URLCONFIGURATION con
            inner join CSM_ENVIRONEMENT env 
            on env.ENV_ID = con.ENV_ID
            where lower(URL_TYPE) = lower(p_URL_TYPE)
            and con.ENV_ID = p_ENVID
            and URL_ISACTIVE = 1;
        
        if v_count > 0 then
            select con.URL_ID, env.ENV_NAME into v_urlId, v_Environtment_Name
            from CSM_URLCONFIGURATION con
            inner join CSM_ENVIRONEMENT env 
            on env.ENV_ID = con.ENV_ID
            where lower(URL_TYPE) = lower(p_URL_TYPE)
            and con.ENV_ID = p_ENVID
            and URL_ISACTIVE = 1;

            if v_urlId > 0
            Then
                p_ScopeOutput := 'Url Configuration already exists under ' || v_Environtment_Name || ' with Type ' || p_URL_TYPE || ', Please click on edit icon to update details|' || p_URL_TYPE || '|' || v_urlId;
            End if;  
        End if;
        
        RETURN p_ScopeOutput;
    End;
    
    
    PROCEDURE SP_CWT_DeleteRecord(p_sID IN NUMBER,
                                  p_sType IN VARCHAR2) IS
    BEGIN
        if lower(p_sType) = 'env'
        Then
            Update CSM_ENVIRONEMENT set ENV_ISACTIVE = 0 
                where ENV_ID = p_sID;
            Update CSM_CONFIGURATION set CONFIG_IS_ACTIVE = 0 
                where (ENV_ID = p_sID);
        else if lower(p_sType) = 'cfg'
        Then
            Update CSM_CONFIGURATION set CONFIG_IS_ACTIVE = 0 
                where (CONFIG_ID = p_sID or CONFIG_REF_ID = p_sID);
        else if lower(p_sType) = 'other'
        Then
            Update CSM_ENVIRONEMENT set ENV_ISACTIVE = 0 
                where ENV_ID = p_sID;
        else if lower(p_sType) = 'email'
        Then
            Update CSM_EMAIL_USERLIST set USRLST_IS_ACTIVE = 0 
                where USRLST_ID = p_sID;
        else if lower(p_sType) = 'ss_notify_stop'
        Then
            Update CSM_CONFIGURATION set CONFIG_ISNOTIFY = 0 
                where CONFIG_ID = p_sID or CONFIG_REF_ID = p_sID;
        else if lower(p_sType) = 'ss_notify_start'
        Then
            Update CSM_CONFIGURATION set CONFIG_ISNOTIFY = 1
                where CONFIG_ID = p_sID or CONFIG_REF_ID = p_sID;
        else if lower(p_sType) = 'user'
        Then
            Update CSM_USER set USER_IS_DELETED = 1
                where USER_ID = p_sID;
            update CSM_USER_ROLE set CSM_USER_ROLE.USER_ROLE_ISACTIVE = 0
                where USER_ID = p_sID;
        else if (lower(p_sType) = 'grpsch')
        Then
            update CSM_GROUP_SCHEDULE set GROUP_SCH_STATUS = 'C', GROUP_SCH_UPDATED_DATETIME = SYSTIMESTAMP
            where GROUP_SCH_ID = p_sID;

            update CSM_GROUP_SCHEDULE_DETAIL  set GROUP_SCH_STATUS = 'C' 
            where GROUP_SCH_ID = p_sID;
        else if lower(p_sType) = 'urlconfig'
        Then
            Update CSM_URLCONFIGURATION set URL_ISACTIVE = 0
                where URL_ID = p_sID;
        End if;
         end if;
         end if;
         end if;
         end if;
         end if;
         end if;
         end if;
        end if;    
    END;

    PROCEDURE SP_CWT_InsUpdEnvironmentConfig
    (	  
          p_ENV_ID number,
          p_CONFIG_ID number,
          p_ENV_NAME varchar2,
          p_ENV_LOCATION varchar2,
          p_ENV_HOST_IP_ADDRESS varchar2,
          p_ENV_PORT varchar2,
          p_ENV_DESCRIPTION varchar2,
          p_ENV_SERVICETYPE varchar2,
          p_ENV_SERVICEURL varchar2,
          p_ENV_MAIL_FREQ varchar2,
          p_ENV_IS_MONITOR number,
          p_ENV_IS_NOTIFY number,
          p_ENV_IS_CONSLTD_MAIL number,
          p_ENV_COMMENTS varchar2,
          p_ENV_ISACTIVE number,
          p_ENV_CREATED_BY varchar2,
          p_ENV_CREATED_DATE timestamp,
          p_ENV_UPDATED_BY varchar2,
          p_ENV_UPDATED_DATE timestamp,
          p_CATEGORY varchar2,
          p_CONFIG_ISPRIMARY number,
          p_CONFIG_REF_ID number,
          p_WINDOWS_SERVICE_NAME varchar,
          p_WINDOWS_SERVICE_ID number)--, cur OUT o_Cursor)
    IS
    v_Environtment_ID number(10);
    v_EnvirontmentDetails_ID number(10);
    v_Environtment_Name varchar(4000);
    v_CurrentTime timestamp;
    v_count number;
    BEGIN
		if(p_ENV_ID is not null and p_ENV_ID > 0)
			Then
				if((p_CONFIG_ID is null or p_CONFIG_ID <= 0) and p_CATEGORY = 'add_ed')
				Then
					insert into CSM_CONFIGURATION (
					   ENV_ID
					  ,CONFIG_SERVICE_TYPE
					  ,CONFIG_PORT_NUMBER
					  ,CONFIG_URL_ADDRESS
					  ,CONFIG_DESCRIPTION
					  ,CONFIG_IS_VALIDATED
					  ,CONFIG_IS_ACTIVE
					  ,CONFIG_IS_MONITORED
					  ,CONFIG_IS_LOCKED
					  ,CONFIG_CREATED_BY
					  ,CONFIG_CREATED_DATE
					  ,CONFIG_COMMENTS
					  ,CONFIG_HOST_IP
					  ,CONFIG_MAIL_FREQ
					  ,CONFIG_LOCATION
					  ,CONFIG_ISCONSOLIDATED
					  ,CONFIG_ISNOTIFY
					  ,CONFIG_ISPRIMARY
					  ,CONFIG_REF_ID
					  ,CONFIG_ISNOTIFY_MAIN
					  	)
					  values(
					   p_ENV_ID
					  ,p_ENV_SERVICETYPE
					  ,p_ENV_PORT
					  ,p_ENV_SERVICEURL
					  ,p_ENV_DESCRIPTION
					  ,1
					  ,1
					  ,p_ENV_IS_MONITOR
					  ,1
					  ,p_ENV_CREATED_BY
					  ,p_ENV_CREATED_DATE
					  ,p_ENV_COMMENTS
					  ,p_ENV_HOST_IP_ADDRESS
					  ,p_ENV_MAIL_FREQ
					  ,p_ENV_LOCATION
					  ,p_ENV_IS_CONSLTD_MAIL
					  ,p_ENV_IS_NOTIFY
					  ,p_CONFIG_ISPRIMARY
					  ,p_CONFIG_REF_ID
					  ,p_ENV_IS_NOTIFY
					  )					RETURNING  CONFIG_ID into v_EnvirontmentDetails_ID;-- := 1;-- IDENT_CURRENT('CSM_CONFIGURATION');
				else if(p_CONFIG_ID is not null  and p_CONFIG_ID > 0 and p_CATEGORY = 'modify_ed')
				Then
					Update CSM_CONFIGURATION set 
					   ENV_ID = p_ENV_ID
					  ,CONFIG_SERVICE_TYPE = p_ENV_SERVICETYPE
					  ,CONFIG_PORT_NUMBER = p_ENV_PORT
					  ,CONFIG_URL_ADDRESS = p_ENV_SERVICEURL
					  ,CONFIG_DESCRIPTION = p_ENV_DESCRIPTION
					  ,CONFIG_IS_VALIDATED = 1
					  ,CONFIG_IS_ACTIVE = 1
					  ,CONFIG_IS_MONITORED = p_ENV_IS_MONITOR
					  ,CONFIG_IS_LOCKED = 1
					  ,CONFIG_UPDATED_BY = p_ENV_UPDATED_BY
					  ,CONFIG_UPDATED_DATE = p_ENV_UPDATED_DATE
					  ,CONFIG_COMMENTS = p_ENV_COMMENTS
					  ,CONFIG_HOST_IP = p_ENV_HOST_IP_ADDRESS
					  ,CONFIG_MAIL_FREQ = p_ENV_MAIL_FREQ
					  ,CONFIG_LOCATION = p_ENV_LOCATION
					  ,CONFIG_ISCONSOLIDATED = p_ENV_IS_CONSLTD_MAIL
					  ,CONFIG_ISNOTIFY = p_ENV_IS_NOTIFY
					  ,CONFIG_ISPRIMARY = p_CONFIG_ISPRIMARY
					  ,CONFIG_REF_ID = p_CONFIG_REF_ID
					  ,CONFIG_ISNOTIFY_MAIN = p_ENV_IS_NOTIFY
					  where CONFIG_ID = p_CONFIG_ID;
					  v_EnvirontmentDetails_ID := p_CONFIG_ID;
					  
					  if p_ENV_SERVICETYPE = '2'
					  then
						update CSM_CONFIGURATION set  CONFIG_IS_ACTIVE = 0 
							where  CONFIG_REF_ID = p_CONFIG_ID 
								and CONFIG_SERVICE_TYPE = p_ENV_SERVICETYPE
								and CONFIG_URL_ADDRESS = p_ENV_SERVICEURL;
					  end if;
					  
					  if p_ENV_IS_NOTIFY = 1
					  Then
						Update CSM_ENVIRONEMENT set ENV_IS_NOTIFY =  p_ENV_IS_NOTIFY where ENV_ID = p_ENV_ID;
					  End if;
					  
					  if p_ENV_IS_MONITOR = 1
					  Then
						Update CSM_ENVIRONEMENT set ENV_IS_MONITOR =  p_ENV_IS_MONITOR where ENV_ID = p_ENV_ID;
					  End if;

					  if p_ENV_IS_CONSLTD_MAIL = 1
					  Then
						Update CSM_ENVIRONEMENT set ENV_IS_CONSLTD_MAIL =  p_ENV_IS_CONSLTD_MAIL where ENV_ID = p_ENV_ID;
					  End if;
					  
				else if(p_CONFIG_ID is not null  and p_CONFIG_ID <= 0 and p_CATEGORY = 'modify_ed')
				Then
					insert into CSM_CONFIGURATION (
					   ENV_ID
					  ,CONFIG_SERVICE_TYPE
					  ,CONFIG_PORT_NUMBER
					  ,CONFIG_URL_ADDRESS
					  ,CONFIG_DESCRIPTION
					  ,CONFIG_IS_VALIDATED
					  ,CONFIG_IS_ACTIVE
					  ,CONFIG_IS_MONITORED
					  ,CONFIG_IS_LOCKED
					  ,CONFIG_CREATED_BY
					  ,CONFIG_CREATED_DATE
					  ,CONFIG_COMMENTS
					  ,CONFIG_HOST_IP
					  ,CONFIG_MAIL_FREQ
					  ,CONFIG_LOCATION
					  ,CONFIG_ISCONSOLIDATED
					  ,CONFIG_ISNOTIFY
					  ,CONFIG_ISPRIMARY
					  ,CONFIG_REF_ID
					  ,CONFIG_ISNOTIFY_MAIN
					  	)
					  values(
					   p_ENV_ID
					  ,p_ENV_SERVICETYPE
					  ,p_ENV_PORT
					  ,p_ENV_SERVICEURL
					  ,p_ENV_DESCRIPTION
					  ,1
					  ,1
					  ,p_ENV_IS_MONITOR
					  ,1
					  ,p_ENV_CREATED_BY
					  ,p_ENV_CREATED_DATE
					  ,p_ENV_COMMENTS
					  ,p_ENV_HOST_IP_ADDRESS
					  ,p_ENV_MAIL_FREQ
					  ,p_ENV_LOCATION
					  ,p_ENV_IS_CONSLTD_MAIL
					  ,p_ENV_IS_NOTIFY
					  ,p_CONFIG_ISPRIMARY
					  ,p_CONFIG_REF_ID
					  ,p_ENV_IS_NOTIFY
					  ) RETURNING config_id into v_EnvirontmentDetails_ID;-- := IDENT_CURRENT('CSM_CONFIGURATION');
                      --select IDENT_CURRENT('CSM_CONFIGURATION') into v_EnvirontmentDetails_ID from dual;
				else if p_CATEGORY = 'modify_en'
				Then
					Update CSM_ENVIRONEMENT set
						   ENV_NAME				=  p_ENV_NAME
						  ,ENV_IS_MONITOR			=  p_ENV_IS_MONITOR
						  ,ENV_IS_NOTIFY			=  p_ENV_IS_NOTIFY
						  ,ENV_UPDATED_BY			=  p_ENV_UPDATED_BY
						  ,ENV_UPDATED_DATE		=  p_ENV_UPDATED_DATE
						  ,ENV_COMMENTS			=  p_ENV_COMMENTS
						  ,ENV_MAIL_FREQ			=  p_ENV_MAIL_FREQ
						  ,ENV_IS_CONSLTD_MAIL	=  p_ENV_IS_CONSLTD_MAIL
						where ENV_ID = p_ENV_ID;
						v_Environtment_ID  := p_ENV_ID;
						
					Update CSM_CONFIGURATION set
					CONFIG_IS_MONITORED = p_ENV_IS_MONITOR,
					CONFIG_ISNOTIFY = p_ENV_IS_NOTIFY,
					CONFIG_ISCONSOLIDATED = p_ENV_IS_CONSLTD_MAIL
					where ENV_ID = p_ENV_ID;
                end if;
                end if;
                end if;
                end if;  
                v_Environtment_ID := p_ENV_ID;
        else
            select count(*) into v_count from CSM_ENVIRONEMENT 
					where lower(ENV_NAME) = lower(p_ENV_NAME) 
					and ENV_ISACTIVE = 1;
            if v_count > 0 then
                select ENV_ID into v_Environtment_ID from CSM_ENVIRONEMENT 
					where lower(ENV_NAME) = lower(p_ENV_NAME) 
					and ENV_ISACTIVE = 1;
					--and 
					--[ENV_HOST_IP_ADDRESS] = @ENV_HOST_IP_ADDRESS and 
					--[ENV_LOCATION] = @ENV_LOCATION
				--Insert / update environment 
            else 
                v_Environtment_ID := 0;
            end if;
            if (v_Environtment_ID > 0 and v_Environtment_ID is not null)
            Then
                Begin 
                Update CSM_ENVIRONEMENT set
                   ENV_NAME				=  p_ENV_NAME
                  ,ENV_IS_MONITOR			=  p_ENV_IS_MONITOR
                  ,ENV_IS_NOTIFY			=  p_ENV_IS_NOTIFY
                  ,ENV_UPDATED_BY			=  p_ENV_UPDATED_BY
                  ,ENV_UPDATED_DATE		=  p_ENV_UPDATED_DATE
                  ,ENV_COMMENTS			=  p_ENV_COMMENTS
                  ,ENV_MAIL_FREQ			=  p_ENV_MAIL_FREQ
                  ,ENV_IS_CONSLTD_MAIL	=  p_ENV_IS_CONSLTD_MAIL
                where ENV_ID = v_Environtment_ID;
                
                    commit;
                    --open cur for select 0 from dual;
                EXCEPTION WHEN OTHERS THEN
                    rollback;
                    --open cur for select -1 from dual;
                  
                End	;
            else
            
                Begin
                    insert into CSM_ENVIRONEMENT (
                   ENV_NAME

                  ,ENV_IS_MONITOR
                  ,ENV_IS_NOTIFY
                  ,ENV_CREATED_BY
                  ,ENV_CREATED_DATE
                  --,[ENV_COMMENTS]
                  ,ENV_MAIL_FREQ
                  ,ENV_IS_CONSLTD_MAIL
                  ,ENV_ISACTIVE
                  ,ENV_SORTORDER
                )
                values
                (
                    p_ENV_NAME
                --  ,@ENV_HOST_IP_ADDRESS
                --  ,@ENV_LOCATION
                  ,p_ENV_IS_MONITOR
                  ,p_ENV_IS_NOTIFY
                  ,p_ENV_CREATED_BY
                  ,p_ENV_CREATED_DATE
                  --,@ENV_COMMENTS
                  ,p_ENV_MAIL_FREQ
                  ,p_ENV_IS_CONSLTD_MAIL
                  ,p_ENV_ISACTIVE
                  ,(SELECT 
                      CASE WHEN MAX(ENV_SORTORDER)IS NULL THEN 1 ELSE MAX(ENV_SORTORDER)+1 END AS ENV_SORTORDER
                      FROM CSM_ENVIRONEMENT)
                ) RETURNING ENV_ID into v_Environtment_ID;--  := IDENT_CURRENT('CSM_ENVIRONEMENT');
                
                commit;
                
                    --open cur for select 0 from dual;
                EXCEPTION WHEN OTHERS THEN
                    rollback;
                    --open cur for select -1 from dual;
                  
                End;
            --Insert / update environment details to details table
                
                if v_EnvirontmentDetails_ID = '' or v_EnvirontmentDetails_ID is null or v_EnvirontmentDetails_ID <= 0
                Then
                    insert into CSM_CONFIGURATION (
                       ENV_ID
                      ,CONFIG_SERVICE_TYPE
                      ,CONFIG_PORT_NUMBER
                      ,CONFIG_URL_ADDRESS
                      ,CONFIG_DESCRIPTION
                      ,CONFIG_IS_VALIDATED
                      ,CONFIG_IS_ACTIVE
                      ,CONFIG_IS_MONITORED
                      ,CONFIG_IS_LOCKED
                      ,CONFIG_CREATED_BY
                      ,CONFIG_CREATED_DATE
                      ,CONFIG_COMMENTS
                      ,CONFIG_HOST_IP
                      ,CONFIG_MAIL_FREQ
                      ,CONFIG_LOCATION
                      ,CONFIG_ISCONSOLIDATED	
                      ,CONFIG_ISNOTIFY
                      ,CONFIG_ISPRIMARY
                      ,CONFIG_REF_ID
                      ,CONFIG_ISNOTIFY_MAIN
                       )
                      values(
                       v_Environtment_ID
                      ,p_ENV_SERVICETYPE
                      ,p_ENV_PORT
                      ,p_ENV_SERVICEURL
                      ,p_ENV_DESCRIPTION
                      ,1
                      ,1
                      ,p_ENV_IS_MONITOR
                      ,1
                      ,p_ENV_CREATED_BY
                      ,p_ENV_CREATED_DATE
                      ,p_ENV_COMMENTS
                      ,p_ENV_HOST_IP_ADDRESS
                      ,p_ENV_MAIL_FREQ
                      ,p_ENV_LOCATION
                      ,p_ENV_IS_CONSLTD_MAIL
                      ,p_ENV_IS_NOTIFY
                      ,p_CONFIG_ISPRIMARY
                      ,p_CONFIG_REF_ID
                      ,p_ENV_IS_NOTIFY
                      ) RETURNING CONFIG_ID into v_EnvirontmentDetails_ID;-- := IDENT_CURRENT('CSM_CONFIGURATION');
                END IF;    
            END IF;
        end if;
        if v_EnvirontmentDetails_ID > 0 and p_CONFIG_ISPRIMARY = 1
        Then
            v_CurrentTime := SYSTIMESTAMP;
            
            SP_CWT_InsUpdWindowsService(
                v_Environtment_ID
                ,v_EnvirontmentDetails_ID
                ,p_WINDOWS_SERVICE_NAME
                ,''
                ,p_ENV_CREATED_BY
                ,p_ENV_CREATED_DATE) ;
                
             SP_CWT_InsUpdServerPerfSch(
                v_Environtment_ID	
                ,v_EnvirontmentDetails_ID
                ,p_ENV_HOST_IP_ADDRESS
                ,p_ENV_PORT
                ,v_CurrentTime
                ,v_CurrentTime
                ,'IS');
            
        END IF;                    
    END;
    
    procedure SP_CWT_InsUpdEnvironment1(p_ENV_ID number,
                                              p_CONFIG_ID number,
                                              p_CATEGORY varchar2) IS
    Begin
    DBMS_OUTPUT.PUT_LINE('asasas');
            
    End;

    procedure SP_CWT_InsUpdEmailUsers(
                                    p_USRLST_ID number,
                                    p_ENV_ID number,
                                    p_USRLST_EMAIL_ADDRESS varchar2,
                                    p_USRLST_MESSAGETYPE varchar2,
                                    p_USRLST_TYPE varchar2,
                                    p_USRLST_IS_ACTIVE number,
                                    p_USRLST_CREATED_BY varchar2,
                                    p_USRLST_CREATED_DATE timestamp,
                                    p_USRLST_COMMENTS varchar2
                                ) IS
        v_tempUSRLST_ID number;
        v_genericUserLstId number;
        v_count int;
    Begin
        v_tempUSRLST_ID  := 0;
        v_genericUserLstId:= p_USRLST_ID;
        
        SELECT  count(*) INTO v_count from CSM_EMAIL_USERLIST 
            where USRLST_EMAIL_ADDRESS = p_USRLST_EMAIL_ADDRESS
                and ENV_ID = p_ENV_ID;
        if v_count > 0 then
            SELECT  USRLST_ID INTO v_tempUSRLST_ID from CSM_EMAIL_USERLIST 
                where USRLST_EMAIL_ADDRESS = p_USRLST_EMAIL_ADDRESS
                    and ENV_ID = p_ENV_ID;
            
                
            if v_tempUSRLST_ID > 0
            then
                if(p_USRLST_ID is null or p_USRLST_ID <= 0)
                Then
                    v_genericUserLstId := v_tempUSRLST_ID;
                End if;
            end if;   
        end if;  
        
        if v_genericUserLstId is null or v_genericUserLstId <=0
        Then
          insert into CSM_EMAIL_USERLIST
          (
               ENV_ID
              ,USRLST_EMAIL_ADDRESS
              ,USRLST_MESSAGETYPE
              ,USRLST_TYPE
              ,USRLST_IS_ACTIVE
              ,USRLST_CREATED_BY
              ,USRLST_CREATED_DATE
              ,USRLST_COMMENTS
          )
          values
          (
               p_ENV_ID
              ,p_USRLST_EMAIL_ADDRESS
              ,p_USRLST_MESSAGETYPE
              ,p_USRLST_TYPE
              ,p_USRLST_IS_ACTIVE
              ,p_USRLST_CREATED_BY
              ,p_USRLST_CREATED_DATE
              ,p_USRLST_COMMENTS
          );
        else if v_genericUserLstId > 0
        Then
            Update CSM_EMAIL_USERLIST set 
                USRLST_EMAIL_ADDRESS = p_USRLST_EMAIL_ADDRESS,
                USRLST_MESSAGETYPE = p_USRLST_MESSAGETYPE,
                USRLST_TYPE = p_USRLST_TYPE,
                USRLST_UPDATED_BY = p_USRLST_CREATED_BY,
                USRLST_UPDATED_DATE = p_USRLST_CREATED_DATE,
                USRLST_IS_ACTIVE = p_USRLST_IS_ACTIVE,
                USRLST_COMMENTS = p_USRLST_COMMENTS
            Where USRLST_ID = v_genericUserLstId;
        End if;
        end if;
    
        
        
    End;

    PROCEDURE SP_CWT_InsUpdWindowsService(p_ENV_ID number,
                                  p_CONFIG_ID number,
                                  p_SERVICENAME varchar2,
                                  p_COMMENTS varchar2,
                                  p_CREATED_BY varchar2,
                                  p_CREATED_DATE timestamp) AS
    v_WS_ID number;      
    v_count number;
    BEGIN
        select count(*) into v_count from CSM_WINDOWS_SERVICES 
					where ENV_ID = p_ENV_ID
							and CONFIG_ID = p_CONFIG_ID;
        if v_count > 0 then
            select WIN_SERVICE_ID into v_WS_ID from CSM_WINDOWS_SERVICES 
					where ENV_ID = p_ENV_ID
							and CONFIG_ID = p_CONFIG_ID;
        else
            v_WS_ID := 0;
        end if;
        
		if v_WS_ID is null or v_WS_ID = 0 or v_WS_ID = ''
		Then
			insert into CSM_WINDOWS_SERVICES
			(
				ENV_ID
				,CONFIG_ID
				,WIN_SERVICENAME
				,WIN_COMMENTS
				,WIN_CREATED_BY
				,WIN_CREATED_DATE
			)
			values
			(
				p_ENV_ID
				,p_CONFIG_ID
				,p_SERVICENAME
				,p_COMMENTS
				,p_CREATED_BY
				,p_CREATED_DATE
			);
		End if;
        
		if v_WS_ID > 0
		Then
			Update CSM_WINDOWS_SERVICES set 
				WIN_SERVICENAME = p_SERVICENAME
				,WIN_COMMENTS = p_COMMENTS
				,WIN_CREATED_BY = p_CREATED_BY
				,WIN_CREATED_DATE = p_CREATED_DATE
				where WIN_SERVICE_ID = v_WS_ID;
				--ENV_ID = @ENV_ID and 
				--	CONFIG_ID = @CONFIG_ID
		End if;    
    END;    

    PROCEDURE SP_CWT_InsUpdServerPerfSch
                                        (
                                        p_ENVID number
                                        ,p_CONFIGID number
                                        ,p_HOSTIP varchar2
                                        ,p_PORT varchar2
                                        ,p_LASTJOBRUNTIME timestamp
                                        ,p_NEXTJOBRUNTIME timestamp
                                        ,p_MODE varchar2
                                        )
    As
    v_SVRID number(10);
    v_count number;
    BEGIN
    
        select count(*) into v_count from CSM_SERVERPERFORMANCE_SCHEDULE 
        where ENVID = p_ENVID
        and CONFIGID = p_CONFIGID;
        
        if v_count > 0 then
            select SVR_ID into v_SVRID from CSM_SERVERPERFORMANCE_SCHEDULE 
            where ENVID = p_ENVID
            and CONFIGID = p_CONFIGID;
        else
            v_SVRID := 0;
        end if;
        
        if p_MODE = 'NS' -- Next schedule
        Then
            update CSM_SERVERPERFORMANCE_SCHEDULE set
            SVR_LASTJOBRAN_TIME = p_LASTJOBRUNTIME,
            SVR_NEXTJOBRAN_TIME = p_NEXTJOBRUNTIME
            where SVR_HOSTIP = p_HOSTIP;
        End if;
        
        if (v_SVRID <= 0 or v_SVRID is null or v_SVRID = '' and p_MODE = 'IS') -- Initial schedule
        Then
            insert into CSM_SERVERPERFORMANCE_SCHEDULE 
            (
                ENVID
              ,CONFIGID
              ,SVR_HOSTIP
              ,SVR_PORT
              ,SVR_LASTJOBRAN_TIME
              ,SVR_NEXTJOBRAN_TIME
            )
            values
            (
                p_ENVID
                ,p_CONFIGID
                ,p_HOSTIP
                ,p_PORT
                ,p_LASTJOBRUNTIME
                ,p_NEXTJOBRUNTIME
            );
        End if;
    End;
    
    PROCEDURE SP_CWT_InsUpdUrlConfiguration
                                        (p_CATEGORY varchar2
                                        ,p_URL_ID number
                                        ,p_ENVID number
                                        ,p_URL_TYPE varchar2
                                        ,p_URL_ADDRESS varchar2
                                        ,p_URL_DISPLAYNAME varchar2
                                        ,p_URL_MATCHCONTENT varchar2
                                        ,p_URL_INTERVAL number
                                        ,p_URL_USERNAME varchar2
                                        ,p_URL_PASSWORD varchar2
                                        ,p_URL_STATUS number
                                        ,p_URL_CREATEDBY varchar2
                                        ,p_URL_UPDATEDBY varchar2
                                        ,p_URL_COMMENTS varchar2
                                        ) IS
    v_urlId number(10);
    v_count NUMBER;
    Begin
        select count(*) into v_count from CSM_URLCONFIGURATION where lower(URL_ADDRESS) = lower(p_URL_ADDRESS);
        
        if v_count > 0 then
            select URL_ID into v_urlId from CSM_URLCONFIGURATION where lower(URL_ADDRESS) = lower(p_URL_ADDRESS);
        else
            v_urlId := p_URL_ID;
        end if;
        
        if v_urlId > 0 and p_CATEGORY = 'modify_ed'
        Then
            update CSM_URLCONFIGURATION set 
                URL_TYPE = p_URL_TYPE
                ,URL_ADDRESS = p_URL_ADDRESS
                ,URL_DISPLAYNAME = p_URL_DISPLAYNAME
                ,URL_MATCHCONTENT = p_URL_MATCHCONTENT
                ,URL_INTERVAL = p_URL_INTERVAL
                ,URL_USERNAME = p_URL_USERNAME
                ,URL_PASSWORD = p_URL_PASSWORD
                ,URL_STATUS = p_URL_STATUS
                ,URL_UPDATEDBY = p_URL_UPDATEDBY
                ,URL_UPDATEDDATE = SYSTIMESTAMP
                ,URL_COMMENTS = p_URL_COMMENTS
            where URL_ID = p_URL_ID;
            v_urlId := p_URL_ID;
        
    
        else if p_CATEGORY = 'add_en' and (p_URL_ID = 0 or p_URL_ID is null)
        Then
            insert into CSM_URLCONFIGURATION (
                    ENV_ID
                  ,URL_TYPE
                  ,URL_ADDRESS
                  ,URL_DISPLAYNAME
                  ,URL_MATCHCONTENT
                  ,URL_INTERVAL
                  ,URL_USERNAME
                  ,URL_PASSWORD
                  ,URL_STATUS
                  ,URL_CREATEDBY
                  ,URL_COMMENTS
                  )
                  values
                  (
                      p_ENVID
                      ,p_URL_TYPE
                      ,p_URL_ADDRESS
                      ,p_URL_DISPLAYNAME
                      ,p_URL_MATCHCONTENT
                      ,p_URL_INTERVAL
                      ,p_URL_USERNAME
                      ,p_URL_PASSWORD
                      ,p_URL_STATUS
                      ,p_URL_CREATEDBY
                      ,p_URL_COMMENTS
                  ) RETURNING URL_ID into v_urlId;
        End if;
        End if;
                
    End;
    
	procedure SP_CWT_InsUpdSubscription
								  (
									p_SUBSCRIPTION_ID number
									,p_SUBSCRIPTION_TYPE varchar2 
									,p_SUBSCRIPTION_TIME varchar2 
									,p_SUBSCRIPTION_ISACTIVE number
									,p_CREATED_BY varchar2
									,p_CREATED_DATE timestamp
									,p_UPDATED_BY varchar2
									,p_UPDATED_DATE timestamp
									,p_NEXTJOBRUNTIME timestamp
									,p_SUBSCRIPTION_EMAILS varchar
									,p_SCOPE_OUTPUT out number 
								  ) 
  As
	v_Character CHAR(1)
	;v_StartIndex NUMBER(10)
	;v_EndIndex NUMBER(10)
	;v_Input varchar(4000)
	;v_tempEmailId varchar2(200)
	;v_tempUserLst number(10);
    v_p_SUBSCRIPTION_ID NUMBER;
    v_count NUMBER;
  BEGIN

  
		v_Character := ',';
		v_Input := p_SUBSCRIPTION_EMAILS;
		v_StartIndex := 1;
  
		if p_SUBSCRIPTION_ID > 0
		THEN
			Update CSM_REPORT_SUBSCRIPTION set 
				SUBSCRIPTION_TYPE = p_SUBSCRIPTION_TYPE
				,SUBSCRIPTION_TIME = p_SUBSCRIPTION_TIME
				,SUBSCRIPTION_ISACTIVE = p_SUBSCRIPTION_ISACTIVE
				,UPDATED_BY = p_UPDATED_BY
				,UPDATED_DATE = p_UPDATED_DATE
				Where SUBSCRIPTION_ID = p_SUBSCRIPTION_ID;
                
            v_p_SUBSCRIPTION_ID := p_SUBSCRIPTION_ID;
		Else
			Insert into CSM_REPORT_SUBSCRIPTION
			(
				SUBSCRIPTION_TYPE
				,SUBSCRIPTION_TIME
				,CREATED_BY
				,CREATED_DATE
				,SCH_NEXTJOBRAN_TIME
			)
			Values
			(
				p_SUBSCRIPTION_TYPE
				,p_SUBSCRIPTION_TIME
				,p_CREATED_BY
				,p_CREATED_DATE
				,p_NEXTJOBRUNTIME
			) RETURNING SUBSCRIPTION_ID INTO v_p_SUBSCRIPTION_ID;
			--p_SUBSCRIPTION_ID := IDENT_CURRENT('CSM_REPORT_SUBSCRIPTION');
		END IF;

        Select count(*) INTO v_count from CSM_REPORT_SUBS_DETAILS where SUBSCRIPTION_ID = v_p_SUBSCRIPTION_ID;
        
		if v_p_SUBSCRIPTION_ID > 0 AND	v_count >= 1
		THEN
			Delete from CSM_REPORT_SUBS_DETAILS where SUBSCRIPTION_ID = v_p_SUBSCRIPTION_ID;
		END IF;
		
		If p_SUBSCRIPTION_EMAILS is not null
		THEN
			IF SUBSTR(v_Input, LENGTH(RTRIM(v_Input)), LENGTH(RTRIM(v_Input))) <> v_Character
			THEN
				v_Input := v_Input || v_Character;
			END IF;	
			
			WHILE INSTR(v_Input, v_Character) > 0
			Loop
				v_EndIndex := INSTR(v_Input, v_Character);
				v_tempEmailId := SUBSTR(v_Input, v_StartIndex, v_EndIndex - 1);
				
				select USRLST_ID into v_tempUserLst 
					from CSM_EMAIL_USERLIST
					where USRLST_EMAIL_ADDRESS = v_tempEmailId and rownum = 1;
					
				insert into CSM_REPORT_SUBS_DETAILS
				(
				  SUBSCRIPTION_ID
				  ,USRLST_ID
				  ,SUBSCRIPTION_EMAILID
				  ,SUBSCRIPTION_ISACTIVE
				)
				Values
				(
					v_p_SUBSCRIPTION_ID
					,v_tempUserLst
					,v_tempEmailId
					,1
				);
				
				v_Input := SUBSTR(v_Input, v_EndIndex + 1, LENGTH(RTRIM(v_Input)));
			End LOOP;
		END IF;
		p_SCOPE_OUTPUT := v_p_SUBSCRIPTION_ID;
  
  END;    
                                        
END;
/


Exit;