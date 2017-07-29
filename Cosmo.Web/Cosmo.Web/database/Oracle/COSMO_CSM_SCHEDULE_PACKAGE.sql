
CREATE OR REPLACE PACKAGE "COSMO_CSM_SCHEDULE_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    
    --CWT_GetAllSchedule_NextJobStartBefore
    FUNCTION FN_CWT_GetAllSchedule_NJS (p_DateNextJobRunStartBefore timestamp) RETURN o_Cursor;
    FUNCTION FN_CWT_GetAllScheduledServices(p_DateNextJobRunStartBefore timestamp) RETURN o_Cursor;
    FUNCTION FN_CWT_GetUserEmailList(p_Env_ID number, p_MessageType varchar2)  RETURN o_Cursor;
    FUNCTION FN_CWT_GetSubscription(p_ID number)   RETURN o_Cursor;
    FUNCTION FN_CWT_GetSubscriptionDetail(p_ID number)   RETURN o_Cursor;
    FUNCTION FN_CWT_GetPortelToMonitor(p_ID number) RETURN o_Cursor;
    FUNCTION FN_CWT_GetSubsMonitorStatus(   p_ENV_ID number
                                                    ,p_TYPE varchar2
                                                    ,p_STARTTIME timestamp
                                                    ,p_ENDTIME timestamp) RETURN o_Cursor;
    
    PROCEDURE SP_CWT_UpdateScheduleLastRunDt (p_SchedulerID number,p_DateLastJobRun timestamp);
    PROCEDURE SP_CWT_SetSubsNextJobRunTime(p_ID number, p_LASTJOBRANTIME timestamp, p_NEXTJOBRUNTIME timestamp);
    PROCEDURE SP_CWT_SetPortalNextJobRunTime(p_ID number, p_LASTJOBRANTIME timestamp, p_NEXTJOBRUNTIME timestamp);
    PROCEDURE SP_CWT_InsertPortalStatus(p_URL_ID number
                                        ,p_ENV_ID number
                                        ,p_PMON_STATUS varchar2
                                        ,p_PMON_MESSAGE varchar
                                        ,p_PMON_RESPONSETIME number
                                        ,p_PMON_EXCEPTION varchar
                                        );
    
    
END COSMO_CSM_SCHEDULE_PACKAGE;    
/

CREATE OR REPLACE PACKAGE BODY "COSMO_CSM_SCHEDULE_PACKAGE" AS

    FUNCTION FN_CWT_GetAllSchedule_NJS (p_DateNextJobRunStartBefore timestamp) RETURN o_Cursor IS
    CUR o_Cursor;
    v_currentDateTime timestamp;
    Begin
        v_currentDateTime := TO_TIMESTAMP(to_char(p_DateNextJobRunStartBefore,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS');
        Open cur FOR Select 
                SCH_ID
              ,cs.ENV_ID
              ,ce.ENV_NAME
              ,SCH_INTERVAL
              ,SCH_DURATION
              ,SCH_IS_ACTIVE
              ,SCH_LASTJOBRAN_TIME
              ,SCH_NEXTJOBRAN_TIME
              ,SCH_CREATED_BY
              ,SCH_CREATED_DATE
              ,SCH_UPDATED_BY
              ,SCH_UPDATED_DATE
              ,SCH_COMMENTS	
            from CSM_SCHEDULE cs 
            inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = cs.ENV_ID
            inner join CSM_CONFIGURATION con on CON.CONFIG_ID = CS.CONGIG_ID
            where (SCH_NEXTJOBRAN_TIME is NULL or cs.SCH_NEXTJOBRAN_TIME <= v_currentDateTime) 
            and cs.SCH_STARTBY <= v_currentDateTime
            and (cs.SCH_ENDBY) >= v_currentDateTime
            and cs.SCH_IS_ACTIVE = 1
            and CON.CONFIG_IS_ACTIVE = 1;        
    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetAllScheduledServices(p_DateNextJobRunStartBefore timestamp) RETURN o_Cursor IS
    CUR o_Cursor;
    v_currentDateTime timestamp;
    Begin
        v_currentDateTime := TO_TIMESTAMP(to_char(p_DateNextJobRunStartBefore,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS');
        Open cur FOR Select
            CONFIG_ID
          ,cc.ENV_ID
          ,CONFIG_SERVICE_TYPE
          ,CONFIG_HOST_IP
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
          ,ce.ENV_NAME
          ,cs.SCH_ID
          from CSM_CONFIGURATION cc
          inner join CSM_ENVIRONEMENT ce on ce.ENV_ID = cc.ENV_ID
          inner join CSM_SCHEDULE cs on cs.ENV_ID = cc.ENV_ID and cs.CONGIG_ID = cc.CONFIG_ID
          where cc.Config_ID in (
            Select 
            CONGIG_ID
            from CSM_SCHEDULE cs 
            where (SCH_NEXTJOBRAN_TIME is null or cs.SCH_NEXTJOBRAN_TIME <= v_currentDateTime) 
            and cs.SCH_STARTBY <= v_currentDateTime
            and cs.SCH_ENDBY >= v_currentDateTime
            and cs.SCH_IS_ACTIVE = 1      
          ) 
          and cc.CONFIG_IS_ACTIVE = 1 
          and cc.CONFIG_IS_MONITORED = 1  
          and cc.CONFIG_IS_VALIDATED = 1;        
        RETURN CUR;
    End;

    FUNCTION FN_CWT_GetUserEmailList(p_Env_ID number, p_MessageType varchar2)  RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        OPEN CUR FOR SELECT  USRLST_ID
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

    FUNCTION FN_CWT_GetSubscription(p_ID number)   RETURN o_Cursor IS
     CUR o_Cursor;
    Begin
        If p_ID > 0
            Then
                open CUR for select 
                    s.SUBSCRIPTION_ID
                    ,s.SUBSCRIPTION_TYPE
                    ,s.SUBSCRIPTION_TIME
                    ,s.SUBSCRIPTION_ISACTIVE
                    ,s.CREATED_BY
                    ,u.USER_FIRST_NAME || ' ' || u.USER_LAST_NAME as CREATEDBY_NAME
                    ,s.CREATED_DATE
                    ,s.UPDATED_BY
                    ,u.USER_FIRST_NAME || ' ' || u.USER_LAST_NAME as UPDATEDBY_NAME
                    ,s.UPDATED_DATE
                    ,s.SCH_LASTJOBRAN_TIME
                    ,s.SCH_NEXTJOBRAN_TIME
                from CSM_REPORT_SUBSCRIPTION s
                left join CSM_USER u on u.USER_ID = s.CREATED_BY
                left join CSM_USER uu on uu.USER_ID = s.UPDATED_BY
                Where s.SUBSCRIPTION_ID = p_ID;
        Else
                open CUR for select 
                    s.SUBSCRIPTION_ID
                    ,s.SUBSCRIPTION_TYPE
                    ,s.SUBSCRIPTION_TIME
                    ,s.SUBSCRIPTION_ISACTIVE
                    ,s.CREATED_BY
                    ,u.USER_FIRST_NAME || ' ' || u.USER_LAST_NAME as CREATEDBY_NAME
                    ,s.CREATED_DATE
                    ,s.UPDATED_BY
                    ,u.USER_FIRST_NAME || ' ' || u.USER_LAST_NAME as UPDATEDBY_NAME
                    ,s.UPDATED_DATE
                    ,s.SCH_LASTJOBRAN_TIME
                    ,s.SCH_NEXTJOBRAN_TIME
                from CSM_REPORT_SUBSCRIPTION s
                left join CSM_USER u on u.USER_ID = s.CREATED_BY
                left join CSM_USER uu on uu.USER_ID = s.UPDATED_BY;
        End if;    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetSubscriptionDetail(p_ID number)   RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        If p_ID > 0
        Then
            open CUR for select 
                s.SUBSCRIPTION_DETAIL_ID
                ,s.SUBSCRIPTION_ID
                ,s.SUBSCRIPTION_EMAILID
                ,s.USRLST_ID
                ,s.SUBSCRIPTION_ISACTIVE
            from CSM_REPORT_SUBS_DETAILS s
            where s.SUBSCRIPTION_ID = p_ID;
        End if;
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetPortelToMonitor(p_ID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
		if p_ID > 0
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
				  ,uc.URL_CREATEDBY
				  ,uc.URL_CREATEDDATE
				  ,uc.URL_UPDATEDBY
				  ,uc.URL_UPDATEDDATE
				  ,uc.URL_COMMENTS
				  ,uc.URL_LASTJOBRUNTIME
				  ,uc.URL_NEXTJOBRUNTIME
			  FROM CSM_URLCONFIGURATION uc
			  join CSM_ENVIRONEMENT env 
			  ON env.ENV_ID = uc.ENV_ID
			  where URL_ISACTIVE = 1
			  and URL_ID = p_ID;
		Else
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
				  ,uc.URL_CREATEDBY
				  ,uc.URL_CREATEDDATE
				  ,uc.URL_UPDATEDBY
				  ,uc.URL_UPDATEDDATE
				  ,uc.URL_COMMENTS
				  ,uc.URL_LASTJOBRUNTIME
				  ,uc.URL_NEXTJOBRUNTIME
			  FROM CSM_URLCONFIGURATION uc
			  join CSM_ENVIRONEMENT env 
			  ON env.ENV_ID = uc.ENV_ID
			  where URL_ISACTIVE = 1;
		End if;
        
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetSubsMonitorStatus(   p_ENV_ID number
                                            ,p_TYPE varchar2
                                            ,p_STARTTIME timestamp
                                            ,p_ENDTIME timestamp) RETURN o_Cursor IS
    CUR o_Cursor;
    StartTime timestamp:= TO_TIMESTAMP(to_char(p_STARTTIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS');
    EndTime timestamp:= TO_TIMESTAMP(to_char(p_ENDTIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS');
    Begin
        if p_TYPE = 'Daily'
        Then
            open CUR for select m.MON_ID
            ,m.ENV_ID
            ,m.CONFIG_ID
            ,mds.MON_TRACK_STATUS MON_STATUS
            ,m.MON_CREATED_DATE
            ,m.MON_UPDATED_DATE
            , case 
                when c.CONFIG_SERVICE_TYPE = 1 then 'Content Manager'
                when c.CONFIG_SERVICE_TYPE = 2 then 'Dispatcher'
              end CONFIG_SERVICE_TYPE
            ,c.CONFIG_HOST_IP
            ,c.CONFIG_PORT_NUMBER
            ,c.CONFIG_DESCRIPTION
            ,c.CONFIG_ISPRIMARY
            ,mds.MON_TRACK_DATE	
            from CSM_MONITOR m
            left join CSM_MON_DAILY_STATUS mds 
            ON m.MON_ID = mds.MON_ID
            left join CSM_CONFIGURATION c
            ON m.CONFIG_ID = c.CONFIG_ID and
                m.ENV_ID = c.ENV_ID
            where 
                m.ENV_ID = p_ENV_ID
                and mds.MON_TRACK_DATE >= StartTime
                and mds.MON_TRACK_DATE <= EndTime;
        End if;
        RETURN CUR;
    End;
                                                        
    PROCEDURE SP_CWT_UpdateScheduleLastRunDt (p_SchedulerID number,p_DateLastJobRun timestamp) IS
        v_currentDateTime TIMESTAMP;
    Begin
        v_currentDateTime := TO_TIMESTAMP(to_char(p_DateLastJobRun,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS');
        update CSM_SCHEDULE set SCH_LASTJOBRAN_TIME = v_currentDateTime, SCH_NEXTJOBRAN_TIME = 
            ( Case
                When (lower(SCH_DURATION) = 'seconds' or lower(SCH_DURATION) = 'second' or lower(SCH_DURATION) = 'sec' or lower(SCH_DURATION) = 's') then  ( v_currentDateTime + interval '1' SECOND)
                When (lower(SCH_DURATION) = 'minutes' or lower(SCH_DURATION) = 'minute' or lower(SCH_DURATION) = 'min' or lower(SCH_DURATION) = 'm') then SCH_INTERVAL + (interval '1' MINUTE +v_currentDateTime)
                When (lower(SCH_DURATION) = 'hours' or lower(SCH_DURATION) = 'hour' or lower(SCH_DURATION) = 'hr' or lower(SCH_DURATION) = 'h') then SCH_INTERVAL + (interval '1' HOUR +v_currentDateTime)
                When (lower(SCH_DURATION) = 'days' or lower(SCH_DURATION) = 'day' or lower(SCH_DURATION) = 'dy' or lower(SCH_DURATION) = 'd') then SCH_INTERVAL + (interval '1' DAY +v_currentDateTime)
                When (lower(SCH_DURATION) = 'weeks' or lower(SCH_DURATION) = 'week' or lower(SCH_DURATION) = 'wk' or lower(SCH_DURATION) = 'w') then SCH_INTERVAL + (interval '7' DAY +v_currentDateTime)
                When (lower(SCH_DURATION) = 'months' or lower(SCH_DURATION) = 'month' or lower(SCH_DURATION) = 'mn' or lower(SCH_DURATION) = 'm') then SCH_INTERVAL + (interval '1' MONTH +v_currentDateTime)
               End
            )
            where SCH_ID = p_SchedulerID;        
    End;
    
    PROCEDURE SP_CWT_SetSubsNextJobRunTime(p_ID number, p_LASTJOBRANTIME timestamp, p_NEXTJOBRUNTIME timestamp) IS
    Begin
    	update CSM_REPORT_SUBSCRIPTION set
        SCH_LASTJOBRAN_TIME = p_LASTJOBRANTIME
        ,SCH_NEXTJOBRAN_TIME = p_NEXTJOBRUNTIME
        where SUBSCRIPTION_ID = p_ID;
    End;

    PROCEDURE SP_CWT_SetPortalNextJobRunTime(p_ID number, p_LASTJOBRANTIME timestamp, p_NEXTJOBRUNTIME timestamp) IS
    Begin
        update CSM_URLCONFIGURATION set
        URL_LASTJOBRUNTIME = p_LASTJOBRANTIME
        ,URL_NEXTJOBRUNTIME = p_NEXTJOBRUNTIME
        where URL_ID = p_ID;    
    End;
    
    PROCEDURE SP_CWT_InsertPortalStatus(p_URL_ID number
                                        ,p_ENV_ID number
                                        ,p_PMON_STATUS varchar2
                                        ,p_PMON_MESSAGE varchar
                                        ,p_PMON_RESPONSETIME number
                                        ,p_PMON_EXCEPTION varchar
                                        ) IS
    Begin
        if p_URL_ID > 0
        Then
            insert into CSM_PORTALMONITOR
            (
                URL_ID
                ,ENV_ID
                ,PMON_STATUS
                ,PMON_MESSAGE
                ,PMON_RESPONSETIME
                ,PMON_EXCEPTION
            )
            values
            (
                p_URL_ID
                ,p_ENV_ID
                ,p_PMON_STATUS
                ,p_PMON_MESSAGE
                ,p_PMON_RESPONSETIME
                ,p_PMON_EXCEPTION
            );
        End if;
    End;
    
END COSMO_CSM_SCHEDULE_PACKAGE;
/

Exit;