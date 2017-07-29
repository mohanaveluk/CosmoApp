--select * from CSM_ENVIRONEMENT;

CREATE OR REPLACE PACKAGE "COSMO_MONITOR_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    FUNCTION FN_CWT_GetMonitorStatus(p_ENV_ID IN NUMBER) RETURN o_Cursor;
    FUNCTION FN_CWT_GetMonStatusWithSN_CID (p_CONFIG_ID IN NUMBER) RETURN o_Cursor;
    FUNCTION FN_CWT_GetMonStatusWithSName (p_ENV_ID IN NUMBER) RETURN o_Cursor;
    FUNCTION FN_CWT_GetAllIncident(p_ENV_ID number, p_Type varchar2) RETURN o_Cursor;
    FUNCTION FN_CWT_GetServiceAvailability(p_ENV_ID number, p_FromTime timestamp, p_ToTime timestamp, p_Type varchar2)   RETURN o_Cursor;
    FUNCTION FN_CWT_GetServiceAvailDowntime(p_ENV_ID number,	p_FromTime timestamp,	p_ToTime timestamp) RETURN o_Cursor;
    FUNCTION FN_CWT_GetCurrentBuildReport(p_ENV_ID number) RETURN o_Cursor;
    FUNCTION FN_CWT_GetUserEmailList(p_Env_ID number, p_MessageType varchar2) RETURN o_Cursor;
    FUNCTION FN_CWT_GetIncidentTracking(p_ENV_ID number, p_FromTime timestamp, p_ToTime timestamp) RETURN o_Cursor;
    FUNCTION FN_CWT_GetConfigServiceName (p_ENV_ID number) RETURN o_Cursor;
    
    FUNCTION FN_CWT_GetAverageUsedSpace(p_HOSTIP varchar2) RETURN o_Cursor;
    FUNCTION FN_CWT_GetCpuMemorySpace(p_HOSTIP varchar2) RETURN o_Cursor;
    FUNCTION FN_CWT_GetServerPerfSchedule(p_ENVID number) RETURN o_Cursor;
    
    PROCEDURE SP_CWT_SetServiceAcknowledge (
                                            p_ENV_ID number,
                                            p_CONFIG_ID number,
                                            p_MON_ID number,
                                            p_ACK_ISACKNOWLEDGE number,
                                            p_ACK_ALERT varchar2,
                                            p_ACK_COMMENTS varchar2,
                                            p_CREATED_BY varchar2,
                                            p_CREATED_DATE timestamp
                                        );
    
    procedure SP_CWT_SetMonitorStatus( p_SCH_ID number
                                    ,p_CONFIG_ID number
                                    ,p_ENV_ID number
                                    ,p_MON_STATUS varchar2
                                    ,p_MON_START_DATE_TIME varchar2
                                    ,p_MON_END_DATE_TIME varchar2
                                    ,p_MON_IS_ACTIVE number
                                    ,p_MON_CREATED_BY varchar2
                                    ,p_MON_CREATED_DATE timestamp
                                    ,p_MON_COMMENTS varchar2
                                  );
    
    PROCEDURE SP_CWT_InsertMailLog(       p_ENV_ID number
                                      ,p_CONFIG_ID number
                                      ,p_EMTRAC_TO_ADDRESS varchar2
                                      ,p_EMTRAC_CC_ADDRESS varchar2
                                      ,p_EMTRAC_BCC_ADDRESS varchar2
                                      ,p_EMTRAC_SUBJECT varchar2
                                      ,p_EMTRAC_BODY varchar2
                                      ,p_EMTRAC_SEND_STATUS varchar2
                                      ,p_EMTRAC_SEND_ERROR varchar2
                                      ,p_EMTRAC_CONTENT_TYPE varchar2
                                      ,p_EMTRAC_CREATED_BY varchar2
                                      ,p_EMTRAC_CREATED_DATE timestamp
                                      ,p_EMTRAC_COMMENTS varchar2
                                );

    procedure SP_CWT_InsertCSMLog(
                                p_SCH_ID number,
                                p_CONFIG_ID number,
                                p_ENV_ID number ,
                                p_LOGDESCRIPTION varchar2,
                                p_LOGERROR varchar2,
                                p_LOG_UPDATE_TATETIME timestamp,
                                p_LOG_UPDATED_BY varchar2
                              );
    procedure SP_CWT_InsIncidentTracking(  p_MON_ID number
                                          ,p_ENV_ID number
                                          ,p_CONFIG_ID number
                                          ,p_TRK_ISSUE varchar
                                          ,p_TRK_SOLUTION varchar
                                          ,p_TRK_CREATED_BY varchar2
                                          ,p_TRK_CREATED_DATE timestamp
                                          ,p_TRK_COMMENTS varchar
                                        );
          
    PROCEDURE SP_CWT_InsUpdPersonalize( p_PERS_ID number
                                        ,p_USER_ID number
                                        ,p_PERS_DB_REFRESHTIME number
                                        ,p_PERS_ISACTIVE number
                                        ,p_PERS_CREATEDDATE timestamp
                                        ,p_PERS_CREATEDBY varchar2
                                        ,p_PERS_SORTORDER varchar2);
    
    PROCEDURE SP_CWT_InsUpdServerPerformance
                                      (
                                            p_ENVID number
                                          ,p_CONFIGID number
                                          ,p_PER_HOSTIP varchar2
                                          ,p_PER_CPU_USAGE binary_double
                                          ,p_PER_AVAILABLEMEMORY binary_double
                                          ,p_PER_TOTALMEMORY binary_double
                                          ,p_PER_CREATED_BY varchar2
                                          ,p_PER_COMMENTS varchar
                                          ,p_SCOPE_OUTPUT out number 
                                      );
    
    PROCEDURE SP_CWT_InsUpdServerPerfDrive
                                          (    p_PER_ID number
                                              ,p_DRIVE_NAME varchar2
                                              ,p_DRIVE_LABEL varchar2
                                              ,p_DRIVE_FORMAT varchar2
                                              ,p_DRIVE_TYPE varchar2
                                              ,p_DRIVE_FREESPACE binary_double
                                              ,p_DRIVE_USEDSPACE binary_double
                                              ,p_DRIVE_TOTALSPACE binary_double
                                              ,p_DRIVE_COMMENTS varchar
                                          );

    PROCEDURE SP_CWT_InsUpdServerPerfSch
                                      (
                                        p_ENVID number
                                        ,p_CONFIGID number
                                        ,p_HOSTIP varchar2
                                        ,p_PORT varchar2
                                        ,p_LASTJOBRUNTIME timestamp
                                        ,p_NEXTJOBRUNTIME timestamp
                                        ,p_MODE varchar2
                                      );                                          
                                          
END COSMO_MONITOR_PACKAGE;
/


CREATE OR REPLACE PACKAGE BODY "COSMO_MONITOR_PACKAGE" AS

    FUNCTION FN_CWT_GetMonitorStatus(p_ENV_ID IN NUMBER) RETURN o_Cursor IS
    CUR o_Cursor;
    v_BuildCount number(10);
    BEGIN
        select count(*) into v_BuildCount from CSM_SERVICEBUILD where ENV_ID = p_ENV_ID;
        if v_BuildCount > 0
        Then
            open CUR for select distinct
                con.ENV_ID,
                con.CONFIG_ID,
                con.CONFIG_HOST_IP,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_DESCRIPTION,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_URL_ADDRESS,
                con.CONFIG_IS_MONITORED,
                con.CONFIG_ISNOTIFY,
                con.CONFIG_MAIL_FREQ,
                con.CONFIG_LOCATION,
                con.CONFIG_ISPRIMARY,
                mon.MON_ID,
                mon.SCH_ID,
                mon.MON_STATUS,
                mon.MON_COMMENTS,
                mon.MON_CREATED_DATE,
                mon.MON_UPDATED_DATE,
                mon.MON_START_DATE_TIME,
                mon.MON_END_DATE_TIME,
                mon.MON_IS_ACTIVE,
                mon.MON_ISACKNOWLEDGE
                ,case when mon.MON_STATUS = 'Running' or mon.MON_STATUS = 'Standby' then
                to_char(trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) || 'd' || ', ' ||
                to_char((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) * 24) || 'h' || ', ' ||
                --to_char((((((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - trunc(MON_UPDATED_DATE - trunc(MON_CREATED_DATE))) * 24) - 
                --          (((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE))) * 24)) * 60)) || 'm'
                to_char(((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) * 24) - 
                ((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) * 24) * 60) || 'm'
                --to_char(trunc(MON_UPDATED_DATE - MON_CREATED_DATE))
                else '0d, 0h, 0m'
                end
                as MON_UPTIME
              ,(select BUILD_VERSION from CSM_SERVICEBUILD T1
                    inner join		
                    (
                        select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
                        --where CONFIG_ID in (select CONFIG_ID from  CSM_CONFIGURATION)
                        group by env_id, config_id
                    ) T2
                    on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
                    where T1.CONFIG_ID = con.CONFIG_ID
                    ) BUILD_VERSION
              ,(select CREATED_DATE from CSM_SERVICEBUILD T1
                    inner join		
                    (
                        select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
                        --where CONFIG_ID  in (select CONFIG_ID from  CSM_CONFIGURATION)
                        group by env_id, config_id
                    ) T2
                    on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
                    where T1.CONFIG_ID = con.CONFIG_ID
                    )CREATED_DATE
            from CSM_CONFIGURATION con
            left join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
            --left join [dbo].[CSM_SERVICEBUILD] sb on sb.CONFIG_ID = con.CONFIG_ID
            where con.ENV_ID = p_ENV_ID  
                and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID union Select null FROM dual) or mon.MON_ID is null)
                and con.CONFIG_IS_ACTIVE = 1
                --and (sb.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [dbo].[CSM_SERVICEBUILD] where ENV_ID = con.ENV_ID group by CONFIG_ID) ) 
            order by CONFIG_SERVICE_TYPE, CONFIG_HOST_IP, CONFIG_PORT_NUMBER;
        Else
            open CUR for select distinct
                con.ENV_ID,
                con.CONFIG_ID,
                con.CONFIG_HOST_IP,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_DESCRIPTION,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_URL_ADDRESS,
                con.CONFIG_IS_MONITORED,
                con.CONFIG_ISNOTIFY,
                con.CONFIG_MAIL_FREQ,
                con.CONFIG_LOCATION,
                con.CONFIG_ISPRIMARY,
                mon.MON_ID,
                mon.SCH_ID,
                mon.MON_STATUS,
                mon.MON_COMMENTS,
                mon.MON_CREATED_DATE,
                mon.MON_UPDATED_DATE,
                mon.MON_START_DATE_TIME,
                mon.MON_END_DATE_TIME,
                mon.MON_IS_ACTIVE,
                mon.MON_ISACKNOWLEDGE
                ,case when mon.MON_STATUS = 'Running' or mon.MON_STATUS = 'Standby' then
                to_char(trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) || 'd' || ', ' ||
                to_char((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) * 24) || 'h' || ', ' ||
                --to_char((((((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - trunc(MON_UPDATED_DATE - trunc(MON_CREATED_DATE))) * 24) - 
                --          (((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE))) * 24)) * 60)) || 'm'
                to_char(((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) * 24) - 
                ((trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) - (trunc(MON_UPDATED_DATE) - trunc(MON_CREATED_DATE)) * 24) * 60) || 'm'
                --to_char(trunc(MON_UPDATED_DATE - MON_CREATED_DATE))
                else '0d, 0h, 0m'
                end
                as MON_UPTIME            
              ,(select BUILD_VERSION from CSM_SERVICEBUILD T1
                    inner join		
                    (
                        select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
                        --where CONFIG_ID in (select CONFIG_ID from  CSM_CONFIGURATION)
                        group by env_id, config_id
                    ) T2
                    on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
                    where T1.CONFIG_ID = con.CONFIG_ID
                    ) BUILD_VERSION
              ,(select CREATED_DATE from CSM_SERVICEBUILD T1
                    inner join		
                    (
                        select env_id, config_id, MAX(created_date) maxdate from CSM_SERVICEBUILD
                        --where CONFIG_ID in (select CONFIG_ID from  CSM_CONFIGURATION)
                        group by env_id, config_id
                    ) T2
                    on T1.CONFIG_ID = T2.CONFIG_ID and T1.CREATED_DATE = T2.maxdate
                    where T1.CONFIG_ID = con.CONFIG_ID
                    )CREATED_DATE
            from CSM_CONFIGURATION con
            left join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
            --left join [dbo].[CSM_SERVICEBUILD] sb on sb.CONFIG_ID = con.CONFIG_ID
            where con.ENV_ID = p_ENV_ID  
                and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID union Select null FROM dual)  or mon.MON_ID is null)
                and con.CONFIG_IS_ACTIVE = 1
                --and (sb.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [dbo].[CSM_SERVICEBUILD] where ENV_ID = con.ENV_ID group by CONFIG_ID) ) 
            order by CONFIG_SERVICE_TYPE, CONFIG_HOST_IP, CONFIG_PORT_NUMBER;
        End if;    
		return CUR;
    END;
    
    FUNCTION FN_CWT_GetMonStatusWithSN_CID (p_CONFIG_ID IN NUMBER) RETURN o_Cursor IS
    CUR o_Cursor;
    BEGIN
        open CUR for select 
            con.ENV_ID,
            con.CONFIG_ID,
            con.CONFIG_HOST_IP,
            con.CONFIG_PORT_NUMBER,
            con.CONFIG_DESCRIPTION,
            con.CONFIG_SERVICE_TYPE,
            con.CONFIG_URL_ADDRESS,
            con.CONFIG_IS_MONITORED,
            con.CONFIG_ISNOTIFY,
            con.CONFIG_MAIL_FREQ,
            con.CONFIG_LOCATION,
            mon.MON_ID,
            mon.SCH_ID,
            mon.MON_STATUS,
            mon.MON_COMMENTS,
            mon.MON_CREATED_DATE,
            mon.MON_UPDATED_DATE,
            mon.MON_START_DATE_TIME,
            mon.MON_END_DATE_TIME,
            mon.MON_IS_ACTIVE,
            mon.MON_ISACKNOWLEDGE,
            win.WIN_SERVICENAME,
            win.WIN_SERVICE_ID
        from CSM_CONFIGURATION con
        left outer join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
        left outer join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
        where con.CONFIG_ID = p_CONFIG_ID
            and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID, SCH_ID) or mon.MON_ID is null)
            and con.CONFIG_IS_ACTIVE = 1
            and con.CONFIG_ISPRIMARY = 1;    
		return CUR;
    END;
    
    FUNCTION FN_CWT_GetMonStatusWithSName (p_ENV_ID IN NUMBER) RETURN o_Cursor IS
    CUR o_Cursor;
    BEGIN
        open CUR for select 
            con.ENV_ID,
            con.CONFIG_ID,
            con.CONFIG_HOST_IP,
            con.CONFIG_PORT_NUMBER,
            con.CONFIG_DESCRIPTION,
            con.CONFIG_SERVICE_TYPE,
            con.CONFIG_URL_ADDRESS,
            con.CONFIG_IS_MONITORED,
            con.CONFIG_ISNOTIFY,
            con.CONFIG_MAIL_FREQ,
            con.CONFIG_LOCATION
            ,mon.MON_ID
            ,mon.SCH_ID
            ,mon.MON_STATUS
            ,mon.MON_COMMENTS
            ,mon.MON_CREATED_DATE
            ,mon.MON_UPDATED_DATE
            ,mon.MON_START_DATE_TIME
            ,mon.MON_END_DATE_TIME
            ,mon.MON_IS_ACTIVE
            ,mon.MON_ISACKNOWLEDGE
            ,win.WIN_SERVICENAME
            ,win.WIN_SERVICE_ID
        from CSM_CONFIGURATION con
        left outer join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
        left outer join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
        where con.ENV_ID = p_ENV_ID  
            and (mon.MON_ID in (select max(mon_id) MON_ID from CSM_MONITOR group by ENV_ID, CONFIG_ID) or mon.MON_ID is null)
            and con.CONFIG_IS_ACTIVE = 1
            and con.CONFIG_ISPRIMARY = 1
        order by CONFIG_SERVICE_TYPE, CONFIG_HOST_IP, CONFIG_PORT_NUMBER;    
		
		return CUR;
    END;
    
    FUNCTION FN_CWT_GetAllIncident(p_ENV_ID number, p_Type varchar2) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if(p_ENV_ID is not null and p_ENV_ID <= 0)
        Then
            OPEN CUR FOR SELECT  con.CONFIG_ID
                  ,con.ENV_ID
                  ,CONFIG_SERVICE_TYPE
                  ,CONFIG_PORT_NUMBER
                  ,CONFIG_DESCRIPTION
                  ,CONFIG_HOST_IP
                  ,CONFIG_LOCATION
                  ,CONFIG_ISPRIMARY
                  ,MON_ID
                  ,MON_STATUS
                  ,MON_CREATED_DATE
              FROM CSM_CONFIGURATION con
              inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
              Where CONFIG_ISPRIMARY = 1
                    And lower(MON_STATUS) not in ('running','standby')
                    And MON_ID not in (select MON_ID from CSM_INCIDENT);
    
            End if;
        if(p_ENV_ID is not null and p_ENV_ID > 0)
        Then
            if(lower(p_Type)='pnd')
            Then
                OPEN CUR FOR SELECT  con.CONFIG_ID
                      ,con.ENV_ID
                      ,CONFIG_SERVICE_TYPE
                      ,CONFIG_PORT_NUMBER
                      ,CONFIG_DESCRIPTION
                      ,CONFIG_HOST_IP
                      ,CONFIG_LOCATION
                      ,CONFIG_ISPRIMARY
                      ,MON_ID
                      ,MON_STATUS
                      ,MON_CREATED_DATE
                  FROM CSM_CONFIGURATION con
                  inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
                  Where CONFIG_ISPRIMARY = 1
                        And lower(MON_STATUS) not in ('running','standby')
                        And MON_ID not in (select MON_ID from CSM_INCIDENT)	
                        And mon.ENV_ID = p_ENV_ID;
            else if(lower(p_Type)='all')
            Then
                OPEN CUR FOR SELECT  con.CONFIG_ID
                      ,con.ENV_ID
                      ,CONFIG_SERVICE_TYPE
                      ,CONFIG_PORT_NUMBER
                      ,CONFIG_DESCRIPTION
                      ,CONFIG_HOST_IP
                      ,CONFIG_LOCATION
                      ,CONFIG_ISPRIMARY
                      ,MON_ID
                      ,MON_STATUS
                      ,MON_CREATED_DATE
                  FROM CSM_CONFIGURATION con
                  inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
                  Where CONFIG_ISPRIMARY = 1
                        And lower(MON_STATUS) not in ('running','standby')
                        And mon.ENV_ID = p_ENV_ID;
            End if;
            end if;
        End if;    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetServiceAvailability(p_ENV_ID number, p_FromTime timestamp, p_ToTime timestamp, p_Type varchar2)   RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if(p_ENV_ID = 0)
        then
            open CUR for select 
                con.ENV_ID,
                con.CONFIG_ID,
                con.CONFIG_HOST_IP,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_URL_ADDRESS,
                con.CONFIG_IS_MONITORED,
                con.CONFIG_ISNOTIFY,
                con.CONFIG_MAIL_FREQ,
                con.CONFIG_LOCATION,
                mon.MON_ID,
                mon.SCH_ID,
                mon.MON_STATUS,
                mon.MON_COMMENTS,
                mon.MON_CREATED_DATE,
                mon.MON_UPDATED_DATE,
                mon.MON_START_DATE_TIME,
                mon.MON_END_DATE_TIME,
                mon.MON_IS_ACTIVE
                from CSM_CONFIGURATION con
                inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
                inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = mon.ENV_ID
                where (MON_UPDATED_DATE between p_FromTime and p_ToTime 
                or p_ToTime <= MON_UPDATED_DATE)
                and ce.ENV_ISACTIVE = 1
                and con.CONFIG_IS_ACTIVE = 1
                order by mon.ENV_ID, MON_UPDATED_DATE;
        else
            if(p_Type='all')
            Then
            open CUR for select 
                con.ENV_ID,
                con.CONFIG_ID,
                con.CONFIG_HOST_IP,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_URL_ADDRESS,
                con.CONFIG_IS_MONITORED,
                con.CONFIG_ISNOTIFY,
                con.CONFIG_MAIL_FREQ,
                con.CONFIG_LOCATION,
                mon.MON_ID,
                mon.SCH_ID,
                mon.MON_STATUS,
                mon.MON_COMMENTS,
                mon.MON_CREATED_DATE,
                mon.MON_UPDATED_DATE,
                mon.MON_START_DATE_TIME,
                mon.MON_END_DATE_TIME,
                mon.MON_IS_ACTIVE
                from CSM_CONFIGURATION con
                inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
                inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = mon.ENV_ID
                where (MON_UPDATED_DATE between p_FromTime and p_ToTime
                 or p_ToTime <= MON_UPDATED_DATE)
                and ce.ENV_ISACTIVE = 1
                and con.CONFIG_IS_ACTIVE = 1
                and mon.ENV_ID = p_ENV_ID
                order by mon.ENV_ID, MON_UPDATED_DATE;
            else if(p_Type='env')
            Then
            open CUR for select 
                con.ENV_ID,
                con.CONFIG_ID,
                con.CONFIG_HOST_IP,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_URL_ADDRESS,
                con.CONFIG_IS_MONITORED,
                con.CONFIG_ISNOTIFY,
                con.CONFIG_MAIL_FREQ,
                con.CONFIG_LOCATION,
                mon.MON_ID,
                mon.SCH_ID,
                mon.MON_STATUS,
                mon.MON_COMMENTS,
                mon.MON_CREATED_DATE,
                mon.MON_UPDATED_DATE,
                mon.MON_START_DATE_TIME,
                mon.MON_END_DATE_TIME,
                mon.MON_IS_ACTIVE
                from CSM_CONFIGURATION con
                inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
                inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = mon.ENV_ID
                where 
                ((MON_CREATED_DATE >= p_FromTime and MON_UPDATED_DATE <= p_ToTime)
                or (MON_CREATED_DATE between p_FromTime and p_ToTime))
                and ce.ENV_ISACTIVE = 1
                and con.CONFIG_IS_ACTIVE = 1
                and mon.ENV_ID = p_ENV_ID
                order by mon.CONFIG_ID, con.CONFIG_SERVICE_TYPE, MON_UPDATED_DATE;
            End if;
            end if;
        End if;    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetServiceAvailDowntime(p_ENV_ID number,	p_FromTime timestamp,	p_ToTime timestamp) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        open CUR for select MON_ID,
		CONFIG_ID, 
		ENV_ID, 
		MON_STATUS, 
		MON_CREATED_DATE, 
		MON_UPDATED_DATE
		--cast(datediff(MI,@FromTime,@ToTime) as decimal(10,3)) as input,
		--cast(datediff(MI,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3)) as minutediff
		--sum(cast(datediff(MI,MON_CREATED_DATE,MON_UPDATED_DATE) as decimal(10,3))) as minutediff

			from CSM_MONITOR mon
			where (MON_UPDATED_DATE >= p_FromTime and MON_UPDATED_DATE <= p_ToTime  
			or (MON_CREATED_DATE <=  p_ToTime AND MON_CREATED_DATE >= p_FromTime ))
			and lower(MON_STATUS) != 'running' AND lower(MON_STATUS) != 'standby'
			and mon.ENV_ID = p_ENV_ID
			order by mon.CONFIG_ID, MON_UPDATED_DATE;                
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetCurrentBuildReport(p_ENV_ID number) RETURN o_Cursor IS
    CUR o_Cursor; 
    v_BuildCount number(10);
    Begin
        select count(*) into v_BuildCount from CSM_SERVICEBUILD where ENV_ID = p_ENV_ID;
        if v_BuildCount > 0
        Then
            open CUR for select distinct
                mon.CONFIG_ID,
                mon.BUILD_VERSION MON_COMMENTS,
                mon.BUILD_DATE MON_BUILD_DATE,
                TO_CHAR(mon.CREATED_DATE,'mm/dd/yyyy HH24:MI:SS') MON_CREATED_DATE,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_DESCRIPTION,
                lower(con.CONFIG_HOST_IP) CONFIG_HOST_IP,
                con.CONFIG_LOCATION,
                con.CONFIG_ISPRIMARY
            from CSM_SERVICEBUILD mon
            inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
            where con.ENV_ID = p_ENV_ID
            and mon.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from CSM_SERVICEBUILD group by CONFIG_ID)
            --and con.CONFIG_ISPRIMARY = '1'
            order by MON_CREATED_DATE, CONFIG_ID;
        Else
            open CUR for select distinct
                mon.CONFIG_ID,
                mon.BUILD_VERSION MON_COMMENTS,
                mon.BUILD_DATE MON_BUILD_DATE,
                TO_CHAR(mon.CREATED_DATE,'mm/dd/yyyy HH24:MI:SS') MON_CREATED_DATE,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_DESCRIPTION,
                lower(con.CONFIG_HOST_IP) CONFIG_HOST_IP,
                con.CONFIG_LOCATION,
                con.CONFIG_ISPRIMARY
            from CSM_SERVICEBUILD mon
            inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
            where con.ENV_ID = p_ENV_ID
            --and con.CONFIG_ISPRIMARY = '1';
            order by MON_CREATED_DATE, CONFIG_ID;	
        End if;
    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetUserEmailList(p_Env_ID number, p_MessageType varchar2) RETURN o_Cursor IS
    CUR o_Cursor; 
    Begin
        OPEN cur FOR SELECT  USRLST_ID
          ,ENV_ID
          ,USRLST_EMAIL_ADDRESS
          ,USRLST_TYPE
          ,USRLST_IS_ACTIVE
          ,USRLST_CREATED_BY
          ,USRLST_CREATED_DATE
          ,USRLST_UPDATED_BY
          ,USRLST_UPDATED_DATE
          ,USRLST_COMMENTS
          FROM CSM_EMAIL_USERLIST
          Where ENV_ID = p_Env_ID
            AND USRLST_MESSAGETYPE = p_MessageType
            AND USRLST_IS_ACTIVE = 1;
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetIncidentTracking(p_ENV_ID number, p_FromTime timestamp, p_ToTime timestamp) RETURN o_Cursor IS
    CUR o_Cursor; 
    Begin
		if(p_ENV_ID is not null and p_ENV_ID <= 0)
		Then
			OPEN CUR FOR SELECT  con.CONFIG_ID
					,con.ENV_ID
					,CONFIG_SERVICE_TYPE
					,CONFIG_PORT_NUMBER
					,CONFIG_DESCRIPTION
					,CONFIG_HOST_IP
					,CONFIG_LOCATION
					,CONFIG_ISPRIMARY
					,mon.MON_ID
					,MON_STATUS
					,MON_CREATED_DATE
					,inc.TRK_ISSUE
					,inc.TRK_SOLUTION
					,inc.TRK_ID
					,inc.TRK_CREATED_DATE
					,inc.TRK_CREATED_BY
				FROM CSM_CONFIGURATION con
				inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
				inner join CSM_INCIDENT inc on inc.MON_ID = mon.MON_ID
				Where CONFIG_ISPRIMARY = 1
				And (MON_CREATED_DATE between 
                TO_TIMESTAMP(to_char(p_FromTime,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
                and
                TO_TIMESTAMP(to_char(p_ToTime,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS'));
			End if;
		if(p_ENV_ID is not null and p_ENV_ID > 0)
		Then
			OPEN CUR FOR SELECT  con.CONFIG_ID
					,con.ENV_ID
					,CONFIG_SERVICE_TYPE
					,CONFIG_PORT_NUMBER
					,CONFIG_DESCRIPTION
					,CONFIG_HOST_IP
					,CONFIG_LOCATION
					,CONFIG_ISPRIMARY
					,mon.MON_ID
					,MON_STATUS
					,MON_CREATED_DATE
					,inc.TRK_ISSUE
					,inc.TRK_SOLUTION
					,inc.TRK_ID
					,inc.TRK_CREATED_DATE
					,inc.TRK_CREATED_BY
				FROM CSM_CONFIGURATION con
				inner join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
				inner join CSM_INCIDENT inc on inc.MON_ID = mon.MON_ID
				Where CONFIG_ISPRIMARY = 1
					And (MON_CREATED_DATE between 
                    TO_TIMESTAMP(to_char(p_FromTime,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
                    and
                    TO_TIMESTAMP(to_char(p_ToTime,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS'))
					And mon.ENV_ID = p_ENV_ID;
		End if;    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetConfigServiceName (p_ENV_ID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        open cur for select 
            con.ENV_ID,
            con.CONFIG_ID,
            con.CONFIG_HOST_IP,
            con.CONFIG_PORT_NUMBER,
            con.CONFIG_SERVICE_TYPE,
            con.CONFIG_URL_ADDRESS,
            con.CONFIG_IS_MONITORED,
            con.CONFIG_ISNOTIFY,
            con.CONFIG_MAIL_FREQ,
            con.CONFIG_LOCATION,
            win.WIN_SERVICENAME,
            win.WIN_SERVICE_ID
        from CSM_CONFIGURATION con
        left outer join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
        where con.ENV_ID = p_ENV_ID  
            and con.CONFIG_IS_ACTIVE = 1
            and con.CONFIG_ISPRIMARY = 1
        order by CONFIG_SERVICE_TYPE;        
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetAverageUsedSpace(p_HOSTIP varchar2) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
	  OPEN cur FOR SELECT pm.DRIVE_NAME, avg(pm.DRIVE_FREESPACE) AVGFREESPACE, 
	  avg(pm.DRIVE_USEDSPACE) AVGUSEDSPACE,
	  avg(pm.DRIVE_TOTALSPACE) AVGTOTALSPACE,
	  TO_NUMBER(TO_CHAR(pm.DRIVE_CREATED_DATE, 'DD')) DATERT,
	  TO_NUMBER(TO_CHAR(pm.DRIVE_CREATED_DATE, 'MM')) MONTHRT
		  from CSM_SERVERPERFORMANCE_DRIVE pm
		  where pm.DRIVE_USEDSPACE > 0
		  and pm.DRIVE_CREATED_DATE >= (interval '-30' day) +SYSTIMESTAMP
		  and  pm.DRIVE_CREATED_DATE <= SYSTIMESTAMP
		  and pm.PER_ID in (Select PER_ID from CSM_SERVERPERFORMANCE 
		  where  PER_HOSTIP = p_HOSTIP)
		  GROUP BY pm.DRIVE_NAME, TO_NUMBER(TO_CHAR(pm.DRIVE_CREATED_DATE, 'DD')), TO_NUMBER(TO_CHAR(pm.DRIVE_CREATED_DATE, 'MM'))
		  order by MONTHRT, DATERT;	
          
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetCpuMemorySpace(p_HOSTIP varchar2) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
		OPEN cur FOR SELECT avg(pm.PER_CPU_USAGE) AVGCPUUSAGE, 
		avg(pm.PER_AVAILABLEMEMORY) AVGAVAILABLEMEMORY,
		avg(pm.PER_TOTALMEMORY) TOTALMEMORY,
		TO_NUMBER(TO_CHAR(pm.PER_CREATED_DATE, 'hh')) HOURRT, 
		TO_NUMBER(TO_CHAR(pm.PER_CREATED_DATE, 'DD')) DATERT
		  from CSM_SERVERPERFORMANCE pm
		  where pm.PER_CPU_USAGE > 0
		  and pm.PER_CREATED_DATE >= (interval '-23' hour) + SYSTIMESTAMP
		  and  pm.PER_CREATED_DATE <= SYSTIMESTAMP
		  --and pm.PER_HOSTIP = @HOSTIP
		  GROUP BY TO_NUMBER(TO_CHAR(pm.PER_CREATED_DATE, 'hh')),TO_NUMBER(TO_CHAR(pm.PER_CREATED_DATE, 'DD'))
		  		  order by DATERT, HOURRT  ;    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetServerPerfSchedule(p_ENVID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
		if p_ENVID > 0
		Then
			open CUR for select sp.SVR_ID
			  ,sp.ENVID
			  ,env.ENV_NAME
			  ,sp.CONFIGID
			  ,con.CONFIG_HOST_IP
			  ,con.CONFIG_PORT_NUMBER
			  ,sp.SVR_LASTJOBRAN_TIME
			  ,sp.SVR_NEXTJOBRAN_TIME
		  From CSM_SERVERPERFORMANCE_SCHEDULE  sp
		  Join CSM_ENVIRONEMENT env
		  On env.ENV_ID = sp.ENVID
		  join CSM_CONFIGURATION con
		  On con.CONFIG_ID = sp.CONFIGID
		  Where sp.ENVID = p_ENVID
			and con.CONFIG_ISPRIMARY = 1
			and con.CONFIG_ISNOTIFY = 1;
		Else
			open CUR for select sp.SVR_ID
			  ,sp.ENVID
			  ,env.ENV_NAME
			  ,sp.CONFIGID
			  ,con.CONFIG_HOST_IP
			  ,con.CONFIG_PORT_NUMBER
			  ,sp.SVR_LASTJOBRAN_TIME
			  ,sp.SVR_NEXTJOBRAN_TIME
		  From CSM_SERVERPERFORMANCE_SCHEDULE  sp
		  Join CSM_ENVIRONEMENT env
		  On env.ENV_ID = sp.ENVID
		  join CSM_CONFIGURATION con
		  On con.CONFIG_ID = sp.CONFIGID
		  Where con.CONFIG_ISPRIMARY = 1
			and con.CONFIG_ISNOTIFY = 1;
		End if;
        
        RETURN CUR;
    End;
    
    PROCEDURE SP_CWT_SetServiceAcknowledge (
                                            p_ENV_ID number,
                                            p_CONFIG_ID number,
                                            p_MON_ID number,
                                            p_ACK_ISACKNOWLEDGE number,
                                            p_ACK_ALERT varchar2,
                                            p_ACK_COMMENTS varchar2,
                                            p_CREATED_BY varchar2,
                                            p_CREATED_DATE timestamp
                                        ) IS
    	v_SCH_ID number;
        v_ENDDATETIME varchar2(50);
        v_CONFIG_DISP_ID number;
        v_comments varchar2(1000);
        v_count NUMBER;
    Begin
        select count(*) into v_count from CSM_CONFIGURATION where CONFIG_REF_ID = p_CONFIG_ID and CONFIG_IS_ACTIVE = 1;
        if v_count > 0 then
            select CONFIG_ID into v_CONFIG_DISP_ID from CSM_CONFIGURATION where CONFIG_REF_ID = p_CONFIG_ID and CONFIG_IS_ACTIVE = 1;
            insert into CSM_ACKNOWLEDGE 
            (
               ENV_ID
              ,CONFIG_ID
              ,ACK_ISACKNOWLEDGE
              ,ACK_ALERT
              ,ACK_COMMENTS
              ,CREATED_BY
              ,CREATED_DATE
            )
            values
            (
                p_ENV_ID,
                p_CONFIG_ID ,
                p_ACK_ISACKNOWLEDGE,
                p_ACK_ALERT ,
                p_ACK_COMMENTS ,
                p_CREATED_BY ,
                p_CREATED_DATE 
            );
            
            if v_CONFIG_DISP_ID > 0
            Then
                insert into CSM_ACKNOWLEDGE 
                (
                   ENV_ID
                  ,CONFIG_ID
                  ,ACK_ISACKNOWLEDGE
                  ,ACK_ALERT
                  ,ACK_COMMENTS
                  ,CREATED_BY
                  ,CREATED_DATE
                )
                values
                (
                    p_ENV_ID,
                    v_CONFIG_DISP_ID ,
                    p_ACK_ISACKNOWLEDGE,
                    p_ACK_ALERT ,
                    p_ACK_COMMENTS ,
                    p_CREATED_BY ,
                    p_CREATED_DATE 
                );
            End if;
          
            if LOWER(p_ACK_ALERT) = 'stop'
            Then
                Update CSM_CONFIGURATION set CONFIG_ISNOTIFY = 0, CONFIG_IS_MONITORED = 0
                    where CONFIG_ID = p_CONFIG_ID or CONFIG_REF_ID = p_CONFIG_ID;
            else if LOWER(p_ACK_ALERT) = 'start'
            Then
                Update CSM_CONFIGURATION set CONFIG_ISNOTIFY = 1, CONFIG_IS_MONITORED = 1 
                    where CONFIG_ID = p_CONFIG_ID or CONFIG_REF_ID = p_CONFIG_ID;
            End if;
              end if;
            
            if LOWER(p_ACK_COMMENTS) =  'scheduled service operation' OR LOWER(p_ACK_COMMENTS) =  'ondemand service operation' 
            Then
                select SCH_ID into v_SCH_ID from CSM_SCHEDULE where ENV_ID = p_ENV_ID and CONGIG_ID = p_CONFIG_ID;
                select to_char(systimestamp, 'Day') || ', ' || to_char(systimestamp, 'Mon DD, YYYY, HH24:MI:SS') into v_ENDDATETIME from dual;
                select CONFIG_ID into v_CONFIG_DISP_ID from CSM_CONFIGURATION where CONFIG_REF_ID = p_CONFIG_ID and CONFIG_IS_ACTIVE = 1;
                
                v_comments := 'Requested to Stop service through ' || p_ACK_COMMENTS;
                
                if LOWER(p_ACK_ALERT) = 'stop'
                Then
                    --Update CSM_CONFIGURATION set CONFIG_IS_MONITORED = 'False' where CONFIG_ID = @CONFIG_ID or CONFIG_REF_ID = @CONFIG_ID
                    
                    SP_CWT_SetMonitorStatus(
                                v_SCH_ID, 
                                p_CONFIG_ID, 
                                p_ENV_ID,
                                'Stopped', 
                                '', 
                                v_ENDDATETIME, 
                                1, 
                                p_CREATED_BY, 
                                p_CREATED_DATE, 
                                v_comments);
                                
                        if v_CONFIG_DISP_ID > 0
                        Then
                            select SCH_ID into v_SCH_ID from CSM_SCHEDULE where ENV_ID = p_ENV_ID and CONGIG_ID = v_CONFIG_DISP_ID;
                            
                            SP_CWT_SetMonitorStatus( 
                                        v_SCH_ID, 
                                        v_CONFIG_DISP_ID, 
                                        p_ENV_ID,
                                        'Stopped', 
                                        '', 
                                        v_ENDDATETIME, 
                                        1, 
                                        p_CREATED_BY, 
                                        p_CREATED_DATE, 
                                        v_comments);
                        End if;
    
                End if;
                --if LOWER(@ACK_ALERT) = 'start'
                --Begin
                    --Update CSM_CONFIGURATION set CONFIG_IS_MONITORED = 1 where CONFIG_ID = @CONFIG_ID or CONFIG_REF_ID = @CONFIG_ID
                --End
            End if;
            if p_MON_ID > 0
            Then
                update CSM_MONITOR set MON_ISACKNOWLEDGE = 1  where MON_ID = p_MON_ID ;
                update CSM_MONITOR set MON_ISACKNOWLEDGE = 1  where MON_ID in 
                    (select MON_ID from CSM_MONITOR where CONFIG_ID = v_CONFIG_DISP_ID and MON_ID > p_MON_ID);
            End if;
        End if;    
    End;

    procedure SP_CWT_SetMonitorStatus( p_SCH_ID number
                                    ,p_CONFIG_ID number
                                    ,p_ENV_ID number
                                    ,p_MON_STATUS varchar2
                                    ,p_MON_START_DATE_TIME varchar2
                                    ,p_MON_END_DATE_TIME varchar2
                                    ,p_MON_IS_ACTIVE number
                                    ,p_MON_CREATED_BY varchar2
                                    ,p_MON_CREATED_DATE timestamp
                                    ,p_MON_COMMENTS varchar2
                                  ) IS
        v_tempMonitorID number(10);
		v_MonitorID number(10);
		v_MonitorStatus varchar2(50);
		v_UpdateStatus number(1);
		v_tempDailyStatusCount number(10);
	Begin
        select max(mon_id) into v_MonitorID from CSM_MONITOR where SCH_ID = p_SCH_ID and CONFIG_ID = p_CONFIG_ID and ENV_ID = p_ENV_ID;
        if (v_MonitorID = '' or v_MonitorID =0 or v_MonitorID is null)
        Then
            v_UpdateStatus := 0;
        else
            select MON_STATUS into v_MonitorStatus from CSM_MONITOR where mon_id = v_MonitorID;
            if (v_MonitorStatus != p_MON_STATUS)
            Then	
                v_UpdateStatus := 0;
            else
                v_UpdateStatus := 1;
            End if;
        End if;
        
        If(v_UpdateStatus = 0)
		then
                insert into CSM_MONITOR (
                SCH_ID
                ,CONFIG_ID
                ,ENV_ID
                ,MON_STATUS
                ,MON_START_DATE_TIME
                ,MON_END_DATE_TIME
                ,MON_IS_ACTIVE
                ,MON_CREATED_BY
                ,MON_CREATED_DATE
                ,MON_UPDATED_BY
                ,MON_UPDATED_DATE
                ,MON_COMMENTS
                )values
                (
                p_SCH_ID
                ,p_CONFIG_ID
                ,p_ENV_ID
                ,p_MON_STATUS
                ,p_MON_START_DATE_TIME
                ,p_MON_END_DATE_TIME
                ,p_MON_IS_ACTIVE
                ,p_MON_CREATED_BY
                ,p_MON_CREATED_DATE
                ,p_MON_CREATED_BY
                ,p_MON_CREATED_DATE
                ,p_MON_COMMENTS      
                ) RETURNING MON_ID into v_tempMonitorID;
                --v_tempMonitorID := IDENT_CURRENT('CSM_MONITOR');
                /*execute immediate  CWT_InsMonitorDailyStatus(
                    v_tempMonitorID 
                    ,p_CONFIG_ID
                    ,p_ENV_ID
                    ,p_MON_CREATED_DATE
                    ,p_MON_STATUS
                    ,'');*/
        Else
            /* Update the monistor status*/
                Update CSM_MONITOR set 
                    MON_STATUS = p_MON_STATUS,
                    MON_START_DATE_TIME = p_MON_START_DATE_TIME,
                    MON_END_DATE_TIME = p_MON_END_DATE_TIME,
                    MON_UPDATED_BY = p_MON_CREATED_BY,
                    MON_UPDATED_DATE = p_MON_CREATED_DATE,
                    MON_COMMENTS = p_MON_COMMENTS
                    where MON_ID = v_MonitorID;
        End if;

            if v_MonitorID > 0
            Then
                select count(*) into v_tempDailyStatusCount  from CSM_MON_DAILY_STATUS 
                where TO_CHAR(MON_TRACK_DATE,'mm/dd/yyyy HH24:MI:SS') = TO_CHAR(p_MON_CREATED_DATE,'mm/dd/yyyy HH24:MI:SS')
                    and MON_ID = v_MonitorID;
                /*
                if v_tempDailyStatusCount <= 0
                Then
                    CWT_InsMonitorDailyStatus; 
                        v_MonitorID 
                        ,p_CONFIG_ID
                        ,p_ENV_ID
                        ,p_MON_CREATED_DATE
                        ,p_MON_STATUS
                        ,'');
                End if;*/
            END IF;        
            
            /*---Reset email notification flag to true in case of service back to normal or running  */
            if(lower(p_MON_STATUS) = 'running' or lower(p_MON_STATUS) = 'standby')
            then
                update CSM_CONFIGURATION set CONFIG_ISNOTIFY = 1 
                    where CONFIG_ID = p_CONFIG_ID
                        and CONFIG_ISNOTIFY_MAIN = 1;
                --Execute immediate  CWT_InsertBuildDetails;  p_CONFIG_ID,p_ENV_ID,p_MON_CREATED_DATE,p_MON_COMMENTS
            end if;
         
         SP_CWT_InsertCSMLog( p_SCH_ID, p_CONFIG_ID, p_ENV_ID,p_MON_STATUS, p_MON_COMMENTS, p_MON_CREATED_DATE, p_MON_CREATED_BY);
        
    End;
    
    PROCEDURE SP_CWT_InsertMailLog(       p_ENV_ID number
                                      ,p_CONFIG_ID number
                                      ,p_EMTRAC_TO_ADDRESS varchar2
                                      ,p_EMTRAC_CC_ADDRESS varchar2
                                      ,p_EMTRAC_BCC_ADDRESS varchar2
                                      ,p_EMTRAC_SUBJECT varchar2
                                      ,p_EMTRAC_BODY varchar2
                                      ,p_EMTRAC_SEND_STATUS varchar2
                                      ,p_EMTRAC_SEND_ERROR varchar2
                                      ,p_EMTRAC_CONTENT_TYPE varchar2
                                      ,p_EMTRAC_CREATED_BY varchar2
                                      ,p_EMTRAC_CREATED_DATE timestamp
                                      ,p_EMTRAC_COMMENTS varchar2
                                ) AS
    Begin
        Insert into CSM_EMAIL_TRACKING
        (
               ENV_ID
              ,Config_ID
              ,EMTRAC_TO_ADDRESS
              ,EMTRAC_CC_ADDRESS
              ,EMTRAC_BCC_ADDRESS
              ,EMTRAC_SUBJECT
              ,EMTRAC_BODY
              ,EMTRAC_SEND_STATUS
              ,EMTRAC_SEND_ERROR
              ,EMTRAC_CONTENT_TYPE
              ,EMTRAC_CREATED_BY
              ,EMTRAC_CREATED_DATE
              ,EMTRAC_COMMENTS
              ,EMTRAC_IS_ACTIVE
        ) values
        (
            p_ENV_ID
          ,p_CONFIG_ID
          ,p_EMTRAC_TO_ADDRESS
          ,p_EMTRAC_CC_ADDRESS
          ,p_EMTRAC_BCC_ADDRESS
          ,p_EMTRAC_SUBJECT
          ,p_EMTRAC_BODY
          ,p_EMTRAC_SEND_STATUS
          ,p_EMTRAC_SEND_ERROR
          ,p_EMTRAC_CONTENT_TYPE
          ,p_EMTRAC_CREATED_BY
          ,p_EMTRAC_CREATED_DATE
          ,p_EMTRAC_COMMENTS
          ,1
        ) ;   
    End;

    procedure SP_CWT_InsertCSMLog(
                                p_SCH_ID number,
                                p_CONFIG_ID number,
                                p_ENV_ID number ,
                                p_LOGDESCRIPTION varchar2,
                                p_LOGERROR varchar2,
                                p_LOG_UPDATE_TATETIME timestamp,
                                p_LOG_UPDATED_BY varchar2
                              ) IS
    Begin
        insert into CSM_LOG (
            SCH_ID
            ,CONFIG_ID
            ,ENV_ID
            ,LOGDESCRIPTION
            ,LOGERROR
            ,LOG_UPDATED_DATETIME
            ,LOG_UPDATED_BY
          ) values(
            p_SCH_ID,
            p_CONFIG_ID,
            p_ENV_ID,
            p_LOGDESCRIPTION,
            p_LOGERROR,
            p_LOG_UPDATE_TATETIME,
            p_LOG_UPDATED_BY      
          );
    End;

    procedure SP_CWT_InsIncidentTracking(  p_MON_ID number
                                          ,p_ENV_ID number
                                          ,p_CONFIG_ID number
                                          ,p_TRK_ISSUE varchar
                                          ,p_TRK_SOLUTION varchar
                                          ,p_TRK_CREATED_BY varchar2
                                          ,p_TRK_CREATED_DATE timestamp
                                          ,p_TRK_COMMENTS varchar
                                        ) IS
    Begin
        Insert into CSM_INCIDENT
        (
               MON_ID
              ,ENV_ID
              ,CONFIG_ID
              ,TRK_ISSUE
              ,TRK_SOLUTION
              ,TRK_CREATED_BY
              ,TRK_CREATED_DATE
              ,TRK_COMMENTS
          )
          values
          (
               p_MON_ID
              ,p_ENV_ID
              ,p_CONFIG_ID
              ,p_TRK_ISSUE
              ,p_TRK_SOLUTION
              ,p_TRK_CREATED_BY
              ,p_TRK_CREATED_DATE
              ,p_TRK_COMMENTS
          );    
    End;
    
    PROCEDURE SP_CWT_InsUpdPersonalize( p_PERS_ID number
                                        ,p_USER_ID number
                                        ,p_PERS_DB_REFRESHTIME number
                                        ,p_PERS_ISACTIVE number
                                        ,p_PERS_CREATEDDATE timestamp
                                        ,p_PERS_CREATEDBY varchar2
                                        ,p_PERS_SORTORDER varchar2) IS
    v_Character CHAR(1)
	;v_StartIndex NUMBER(10)
	;v_EndIndex NUMBER(10)
	;v_Input varchar(4000)
	;v_temp_PERS_Count number(10)
	;v_tempSortOrder number(10)
	;v_tempCount number(10);                                        
    Begin
        v_Character := ',';
        v_Input := p_PERS_SORTORDER;
        v_StartIndex := 1;
        v_tempCount := 0;
        
        if p_USER_ID > 0 
        Then
            select COUNT(*) into v_temp_PERS_Count	
            from CSM_POERSONALIZE
            Where USER_ID = p_USER_ID;
            
            if v_temp_PERS_Count = 0
            Then
                insert into CSM_POERSONALIZE
                (
                    User_ID
                    ,PERS_DB_REFRESHTIME
                    ,PERS_ISACTIVE
                    ,PERS_CREATEDDATE
                    ,PERS_CREATEDBY
                )
                values
                (
                    p_USER_ID
                    ,p_PERS_DB_REFRESHTIME
                    ,p_PERS_ISACTIVE
                    ,p_PERS_CREATEDDATE
                    ,p_PERS_CREATEDBY
                );
            else if v_temp_PERS_Count >= 1 
            Then
                Update CSM_POERSONALIZE set
                    PERS_DB_REFRESHTIME = p_PERS_DB_REFRESHTIME
                    ,PERS_ISACTIVE = p_PERS_ISACTIVE
                    ,PERS_UPDATEDDATE = p_PERS_CREATEDDATE
                    ,PERS_UPDATEDBY = p_PERS_CREATEDBY
                    Where USER_ID = p_USER_ID;
            end if;
            end if;
            
            IF SUBSTR(v_Input, LENGTH(RTRIM(v_Input)) , LENGTH(RTRIM(v_Input))) <> v_Character
            THEN
                v_Input := v_Input || v_Character;
            END IF;
            WHILE INSTR(v_Input, v_Character) > 0
            Loop	
                v_EndIndex := INSTR(v_Input, v_Character);
                v_tempSortOrder := SUBSTR(v_Input, v_StartIndex, v_EndIndex - 1);	
                v_tempCount := v_tempCount + 1;
                update CSM_ENVIRONEMENT set ENV_SORTORDER = v_tempCount where ENV_ID = v_tempSortOrder;
                
                v_Input := SUBSTR(v_Input, v_EndIndex + 1, LENGTH(RTRIM(v_Input)));
            End LOOP;    
            
        End if;
    End;

    PROCEDURE SP_CWT_InsUpdServerPerformance
                                      (
                                            p_ENVID number
                                          ,p_CONFIGID number
                                          ,p_PER_HOSTIP varchar2
                                          ,p_PER_CPU_USAGE binary_double
                                          ,p_PER_AVAILABLEMEMORY binary_double
                                          ,p_PER_TOTALMEMORY binary_double
                                          ,p_PER_CREATED_BY varchar2
                                          ,p_PER_COMMENTS varchar
                                          ,p_SCOPE_OUTPUT out number 
                                      ) IS
    Begin
        insert into CSM_SERVERPERFORMANCE
        (
        ENVID
          ,CONFIGID
          ,PER_HOSTIP
          ,PER_CPU_USAGE
          ,PER_AVAILABLEMEMORY
          ,PER_TOTALMEMORY
          ,PER_CREATED_BY
          ,PER_COMMENTS
          )
        values
        (
            p_ENVID
          ,p_CONFIGID
          ,p_PER_HOSTIP
          ,p_PER_CPU_USAGE
          ,p_PER_AVAILABLEMEMORY
          ,p_PER_TOTALMEMORY
          ,p_PER_CREATED_BY
          ,p_PER_COMMENTS
         ) RETURNING PER_ID INTO p_SCOPE_OUTPUT;
          --p_SCOPE_OUTPUT := IDENT_CURRENT('CSM_SERVERPERFORMANCE');
    End;
    
    PROCEDURE SP_CWT_InsUpdServerPerfDrive
                                          (    p_PER_ID number
                                              ,p_DRIVE_NAME varchar2
                                              ,p_DRIVE_LABEL varchar2
                                              ,p_DRIVE_FORMAT varchar2
                                              ,p_DRIVE_TYPE varchar2
                                              ,p_DRIVE_FREESPACE binary_double
                                              ,p_DRIVE_USEDSPACE binary_double
                                              ,p_DRIVE_TOTALSPACE binary_double
                                              ,p_DRIVE_COMMENTS varchar
                                          ) IS
    Begin
        insert into CSM_SERVERPERFORMANCE_DRIVE
        (
            PER_ID
          ,DRIVE_NAME
          ,DRIVE_LABEL
          ,DRIVE_FORMAT
          ,DRIVE_TYPE
          ,DRIVE_FREESPACE
          ,DRIVE_USEDSPACE
          ,DRIVE_TOTALSPACE
          ,DRIVE_COMMENTS
        )
        values
        (
            p_PER_ID
          ,p_DRIVE_NAME
          ,p_DRIVE_LABEL
          ,p_DRIVE_FORMAT
          ,p_DRIVE_TYPE
          ,p_DRIVE_FREESPACE
          ,p_DRIVE_USEDSPACE
          ,p_DRIVE_TOTALSPACE
          ,p_DRIVE_COMMENTS
        );
    End;
                              
        PROCEDURE SP_CWT_InsUpdServerPerfSch
                                      (
                                        p_ENVID number
                                        ,p_CONFIGID number
                                        ,p_HOSTIP varchar2
                                        ,p_PORT varchar2
                                        ,p_LASTJOBRUNTIME timestamp
                                        ,p_NEXTJOBRUNTIME timestamp
                                        ,p_MODE varchar2
                                      ) IS
        v_SVRID number(10);                                      
    Begin
        select count(*) into v_SVRID from CSM_SERVERPERFORMANCE_SCHEDULE 
        where ENVID = p_ENVID
        and CONFIGID = p_CONFIGID;
        
    
        if p_MODE = 'NS' -- Next schedule
        Then
            update CSM_SERVERPERFORMANCE_SCHEDULE set
            SVR_LASTJOBRAN_TIME = p_LASTJOBRUNTIME,
            SVR_NEXTJOBRAN_TIME = p_NEXTJOBRUNTIME
            where SVR_HOSTIP = p_HOSTIP;
        End if;
    
        if (v_SVRID <= 0 or v_SVRID is null or v_SVRID = '') and p_MODE = 'IS' -- Initial schedule
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
    
    
END;
/

Exit;