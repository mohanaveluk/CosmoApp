
CREATE OR REPLACE PACKAGE "COSMO_CSM_EXECUTIVE_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    FUNCTION FN_CWT_GetSendNotification(	p_CurrentTimeStamp timestamp) RETURN o_Cursor;
    FUNCTION FN_CWT_GetServiceLastStatus(p_SCH_ID number	,p_CONFIG_ID number	,p_ENV_ID number)  RETURN VARCHAR2;
    
    PROCEDURE SP_CWT_SetMonitorStatus(
                                    p_SCH_ID number
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
  
    PROCEDURE SP_CWT_InsMonitorDailyStatus(
                                        p_MON_ID number
                                        ,p_CONFIG_ID number
                                        ,p_ENV_ID number
                                        ,p_MON_TRACK_DATE timestamp
                                        ,p_MON_TRACK_STATUS varchar2
                                        ,p_MON_TRACK_COMMENTS varchar
                                        );
    PROCEDURE SP_CWT_InsertBuildDetails(
                                     p_CONFIG_ID number
                                    ,p_ENV_ID number
                                    ,p_MON_CREATED_DATE timestamp
                                    ,p_MON_COMMENTS varchar2
                                    );

     PROCEDURE SP_CWT_InsertCSMLog(
                                p_SCH_ID number,
                                p_CONFIG_ID number,
                                p_ENV_ID number ,
                                p_LOGDESCRIPTION varchar2,
                                p_LOGERROR varchar2,
                                p_LOG_UPDATE_TATETIME timestamp,
                                p_LOG_UPDATED_BY varchar2
                              );
                              
    PROCEDURE SP_CWT_InsertMailLog( p_ENV_ID number
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
END COSMO_CSM_EXECUTIVE_PACKAGE;    
/


CREATE OR REPLACE PACKAGE BODY "COSMO_CSM_EXECUTIVE_PACKAGE" AS
    
    FUNCTION FN_CWT_GetSendNotification( p_CurrentTimeStamp timestamp) RETURN o_Cursor IS
    CUR o_Cursor;
    v_currentDateTime timestamp;
    Begin
    
        open CUR for select 
            con.CONFIG_ID,
            con.ENV_ID,
            con.CONFIG_SERVICE_TYPE,
            con.CONFIG_IS_ACTIVE,
            con.CONFIG_ISNOTIFY,
            con.CONFIG_MAIL_FREQ,
            mon.MON_STATUS,
            --max(eml.EMTRAC_CREATED_DATE) EMTRAC_CREATED_DATE
            (select max(EMTRAC_CREATED_DATE) from CSM_EMAIL_TRACKING where Config_ID = con.CONFIG_ID AND ENV_ID = con.ENV_ID) EMTRAC_CREATED_DATE
            from CSM_CONFIGURATION con
            left  join CSM_MONITOR mon on mon.CONFIG_ID = con.CONFIG_ID
            left  join CSM_EMAIL_TRACKING eml on eml.Config_ID = mon.CONFIG_ID
            --and eml.Config_ID = con.CONFIG_ID
            where (lower(mon.MON_STATUS) = 'stopped' or lower(mon.MON_STATUS) like '%not running%') 
                --and CONVERT(VARCHAR(19), @currentDateTime, 'mm/dd/yyyy HH24:MI:SS') >= CONVERT(VARCHAR(19), DATEADD(mi,con.CONFIG_MAIL_FREQ,eml.EMTRAC_CREATED_DATE), 'mm/dd/yyyy HH24:MI:SS')
                and con.CONFIG_IS_ACTIVE = 1
                and con.CONFIG_ISNOTIFY = 1
                and con.CONFIG_ID in
                (
                Select 
                    CONGIG_ID
                    from CSM_SCHEDULE cs 
                    where (SCH_NEXTJOBRAN_TIME is null or cs.SCH_NEXTJOBRAN_TIME <= v_currentDateTime) 
                    and cs.SCH_ENDBY <= v_currentDateTime
                    and cs.SCH_ENDBY >= v_currentDateTime
                    and cs.SCH_IS_ACTIVE = 1 
                )
            Group by
                con.CONFIG_ID,
                con.ENV_ID,
                con.CONFIG_SERVICE_TYPE,
                con.CONFIG_IS_ACTIVE,
                con.CONFIG_ISNOTIFY,
                con.CONFIG_MAIL_FREQ,
                mon.MON_STATUS;    
    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetServiceLastStatus(p_SCH_ID number	,p_CONFIG_ID number	,p_ENV_ID number)  RETURN VARCHAR2 IS
    CUR o_Cursor;
    v_MonitorStatus varchar2(100):= '';
    v_MonitorID number(10);
    Begin
        select NVL(max(mon_id),0) into v_MonitorID from CSM_MONITOR where SCH_ID = p_SCH_ID 
            and CONFIG_ID = p_CONFIG_ID 
            and ENV_ID = p_ENV_ID;
            
        If (v_MonitorID > 0) then
            select MON_STATUS INTO v_MonitorStatus from CSM_MONITOR where mon_id = v_MonitorID;
        End If;
        
        RETURN v_MonitorStatus;
    End;
     PROCEDURE SP_CWT_SetMonitorStatus(
                                    p_SCH_ID number
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
        select NVL(max(mon_id),0) into v_MonitorID from CSM_MONITOR where SCH_ID = p_SCH_ID and CONFIG_ID = p_CONFIG_ID and ENV_ID = p_ENV_ID;
        if (v_MonitorID =0 or v_MonitorID is null)
        Then
            v_UpdateStatus := 0;
        else
            select MON_STATUS into v_MonitorStatus from CSM_MONITOR where mon_id = v_MonitorID;
            if (v_MonitorStatus = p_MON_STATUS)
            Then	
                v_UpdateStatus := 1;
            else
                v_UpdateStatus := 0;
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
                ) RETURNING MON_ID INTO v_tempMonitorID;
                --v_tempMonitorID := IDENT_CURRENT('CSM_MONITOR');
                SP_CWT_InsMonitorDailyStatus(
                    v_tempMonitorID 
                    ,p_CONFIG_ID
                    ,p_ENV_ID
                    ,p_MON_CREATED_DATE
                    ,p_MON_STATUS
                    ,'');
        else
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
		--insert daily status record for report
		if v_MonitorID > 0
		Then
			select count(*) into v_tempDailyStatusCount  from CSM_MON_DAILY_STATUS 
			where MON_TRACK_DATE = p_MON_CREATED_DATE
				and MON_ID = v_MonitorID;
			if v_tempDailyStatusCount <= 0
			Then
				SP_CWT_InsMonitorDailyStatus(
					v_MonitorID 
					,p_CONFIG_ID
					,p_ENV_ID
					,p_MON_CREATED_DATE
					,p_MON_STATUS
					,'');
			End if;
		END IF;              
		/*---Reset email notification flag to true in case of service back to normal or running  */
		if(lower(p_MON_STATUS) = 'running' or lower(p_MON_STATUS) = 'standby')
		then
			update CSM_CONFIGURATION set CONFIG_ISNOTIFY = 1 
				where CONFIG_ID = p_CONFIG_ID
					and CONFIG_ISNOTIFY_MAIN = 1;
			SP_CWT_InsertBuildDetails(  p_CONFIG_ID,p_ENV_ID,p_MON_CREATED_DATE,p_MON_COMMENTS);
		end if;        
        SP_CWT_InsertCSMLog( p_SCH_ID, p_CONFIG_ID, p_ENV_ID,p_MON_STATUS, p_MON_COMMENTS, p_MON_CREATED_DATE, p_MON_CREATED_BY);   
        
    End;
    
    
    
    procedure SP_CWT_InsMonitorDailyStatus(
                                        p_MON_ID number
                                        ,p_CONFIG_ID number
                                        ,p_ENV_ID number
                                        ,p_MON_TRACK_DATE timestamp
                                        ,p_MON_TRACK_STATUS varchar2
                                        ,p_MON_TRACK_COMMENTS varchar
                                        )    IS
    Begin
        insert into CSM_MON_DAILY_STATUS 
        (
            MON_ID
            ,CONFIG_ID
            ,ENV_ID
            ,MON_TRACK_DATE
            ,MON_TRACK_STATUS
            ,MON_TRACK_COMMENTS
        )
        values
        (
            p_MON_ID
            ,p_CONFIG_ID
            ,p_ENV_ID
            ,p_MON_TRACK_DATE
            ,p_MON_TRACK_STATUS
            ,p_MON_TRACK_COMMENTS
        )	;
    End;
    
    procedure SP_CWT_InsertBuildDetails(
                                     p_CONFIG_ID number
                                    ,p_ENV_ID number
                                    ,p_MON_CREATED_DATE timestamp
                                    ,p_MON_COMMENTS varchar2
                                    ) IS
    v_BuildID number(10)
	;v_Version varchar2(100)
	;v_StartIndex number(10)
	;v_EndIndex number(10);
    v_cout NUMBER;
    Begin
        if(INSTR(p_MON_COMMENTS, 'Build') > 0)
        Then
            v_StartIndex := INSTR(p_MON_COMMENTS, 'Build:') + LENGTH(RTRIM('Build: '));
            v_EndIndex := INSTR(p_MON_COMMENTS, 'Status');
            v_Version := SUBSTR(p_MON_COMMENTS,v_StartIndex,(v_EndIndex - v_StartIndex));
        else if(INSTR(p_MON_COMMENTS, 'version:') > 0)
            Then
                v_StartIndex := INSTR(p_MON_COMMENTS, 'version:') +  + LENGTH(RTRIM('version: '));	
                v_EndIndex := INSTR(p_MON_COMMENTS, 'Dispatcher');
                v_Version := SUBSTR(p_MON_COMMENTS,v_StartIndex,(v_EndIndex - v_StartIndex));
            end if;
        end if;
        
        if v_Version is not null
        Then
            select count(*) INTO v_cout from CSM_SERVICEBUILD bld where bld.ENV_ID = p_ENV_ID and bld.CONFIG_ID = p_CONFIG_ID and LTRIM(RTRIM(bld.BUILD_VERSION)) = LTRIM(RTRIM(v_Version));
            if (v_cout > 0) then
                select BUILD_ID into v_BuildID from CSM_SERVICEBUILD bld where bld.ENV_ID = p_ENV_ID and bld.CONFIG_ID = p_CONFIG_ID and LTRIM(RTRIM(bld.BUILD_VERSION)) = LTRIM(RTRIM(v_Version));
            else
                v_BuildID := 0;
            end if;
            if(v_BuildID = 0)
            Then
                insert into CSM_SERVICEBUILD
                (
                    ENV_ID,
                    CONFIG_ID,
                    BUILD_DATE,
                    BUILD_VERSION,
                    CREATED_DATE
                )
                values
                (
                    p_ENV_ID
                    ,p_CONFIG_ID
                    ,p_MON_CREATED_DATE
                    ,v_Version
                    ,p_MON_CREATED_DATE
                );
            End if;
        End if;
    End;
    
    PROCEDURE SP_CWT_InsertCSMLog(
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

    PROCEDURE SP_CWT_InsertMailLog( p_ENV_ID number
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
                            ) is
        
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
        );    
    End;
    
END COSMO_CSM_EXECUTIVE_PACKAGE;

/

Exit;