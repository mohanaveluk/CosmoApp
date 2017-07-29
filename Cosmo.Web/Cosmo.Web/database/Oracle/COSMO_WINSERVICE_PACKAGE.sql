
CREATE OR REPLACE PACKAGE "COSMO_WINSERVICE_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    
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
    
    PROCEDURE SP_CWT_InsGroup( p_GROUP_ID number
                                ,p_GROUP_NAME varchar2
                                ,p_GROUP_CREATED_BY varchar2
                                ,p_GROUP_CREATED_DATE timestamp
                                ,p_GROUP_COMMENTS varchar2
                                ,p_GROUP_ISACTIVE number
                            );
    
    PROCEDURE SP_CWT_InsGroupDetail(   p_GROUP_ID number
                                    ,p_ENV_ID number
                                    ,p_SERVICE_IDS varchar
                                    ,p_WIN_SERVICE_ID number
                                    ,p_GROUP_DETAIL_COMMENTS varchar2
                                    ,p_GROUP_CREATED_BY varchar2
                                    ,p_GROUP_CREATED_DATE timestamp
                                    ,p_GROUP_ISACTIVE number);    
                                    
    PROCEDURE SP_CWT_InsGroupSchedule ( p_GROUP_SCH_ID number
                                    ,p_GROUP_ID number
                                    ,p_GROUP_NAME varchar
                                    ,p_ENV_IDS varchar
                                    ,p_CONFIG_IDS varchar
                                    ,p_WIN_SERVICE_IDS varchar
                                    ,p_GROUP_SCH_ACTION varchar2
                                    ,p_GROUP_SCH_STATUS varchar2
                                    ,p_GROUP_SCH_TIME timestamp
                                    ,p_GROUP_SCH_COMMENTS	varchar
                                    ,p_GROUP_SCH_CREATED_BY varchar2
                                    ,p_GROUP_SCH_CREATED_DATETIME timestamp
                                    ,p_GROUP_SCH_ISACTIVE number
                                    ,p_GROUP_SCH_ONDEMAND number
                                    ,p_GROUP_SCH_COMPLETESTATUS varchar2
                                    ,p_GROUP_SCH_COMPLETEDTIME timestamp
                                    ,p_GROUP_SCH_REQUESTSOURCE varchar2
                                    ,p_GROUP_SCH_SERVICE_STARTTIME timestamp
                                    ,p_GROUP_SCH_SERVICE_COMPLETEDT timestamp
                                    ,p_SCOPE_OUTPUT out number 
                                    );
                            
    FUNCTION FN_CWT_GetWinServiceDetails
                                        (
                                            p_ENV_ID number 
                                        ) RETURN o_Cursor;   

    FUNCTION FN_CWT_GetUrlPerformance(p_ENVID number) RETURN o_Cursor;
    
    FUNCTION FN_CWT_GetUrlPerfLast24Hrs(p_ENVID number) RETURN o_Cursor;
    
    FUNCTION FN_CWT_GetGroup(p_GRP_ID number)  RETURN o_Cursor;
    FUNCTION FN_CWT_GetGroupDetail(	p_GROUP_ID number, p_ENV_ID number)  RETURN o_Cursor;
    FUNCTION FN_CWT_GetGroupID(p_GROUP_NAME varchar) RETURN o_Cursor;
    FUNCTION FN_CWT_GetGroupSchedule(p_GROUP_ID number, p_GROUP_SCH_ID number, p_CURRENTDATETIME timestamp) RETURN o_Cursor;
    FUNCTION FN_CWT_GetGroupSchServiceDet(   p_GROUP_SCH_ID number
                                                    ,p_Category varchar2
                                                    ,p_ENV_ID number
                                                    ,p_GROUP_SCH_STATUS char) RETURN o_Cursor;
    
    FUNCTION FN_CWT_GetWSConfigOnDemand(p_WIN_SERVICE_ID number) RETURN o_Cursor;
    FUNCTION FN_CWT_ReportRestartService(   p_SCHEDULETYPE varchar2
                                            ,p_STARTDATE TIMESTAMP
                                            ,p_ENDDATE TIMESTAMP) RETURN o_Cursor;
    
END COSMO_WINSERVICE_PACKAGE;
/


CREATE OR REPLACE PACKAGE BODY "COSMO_WINSERVICE_PACKAGE" AS

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
        --if @SVRID > 0
        --Begin
            --update [dbo].[CSM_SERVERPERFORMANCE_SCHEDULE] set
            --[SVR_LASTJOBRAN_TIME] = @LASTJOBRUNTIME,
            --[SVR_NEXTJOBRAN_TIME] = @NEXTJOBRUNTIME
            --where [ENVID] = @ENVID
            --and [CONFIGID] = @CONFIGID
        --End
        --Else
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

    PROCEDURE SP_CWT_InsGroup( p_GROUP_ID number
                                ,p_GROUP_NAME varchar2
                                ,p_GROUP_CREATED_BY varchar2
                                ,p_GROUP_CREATED_DATE timestamp
                                ,p_GROUP_COMMENTS varchar2
                                ,p_GROUP_ISACTIVE number
                            ) IS
    v_tempGroupID number(10);
    v_count NUMBER;
    Begin

        if (p_GROUP_ID = 0)
        Then
            
            select count(*) into v_count from CSM_GROUP where lower(GROUP_NAME) = LOWER(p_GROUP_NAME);
            --select GROUP_ID into v_tempGroupID from CSM_GROUP where lower(GROUP_NAME) = LOWER(p_GROUP_NAME);
            if v_count = 0
            Then
            Insert into CSM_GROUP 
            (
              GROUP_NAME
              ,GROUP_CREATED_BY
              ,GROUP_CREATED_DATE
              ,GROUP_COMMENTS
              ,GROUP_IS_ACTIVE
            )
            values
            (
                p_GROUP_NAME
                ,p_GROUP_CREATED_BY
                ,p_GROUP_CREATED_DATE
                ,p_GROUP_COMMENTS
                ,p_GROUP_ISACTIVE
            );
            else
                Update CSM_GROUP set  
                GROUP_NAME = p_GROUP_NAME
                ,GROUP_UPDATED_BY = p_GROUP_CREATED_BY
                ,GROUP_UPDATED_DATE = p_GROUP_CREATED_DATE
                ,GROUP_COMMENTS = p_GROUP_COMMENTS
                ,GROUP_IS_ACTIVE = p_GROUP_ISACTIVE
                where GROUP_id = p_GROUP_ID;
            End if;
        Else
            Update CSM_GROUP set
                GROUP_NAME = p_GROUP_NAME
              ,GROUP_UPDATED_BY = p_GROUP_CREATED_BY
              ,GROUP_UPDATED_DATE = p_GROUP_CREATED_DATE
              ,GROUP_COMMENTS = p_GROUP_COMMENTS
              ,GROUP_IS_ACTIVE = p_GROUP_ISACTIVE
            where GROUP_id = p_GROUP_ID;
        End if;    
    End;

    PROCEDURE SP_CWT_InsGroupDetail(   p_GROUP_ID number
                                    ,p_ENV_ID number
                                    ,p_SERVICE_IDS varchar
                                    ,p_WIN_SERVICE_ID number
                                    ,p_GROUP_DETAIL_COMMENTS varchar2
                                    ,p_GROUP_CREATED_BY varchar2
                                    ,p_GROUP_CREATED_DATE timestamp
                                    ,p_GROUP_ISACTIVE number) IS
    v_Character CHAR(1)
	;v_StartIndex NUMBER(10)
	;v_EndIndex NUMBER(10)
	;v_Input varchar(4000)
	;v_tempGroupDetailsID number(10)
	;v_tempConfigID number(10);
    v_count NUMBER;
    Begin
        v_Character := ',';
        v_Input := p_SERVICE_IDS;
        v_StartIndex := 1;
	
        IF SUBSTR(v_Input, LENGTH(RTRIM(v_Input)) - 0, LENGTH(RTRIM(v_Input))) <> v_Character
        THEN
                --print SUBSTR(v_Input, LENGTH(RTRIM(v_Input)) - 0, LENGTH(RTRIM(v_Input)))
                v_Input := v_Input || v_Character;
        END IF;
        --print @Input
        if p_GROUP_ID > 0
        Then
            
            --Delete from [CSM_GROUP_DETAIL] where GROUP_ID = @GROUP_ID and ENV_ID = @ENV_ID 
            update CSM_GROUP_DETAIL set GROUP_ISACTIVE = 0 
                where GROUP_ID = p_GROUP_ID; --and ENV_ID = @ENV_ID 
                
            if p_SERVICE_IDS is not null
            Then
                WHILE INSTR(v_Input, v_Character) > 0
                Loop
                    v_tempGroupDetailsID := 0;
                    v_EndIndex := INSTR(v_Input, v_Character);
                    v_tempConfigID := SUBSTR(v_Input, v_StartIndex, v_EndIndex - 1);
                    --print @tempConfigID --SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
                    
                    select count(*) into v_count from CSM_GROUP_DETAIL
                        where GROUP_ID = p_GROUP_ID 
                            and ENV_ID in (select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = v_tempConfigID) 
                            and CONFIG_ID = v_tempConfigID;
                    
                    if v_count > 0 then
                    select GROUP_DETAIL_ID into v_tempGroupDetailsID from CSM_GROUP_DETAIL
                        where GROUP_ID = p_GROUP_ID 
                            and ENV_ID in (select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = v_tempConfigID) 
                            and CONFIG_ID = v_tempConfigID;
                    else
                        v_tempGroupDetailsID := 0;
                    end if;
                    
                    --print '@tempGroupDetailsID :' + Convert(varchar(10),@tempGroupDetailsID)
                    if v_tempGroupDetailsID > 0 
                    Then
                        update CSM_GROUP_DETAIL set GROUP_ISACTIVE = 1 
                        where GROUP_DETAIL_ID = v_tempGroupDetailsID;
                    else
                        insert into CSM_GROUP_DETAIL
                        (
                            GROUP_ID
                            ,ENV_ID
                            ,CONFIG_ID
                            ,WIN_SERVICE_ID
                            ,GROUP_CREATED_BY
                            ,GROUP_CREATED_DATE
                            ,GROUP_DETAIL_COMMENTS
                            ,GROUP_ISACTIVE
                        )
                        values
                        (	
                            p_GROUP_ID
                            ,(select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = v_tempConfigID)
                            ,v_tempConfigID
                            ,p_WIN_SERVICE_ID
                            ,p_GROUP_CREATED_BY
                            ,p_GROUP_CREATED_DATE
                            ,p_GROUP_DETAIL_COMMENTS
                            ,1
                        );
                    End if;
                                                
                    v_Input := SUBSTR(v_Input, v_EndIndex + 1, LENGTH(RTRIM(v_Input)));
                End LOOP;		
            End if;
        End if;    
    End;

    PROCEDURE SP_CWT_InsGroupSchedule ( p_GROUP_SCH_ID number
                                    ,p_GROUP_ID number
                                    ,p_GROUP_NAME varchar
                                    ,p_ENV_IDS varchar
                                    ,p_CONFIG_IDS varchar
                                    ,p_WIN_SERVICE_IDS varchar
                                    ,p_GROUP_SCH_ACTION varchar2
                                    ,p_GROUP_SCH_STATUS varchar2
                                    ,p_GROUP_SCH_TIME timestamp
                                    ,p_GROUP_SCH_COMMENTS	varchar
                                    ,p_GROUP_SCH_CREATED_BY varchar2
                                    ,p_GROUP_SCH_CREATED_DATETIME timestamp
                                    ,p_GROUP_SCH_ISACTIVE number
                                    ,p_GROUP_SCH_ONDEMAND number
                                    ,p_GROUP_SCH_COMPLETESTATUS varchar2
                                    ,p_GROUP_SCH_COMPLETEDTIME timestamp
                                    ,p_GROUP_SCH_REQUESTSOURCE varchar2
                                    ,p_GROUP_SCH_SERVICE_STARTTIME timestamp
                                    ,p_GROUP_SCH_SERVICE_COMPLETEDT timestamp
                                    ,p_SCOPE_OUTPUT out number 
                                    ) IS
    v_Character CHAR(1)
	;v_StartIndex NUMBER(10)
	;v_EndIndex NUMBER(10)
	;v_Input varchar(4000)
	;v_tempConfigID number(10)
	;v_tempGroupDetailsID number(10)
	;v_tempGroupScheduleID number(10);
    v_GROUP_ID NUMBER;
    v_count NUMBER;
    
    Begin
        v_Character := ',';
        v_Input := p_CONFIG_IDS;
        v_StartIndex := 1;
        v_GROUP_ID := p_GROUP_ID;
        
        --insert / update group
        if p_GROUP_ID <= 0 
        Then
            SP_CWT_InsGroup(
                p_GROUP_ID
                ,p_GROUP_NAME
                ,p_GROUP_SCH_CREATED_BY
                ,p_GROUP_SCH_CREATED_DATETIME
                ,'Created using the dynamic page'
                ,1);	
            commit;
            
            select grp.GROUP_ID into v_GROUP_ID from CSM_GROUP grp where grp.GROUP_NAME = p_GROUP_NAME;
            
            SP_CWT_InsGroupDetail(
                v_GROUP_ID
                ,0
                ,p_CONFIG_IDS
                ,0
                ,'Updating group details from dynamic page'
                ,p_GROUP_SCH_CREATED_BY
                ,p_GROUP_SCH_CREATED_DATETIME
                ,1);
                    
        End if;
            
        if v_GROUP_ID >0
        Then
            --updating Group detils table if at all any changes
            SP_CWT_InsGroupDetail(
                v_GROUP_ID
                ,0
                ,p_CONFIG_IDS
                ,0
                ,'Updating group details from dynamic page'
                ,p_GROUP_SCH_CREATED_BY
                ,p_GROUP_SCH_CREATED_DATETIME
                ,1);
    
    
            if(p_GROUP_SCH_ID = 0) 
            then
                select count(*) into v_count from CSM_GROUP_SCHEDULE 
                where GROUP_ID = v_GROUP_ID
                    and GROUP_SCH_STATUS = 'O';
                if v_count > 0 then
                select GROUP_SCH_ID into v_tempGroupScheduleID from CSM_GROUP_SCHEDULE 
                where GROUP_ID = v_GROUP_ID
                    and GROUP_SCH_STATUS = 'O';
                else
                    v_tempGroupScheduleID := 0;
                end if;
            else
                v_tempGroupScheduleID := p_GROUP_SCH_ID;
            End if;
        
            
    
            if(v_tempGroupScheduleID >0)
            Then
                Update CSM_GROUP_SCHEDULE set 
                    GROUP_SCH_TIME = p_GROUP_SCH_TIME
                    ,GROUP_SCH_ACTION = p_GROUP_SCH_ACTION
                    ,GROUP_SCH_COMMENTS = p_GROUP_SCH_COMMENTS
                    ,GROUP_SCH_UPDATED_BY = p_GROUP_SCH_CREATED_BY
                    ,GROUP_SCH_UPDATED_DATETIME = p_GROUP_SCH_CREATED_DATETIME
                    --,[GROUP_SCH_RESULT] = @GROUP_SCH_COMPLETESTATUS
                    Where GROUP_SCH_ID = v_tempGroupScheduleID;
                p_SCOPE_OUTPUT := v_tempGroupScheduleID;
            Else
                if p_GROUP_SCH_ONDEMAND = 1 or p_GROUP_SCH_ONDEMAND = 1
                Then
                Insert into CSM_GROUP_SCHEDULE
                (
                      GROUP_ID
                      ,GROUP_SCH_TIME
                      ,GROUP_SCH_ACTION
                      ,GROUP_SCH_STATUS
                      ,GROUP_SCH_COMMENTS
                      ,GROUP_SCH_CREATED_BY
                      ,GROUP_SCH_CREATED_DATETIME
                      ,GROUP_SCH_COMPLETED_TIME
                      ,GROUP_SCH_ONDEMAND
                      ,GROUP_SCH_REQUESTSOURCE
                )
                values
                (
                        v_GROUP_ID
                        ,p_GROUP_SCH_TIME
                        ,p_GROUP_SCH_ACTION
                        ,p_GROUP_SCH_STATUS
                        ,p_GROUP_SCH_COMMENTS	
                        ,p_GROUP_SCH_CREATED_BY
                        ,p_GROUP_SCH_CREATED_DATETIME
                        ,p_GROUP_SCH_COMPLETEDTIME
                        ,p_GROUP_SCH_ONDEMAND
                        ,p_GROUP_SCH_REQUESTSOURCE
                );
                Else
                Insert into CSM_GROUP_SCHEDULE
                (
                      GROUP_ID
                      ,GROUP_SCH_TIME
                      ,GROUP_SCH_ACTION
                      ,GROUP_SCH_STATUS
                      ,GROUP_SCH_COMMENTS
                      ,GROUP_SCH_CREATED_BY
                      ,GROUP_SCH_CREATED_DATETIME
                      ,GROUP_SCH_ONDEMAND
                      ,GROUP_SCH_REQUESTSOURCE
                )
                values
                (
                        v_GROUP_ID
                        ,p_GROUP_SCH_TIME
                        ,p_GROUP_SCH_ACTION
                        ,p_GROUP_SCH_STATUS
                        ,p_GROUP_SCH_COMMENTS	
                        ,p_GROUP_SCH_CREATED_BY
                        ,p_GROUP_SCH_CREATED_DATETIME
                        ,p_GROUP_SCH_ONDEMAND
                        ,p_GROUP_SCH_REQUESTSOURCE
                ) RETURNING GROUP_SCH_ID INTO p_SCOPE_OUTPUT;
                End if;
                --p_SCOPE_OUTPUT := IDENT_CURRENT('CSM_GROUP_SCHEDULE');
                v_tempGroupScheduleID := p_SCOPE_OUTPUT;
            End if;
            
            if p_CONFIG_IDS is not null
            Then	
                update CSM_GROUP_SCHEDULE_DETAIL set GROUP_SCH_ISACTIVE = 0
                where GROUP_ID = v_GROUP_ID
                and GROUP_SCH_ID = v_tempGroupScheduleID
                and GROUP_SCH_STATUS = 'O';
                --IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
                --BEGIN
                --	SET @Input = @Input + @Character
                --END	
                
                WHILE INSTR(v_Input, v_Character) > 0
                Loop
                    v_tempGroupDetailsID := 0;
                    v_EndIndex := INSTR(v_Input, v_Character);
                    v_tempConfigID := SUBSTR(v_Input, v_StartIndex, v_EndIndex - 1);
                    --Check whether the current value is there in the tavle already or not
                    select count(*) into v_count from CSM_GROUP_SCHEDULE_DETAIL 
                        where CONFIG_ID = v_tempConfigID
                            and GROUP_ID = v_GROUP_ID
                            and GROUP_SCH_STATUS = 'O'
                            and GROUP_SCH_ID = v_tempGroupScheduleID;
                    
                    if (v_count > 0) then
                        select GROUP_SERVICE_SCH_ID into v_tempGroupDetailsID from CSM_GROUP_SCHEDULE_DETAIL 
                            where CONFIG_ID = v_tempConfigID
                                and GROUP_ID = v_GROUP_ID
                                and GROUP_SCH_STATUS = 'O'
                                and GROUP_SCH_ID = v_tempGroupScheduleID;
                    else
                        v_tempGroupDetailsID := 0;
                    end if;
                    
                    if v_tempGroupDetailsID > 0 
                    Then
                        update CSM_GROUP_SCHEDULE_DETAIL set GROUP_SCH_ISACTIVE = 1
                            where GROUP_SERVICE_SCH_ID = v_tempGroupDetailsID;
                    else
                        Insert into CSM_GROUP_SCHEDULE_DETAIL
                        (
                            GROUP_SCH_ID
                            ,GROUP_ID
                            ,ENV_ID
                            ,CONFIG_ID
                            ,GROUP_SCH_ISACTIVE
                            ,GROUP_SCH_STATUS
                            ,GROUP_SCH_UPDATEDTIME
                            ,GROUP_SCH_RESULT
                            --,GROUP_SCH_SERVICE_STARTTIME
                            --,p_GROUP_SCH_SERVICE_COMPLETEDT
                        )
                        values
                        (
                            --(select isnull(MAX(GROUP_SCH_ID),1) from dbo.CSM_GROUP_SCHEDULE)
                            v_tempGroupScheduleID
                            ,v_GROUP_ID
                            ,(select cenv.ENV_ID from CSM_CONFIGURATION cenv where cenv.CONFIG_ID = v_tempConfigID)
                            ,v_tempConfigID
                            ,p_GROUP_SCH_ISACTIVE
                            ,p_GROUP_SCH_STATUS
                            ,p_GROUP_SCH_CREATED_DATETIME
                            ,p_GROUP_SCH_COMPLETESTATUS
                            --,(Case p_GROUP_SCH_ONDEMAND when 1 then p_GROUP_SCH_SERVICE_STARTTIME else null end )
                            --,(Case p_GROUP_SCH_ONDEMAND when 1 then p_GROUP_SCH_SERVICE_COMPLETEDT else null end) 
                        );
                    End if;
                    v_Input := SUBSTR(v_Input, v_EndIndex + 1, LENGTH(RTRIM(v_Input)));
                    
                End LOOP;
            End if;
        End if;        
    
    End;
  
    FUNCTION FN_CWT_GetWinServiceDetails
                                        (
                                            p_ENV_ID number 
                                        ) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        
            if p_ENV_ID > 0
            Then
                open CUR for select 
                    CSM_ENVIRONEMENT.ENV_ID
                    ,CSM_ENVIRONEMENT.ENV_NAME
                    ,CSM_CONFIGURATION.CONFIG_ID
                    ,CSM_CONFIGURATION.CONFIG_HOST_IP
                    ,CSM_CONFIGURATION.CONFIG_SERVICE_TYPE
                    ,CSM_CONFIGURATION.CONFIG_PORT_NUMBER
                    ,CSM_CONFIGURATION.CONFIG_LOCATION
                    ,CSM_CONFIGURATION.CONFIG_ISPRIMARY
                    ,CSM_WINDOWS_SERVICES.WIN_SERVICE_ID
                    ,CSM_WINDOWS_SERVICES.WIN_SERVICENAME
                    ,CSM_WINDOWS_SERVICES.WIN_COMMENTS
                    ,CSM_WINDOWS_SERVICES.WIN_CREATED_BY
                    ,CSM_WINDOWS_SERVICES.WIN_CREATED_DATE
                    ,CSM_WINDOWS_SERVICES.WIN_UPDATED_BY
                    ,CSM_WINDOWS_SERVICES.WIN_UPDATED_DATE
        
                from CSM_CONFIGURATION
                    left join CSM_WINDOWS_SERVICES on CSM_WINDOWS_SERVICES.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID 
                        and CSM_WINDOWS_SERVICES.ENV_ID = CSM_CONFIGURATION.ENV_ID
                    inner join CSM_ENVIRONEMENT on CSM_CONFIGURATION.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
                where CSM_CONFIGURATION.CONFIG_IS_ACTIVE = 1
                        and CSM_CONFIGURATION.CONFIG_ISPRIMARY = 1
                        and CSM_CONFIGURATION.ENV_ID = p_ENV_ID;
                else if(p_ENV_ID<=0)
                Then
                open CUR for select 
                    CSM_ENVIRONEMENT.ENV_ID
                    ,CSM_ENVIRONEMENT.ENV_NAME
                    ,CSM_CONFIGURATION.CONFIG_ID
                    ,CSM_CONFIGURATION.CONFIG_HOST_IP
                    ,CSM_CONFIGURATION.CONFIG_SERVICE_TYPE
                    ,CSM_CONFIGURATION.CONFIG_PORT_NUMBER
                    ,CSM_CONFIGURATION.CONFIG_LOCATION
                    ,CSM_CONFIGURATION.CONFIG_ISPRIMARY
                    ,CSM_WINDOWS_SERVICES.WIN_SERVICE_ID
                    ,CSM_WINDOWS_SERVICES.WIN_SERVICENAME
                    ,CSM_WINDOWS_SERVICES.WIN_COMMENTS
                    ,CSM_WINDOWS_SERVICES.WIN_CREATED_BY
                    ,CSM_WINDOWS_SERVICES.WIN_CREATED_DATE
                    ,CSM_WINDOWS_SERVICES.WIN_UPDATED_BY
                    ,CSM_WINDOWS_SERVICES.WIN_UPDATED_DATE
        
                from CSM_CONFIGURATION
                    left join CSM_WINDOWS_SERVICES on CSM_WINDOWS_SERVICES.CONFIG_ID = CSM_CONFIGURATION.CONFIG_ID 
                        and CSM_WINDOWS_SERVICES.ENV_ID = CSM_CONFIGURATION.ENV_ID
                    inner join CSM_ENVIRONEMENT on CSM_CONFIGURATION.ENV_ID = CSM_ENVIRONEMENT.ENV_ID
                where CSM_CONFIGURATION.CONFIG_IS_ACTIVE = 1
                        and CSM_CONFIGURATION.CONFIG_ISPRIMARY = 1
                        and CSM_CONFIGURATION.ENV_ID = p_ENV_ID;
                End if;
            end if;
            
            RETURN CUR;    
    End;
    
    FUNCTION FN_CWT_GetUrlPerformance(p_ENVID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if p_ENVID > 0
        Then
            open CUR for select uc.URL_ID
                      ,uc.ENV_ID
                      ,env.ENV_NAME
                      ,uc.URL_TYPE
                      ,uc.URL_ADDRESS
                      ,uc.URL_DISPLAYNAME
                      ,(select TO_CHAR(pm.PMON_RESPONSETIME/1000.0, '999.99') PMON_RESPONSETIME	
                        from CSM_PORTALMONITOR pm 
                        where pm.ENV_ID = uc.ENV_ID
                        and pm.URL_ID in (uc.URL_ID)
                        and pm.PMON_RESPONSETIME > 0
                        and rownum = 1
                        --order by PMON_RESPONSETIME desc
                        ) PMON_CREATEDDATE
                        ,(select TO_CHAR(avg(pm.PMON_RESPONSETIME)/1000.0, '999.99') from CSM_PORTALMONITOR pm 
                        where pm.PMON_RESPONSETIME > 0 and 
                        pm.PMON_CREATEDDATE >= interval '-1' hour +SYSTIMESTAMP 
                        and  pm.PMON_CREATEDDATE <= SYSTIMESTAMP
                        and pm.ENV_ID = uc.ENV_ID
                        and pm.URL_ID in (uc.URL_ID)) 	RESPONSETIMEINHOUR
                        ,(select max(pm.PMON_CREATEDDATE) from CSM_PORTALMONITOR pm
                        where  pm.ENV_ID = uc.ENV_ID
                        and pm.URL_ID in (uc.URL_ID)) LASTPINGDATETIME
            from CSM_URLCONFIGURATION uc
            left join CSM_ENVIRONEMENT env
                  On env.ENV_ID = uc.ENV_ID
                  where uc.ENV_ID = p_ENVID;
                  
        else
            open CUR for select uc.URL_ID
                      ,uc.ENV_ID
                      ,env.ENV_NAME
                      ,uc.URL_TYPE
                      ,uc.URL_ADDRESS
                      ,uc.URL_DISPLAYNAME
                      ,(select TO_CHAR(pm.PMON_RESPONSETIME/1000.0, '999.99')	
                        from CSM_PORTALMONITOR pm 
                        where pm.ENV_ID = uc.ENV_ID
                        and pm.URL_ID in (uc.URL_ID)
                        and pm.PMON_RESPONSETIME > 0
                        --order by pm.PMON_CREATEDDATE desc
                        ) RESPONSETIME
                      ,(select TO_CHAR(avg(pm.PMON_RESPONSETIME)/1000.0, '999.99') from CSM_PORTALMONITOR pm 
                        where pm.PMON_RESPONSETIME > 0 and 
                        pm.PMON_CREATEDDATE >= interval '-1' hour +SYSTIMESTAMP 
                        and  pm.PMON_CREATEDDATE <= SYSTIMESTAMP
                        and pm.ENV_ID = uc.ENV_ID
                        and pm.URL_ID in (uc.URL_ID)) 	RESPONSETIMEINHOUR,
                        (select max(pm.PMON_CREATEDDATE) from CSM_PORTALMONITOR pm
                        where  pm.ENV_ID = uc.ENV_ID
                        and pm.URL_ID in (uc.URL_ID)) LASTPINGDATETIME 
            from CSM_URLCONFIGURATION uc
            left join CSM_ENVIRONEMENT env
                  On env.ENV_ID = uc.ENV_ID;
        End if;    
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetUrlPerfLast24Hrs(p_ENVID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if p_ENVID > 0
        Then
            OPEN CUR FOR SELECT TO_CHAR(sum(pm.PMON_RESPONSETIME)/1000.0,'999.99') TOTALRT, TO_CHAR(avg(pm.PMON_RESPONSETIME)/1000.0,'999.99') AVGRT, TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'hh')) HOURRT,TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'DD')) DATERT
              from CSM_PORTALMONITOR pm
              where pm.PMON_RESPONSETIME > 0
              and pm.PMON_CREATEDDATE >= SYSTIMESTAMP - 23/24
              and  pm.PMON_CREATEDDATE <= SYSTIMESTAMP
              and pm.ENV_ID = p_ENVID
              GROUP BY TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'hh')),TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'DD'))
                      order by DATERT, HOURRT;
    
        else
            OPEN CUR FOR SELECT TO_CHAR(sum(pm.PMON_RESPONSETIME)/1000.0,'999.99') TOTALRT, TO_CHAR(avg(pm.PMON_RESPONSETIME)/1000.0,'999.99') AVGRT, TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'hh')) HOURRT,TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'DD')) DATERT
              from CSM_PORTALMONITOR pm
              where pm.PMON_RESPONSETIME > 0
              and pm.PMON_CREATEDDATE >= SYSTIMESTAMP - 23/24
              and  pm.PMON_CREATEDDATE <= SYSTIMESTAMP
              GROUP BY TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'hh')),TO_NUMBER(TO_CHAR(pm.PMON_CREATEDDATE, 'DD'))
              order by DATERT, HOURRT;
    
        End if; 
        RETURN CUR;
    End;

    FUNCTION FN_CWT_GetGroup(p_GRP_ID number)  RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if p_GRP_ID =0 or p_GRP_ID is null
        Then
            Open CUR FOR Select 
                grp.GROUP_ID, 
                grp.GROUP_NAME,
                grp.GROUP_COMMENTS 
                from CSM_GROUP grp
                where grp.GROUP_IS_ACTIVE = 1
                    and grp.GROUP_ID in 
                    (
                        select schedule.GROUP_ID from CSM_GROUP_SCHEDULE schedule 
                        where 
                            (schedule.GROUP_SCH_TIME <= SYSTIMESTAMP or schedule.GROUP_SCH_TIME is null)
                            --and schedule.GROUP_SCH_STATUS <> 'O'
                    )
                    and grp.GROUP_NAME <> 'OnDemand';
                    
            
        else if p_GRP_ID > 0
        Then
            Open CUR FOR Select 
                grp.GROUP_ID, 
                grp.GROUP_NAME,
                grp.GROUP_COMMENTS 
                from CSM_GROUP grp
                where GROUP_IS_ACTIVE = 1
                    and grp.GROUP_ID in 
                    (
                        select schedule.GROUP_ID from CSM_GROUP_SCHEDULE schedule 
                        where 
                            (schedule.GROUP_SCH_TIME <= SYSTIMESTAMP or schedule.GROUP_SCH_TIME is null)
                            --and schedule.GROUP_SCH_STATUS <> 'O'
                    )
                    and grp.GROUP_ID = p_GRP_ID
                    and grp.GROUP_NAME <> 'OnDemand';
        end if;   
        end if;
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetGroupDetail(	p_GROUP_ID number, p_ENV_ID number)  RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if p_ENV_ID  > 0
        Then
          open CUR for select 
            cgd.GROUP_DETAIL_ID
            ,cgd.GROUP_ID 
            ,cg.GROUP_NAME
            ,cgd.ENV_ID
            ,env.ENV_NAME
            ,cgd.CONFIG_ID
            ,cgd.GROUP_DETAIL_COMMENTS
            ,con.CONFIG_HOST_IP
            ,con.CONFIG_PORT_NUMBER
            ,con.CONFIG_ISPRIMARY
            ,con.CONFIG_LOCATION
            ,con.CONFIG_SERVICE_TYPE
            ,cws.WIN_SERVICE_ID
            ,cws.WIN_SERVICENAME
            ,usr.USER_FIRST_NAME USERFIRSTNAME
            ,usr.USER_LAST_NAME USERLASTNAME
            from CSM_GROUP_DETAIL cgd
            inner join CSM_GROUP cg on cg.GROUP_ID = cgd.GROUP_ID
            inner join CSM_CONFIGURATION con on cgd.CONFIG_ID = con.CONFIG_ID
            inner join CSM_ENVIRONEMENT env on env.ENV_ID = cgd.ENV_ID
            left outer join CSM_WINDOWS_SERVICES cws on cws.CONFIG_ID = cgd.CONFIG_ID and cws.ENV_ID = con.ENV_ID
            join CSM_USER usr on usr.USER_ID = CONFIG_CREATED_BY
            where 
                cgd.GROUP_ISACTIVE = 1
                and cgd.GROUP_ID = p_GROUP_ID
                and cgd.ENV_ID = p_ENV_ID
                and con.CONFIG_ISPRIMARY = 1
            order by cg.GROUP_ID, con.CONFIG_HOST_IP, con.CONFIG_PORT_NUMBER;
        else
          open CUR for select 
            cgd.GROUP_DETAIL_ID
            ,cgd.GROUP_ID 
            ,cg.GROUP_NAME
            ,cgd.ENV_ID
            ,env.ENV_NAME
            ,cgd.CONFIG_ID
            ,cgd.GROUP_DETAIL_COMMENTS
            ,con.CONFIG_HOST_IP
            ,con.CONFIG_PORT_NUMBER
            ,con.CONFIG_ISPRIMARY
            ,con.CONFIG_LOCATION
            ,con.CONFIG_SERVICE_TYPE
            ,cws.WIN_SERVICE_ID
            ,cws.WIN_SERVICENAME
            ,usr.USER_FIRST_NAME USERFIRSTNAME
            ,usr.USER_LAST_NAME USERLASTNAME
            from CSM_GROUP_DETAIL cgd
            inner join CSM_GROUP cg on cg.GROUP_ID = cgd.GROUP_ID
            inner join CSM_CONFIGURATION con on cgd.CONFIG_ID = con.CONFIG_ID
            inner join CSM_ENVIRONEMENT env on env.ENV_ID = cgd.ENV_ID
            left outer join CSM_WINDOWS_SERVICES cws on cws.CONFIG_ID = cgd.CONFIG_ID
            join CSM_USER usr on usr.USER_ID = CONFIG_CREATED_BY
            where 
                cgd.GROUP_ISACTIVE = 1
                and cgd.GROUP_ID = p_GROUP_ID
                and con.CONFIG_ISPRIMARY = 1	
            order by cg.GROUP_ID, con.CONFIG_HOST_IP, con.CONFIG_PORT_NUMBER;
        End if;    
        return cur;
    End;
    
    FUNCTION FN_CWT_GetGroupID(p_GROUP_NAME varchar) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        Open cur FOR Select 
		GROUP_ID, 
		GROUP_NAME,
		GROUP_COMMENTS 
		from CSM_GROUP 
		where GROUP_IS_ACTIVE = 1
			and GROUP_NAME = p_GROUP_NAME;
        RETURN CUR;
    End;
    
    FUNCTION FN_CWT_GetGroupSchedule(p_GROUP_ID number, p_GROUP_SCH_ID number, p_CURRENTDATETIME timestamp) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
		if p_GROUP_SCH_ID > 0 
		Then
			OPEN CUR FOR SELECT  
				   grpSch.GROUP_SCH_ID
				  ,grp.GROUP_ID
				  ,grp.GROUP_NAME
				  ,grp.GROUP_COMMENTS
				  ,TO_CHAR (grpSch.GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
				  ,grpSch.GROUP_SCH_ACTION
				  ,grpSch.GROUP_SCH_STATUS
				  ,TO_CHAR (grpSch.GROUP_SCH_COMPLETED_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
				  ,grpSch.GROUP_SCH_COMMENTS
				  ,grpSch.GROUP_SCH_CREATED_BY
				  ,grpSch.GROUP_SCH_CREATED_DATETIME
				  ,grpSch.GROUP_SCH_UPDATED_BY
				  ,grpSch.GROUP_SCH_UPDATED_DATETIME			  
				  ,grpSch.GROUP_SCH_REQUESTSOURCE
				  ,usr.USER_FIRST_NAME USERFIRSTNAME
				  ,usr.USER_LAST_NAME USERLASTNAME
			  FROM CSM_GROUP_SCHEDULE grpSch
			  right outer join CSM_GROUP grp on grp.GROUP_ID = grpSch.GROUP_ID
					and GROUP_SCH_TIME >= TO_TIMESTAMP(to_char(p_CURRENTDATETIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
					and grp.GROUP_IS_ACTIVE = 1
					and grpSch.GROUP_SCH_ISACTIVE = 1
					and GROUP_SCH_ID in
					(
						select GROUP_SCH_ID from CSM_GROUP_SCHEDULE_DETAIL 
						where CONFIG_ID in
						(
							select CONFIG_ID from CSM_CONFIGURATION 
							where CONFIG_IS_ACTIVE = 1
						)
					)
				left join CSM_USER usr on usr.USER_ID = grpSch.GROUP_SCH_CREATED_BY
				where GROUP_SCH_ID = p_GROUP_SCH_ID
				and grp.GROUP_NAME <> 'OnDemand';
		else
			if p_GROUP_ID > 0
			Then
				OPEN CUR FOR SELECT  
					   grpSch.GROUP_SCH_ID
					  ,grp.GROUP_ID
					  ,grp.GROUP_NAME
					  ,grp.GROUP_COMMENTS
					  ,TO_CHAR (grpSch.GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
					  ,grpSch.GROUP_SCH_ACTION
					  ,grpSch.GROUP_SCH_STATUS
					  ,TO_CHAR (grpSch.GROUP_SCH_COMPLETED_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
					  ,GROUP_SCH_COMMENTS
					  ,grpSch.GROUP_SCH_CREATED_BY
					  ,grpSch.GROUP_SCH_CREATED_DATETIME
					  ,grpSch.GROUP_SCH_UPDATED_BY
					  ,grpSch.GROUP_SCH_UPDATED_DATETIME	
					  ,grpSch.GROUP_SCH_REQUESTSOURCE		  
					  ,usr.USER_FIRST_NAME USERFIRSTNAME
					  ,usr.USER_LAST_NAME USERLASTNAME
				  FROM CSM_GROUP_SCHEDULE grpSch
				  right outer join CSM_GROUP grp on grp.GROUP_ID = grpSch.GROUP_ID
						and GROUP_SCH_TIME >=  TO_TIMESTAMP(to_char(p_CURRENTDATETIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
						and grp.GROUP_IS_ACTIVE = 1	
						and grpSch.GROUP_SCH_ISACTIVE = 1
						and GROUP_SCH_ID in
						(
							select GROUP_SCH_ID from CSM_GROUP_SCHEDULE_DETAIL 
							where CONFIG_ID in
							(
								select CONFIG_ID from CSM_CONFIGURATION 
								where CONFIG_IS_ACTIVE = 1
							)
						)
					left join CSM_USER usr on usr.USER_ID = grpSch.GROUP_SCH_CREATED_BY					
					where grp.GROUP_ID = p_GROUP_ID
					and grp.GROUP_NAME <> 'OnDemand';
			else
				OPEN CUR FOR SELECT  
					   grpSch.GROUP_SCH_ID
					  ,grp.GROUP_ID
					  ,grp.GROUP_NAME
					  ,grp.GROUP_COMMENTS
					  ,TO_CHAR (grpSch.GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
					  ,grpSch.GROUP_SCH_ACTION
					  ,grpSch.GROUP_SCH_STATUS
					  ,TO_CHAR (grpSch.GROUP_SCH_COMPLETED_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
					  ,GROUP_SCH_COMMENTS
					  ,grpSch.GROUP_SCH_CREATED_BY
					  ,grpSch.GROUP_SCH_CREATED_DATETIME
					  ,grpSch.GROUP_SCH_UPDATED_BY
					  ,grpSch.GROUP_SCH_UPDATED_DATETIME	
					  ,grpSch.GROUP_SCH_REQUESTSOURCE
					  ,usr.USER_FIRST_NAME USERFIRSTNAME
					  ,usr.USER_LAST_NAME USERLASTNAME
				  FROM CSM_GROUP_SCHEDULE grpSch
				  right OUTER join CSM_GROUP grp on grp.GROUP_ID = grpSch.GROUP_ID
						and GROUP_SCH_TIME >=  TO_TIMESTAMP(to_char(p_CURRENTDATETIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
						and grp.GROUP_IS_ACTIVE = 1	
						and grpSch.GROUP_SCH_ISACTIVE = 1
						and grpSch.GROUP_SCH_STATUS <> 'C'
						and GROUP_SCH_ID in
						(
							select GROUP_SCH_ID from CSM_GROUP_SCHEDULE_DETAIL 
							where CONFIG_ID in
							(
								select CONFIG_ID from CSM_CONFIGURATION 
								where CONFIG_IS_ACTIVE = 1
							)
						)
					left join CSM_USER usr on usr.USER_ID = grpSch.GROUP_SCH_CREATED_BY
					where grp.GROUP_NAME <> 'OnDemand';
			End if;	
		End if;						
        
        RETURN CUR;
    End;
 
     FUNCTION FN_CWT_GetGroupSchServiceDet(   p_GROUP_SCH_ID number
                                                    ,p_Category varchar2
                                                    ,p_ENV_ID number
                                                    ,p_GROUP_SCH_STATUS char) RETURN o_Cursor IS
     CUR o_Cursor;
     Begin
		if p_Category = 'env'
		Then
			OPEN CUR FOR SELECT  distinct cgs.GROUP_SCH_ID
				  ,cgs.GROUP_ID
				  ,cgsd.ENV_ID
				  ,env.ENV_NAME

			  FROM CSM_GROUP_SCHEDULE cgs
			  join CSM_GROUP_SCHEDULE_DETAIL cgsd on cgsd.GROUP_SCH_ID = cgs.GROUP_SCH_ID
			  --join CSM_CONFIGURATION con on con.CONFIG_ID = cgsd.CONFIG_ID
			  join CSM_ENVIRONEMENT env on env.ENV_ID = cgsd.ENV_ID
			  join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = cgsd.CONFIG_ID
			  where cgs.GROUP_SCH_ID = p_GROUP_SCH_ID;
					--and con.CONFIG_IS_ACTIVE = 1
					--and cgsd.ENV_ID = @ENV_ID
		else if p_Category = 'cfg'
		Then
			OPEN CUR FOR SELECT  cgs.GROUP_SCH_ID
				  ,cgs.GROUP_ID
				  ,TO_CHAR (cgs.GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
				  ,cgs.GROUP_SCH_ACTION
				  ,cgs.GROUP_SCH_STATUS
				  ,TO_CHAR (cgs.GROUP_SCH_COMPLETED_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
				  ,cgs.GROUP_SCH_COMMENTS
				  ,cgs.GROUP_SCH_CREATED_BY
				  ,TO_CHAR (cgs.GROUP_SCH_CREATED_DATETIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_CREATED_DATETIME
				  ,cgs.GROUP_SCH_UPDATED_BY
				  ,TO_CHAR (cgs.GROUP_SCH_UPDATED_DATETIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_UPDATED_DATETIME
				  ,cgs.GROUP_SCH_REQUESTSOURCE
				  ,cgsd.GROUP_SERVICE_SCH_ID
				  ,cgsd.CONFIG_ID
				  ,cgsd.GROUP_SCH_ISACTIVE
				  ,cgsd.GROUP_SCH_STATUS
				  ,con.CONFIG_SERVICE_TYPE
				  ,con.CONFIG_HOST_IP
				  ,con.CONFIG_PORT_NUMBER
				  ,con.ENV_ID
				  ,env.ENV_NAME
				  ,win.WIN_SERVICE_ID
				  ,win.WIN_SERVICENAME
				  ,usr.USER_FIRST_NAME USERFIRSTNAME
				  ,usr.USER_LAST_NAME USERLASTNAME
			  FROM CSM_GROUP_SCHEDULE cgs
			  join CSM_GROUP_SCHEDULE_DETAIL cgsd on cgsd.GROUP_SCH_ID = cgs.GROUP_SCH_ID
			  join CSM_CONFIGURATION con on con.CONFIG_ID = cgsd.CONFIG_ID
			  join CSM_ENVIRONEMENT env on env.ENV_ID = cgsd.ENV_ID
			  join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
			  join CSM_USER usr on usr.USER_ID = cgs.GROUP_SCH_CREATED_BY
			  where cgs.GROUP_SCH_ID = p_GROUP_SCH_ID
					and env.ENV_ID = p_ENV_ID
					and cgsd.GROUP_SCH_STATUS = p_GROUP_SCH_STATUS;
					--and con.CONFIG_IS_ACTIVE = 'true'
		else if p_Category = 'sch'
		Then
			OPEN CUR FOR SELECT  cgs.GROUP_SCH_ID
				  ,cgs.GROUP_ID
				  ,TO_CHAR (cgs.GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
				  ,cgs.GROUP_SCH_ACTION
				  ,cgs.GROUP_SCH_STATUS
				  ,TO_CHAR (cgs.GROUP_SCH_COMPLETED_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
				  ,cgs.GROUP_SCH_COMMENTS
				  ,cgs.GROUP_SCH_CREATED_BY
				  ,TO_CHAR (cgs.GROUP_SCH_CREATED_DATETIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_CREATED_DATETIME
				  ,cgs.GROUP_SCH_UPDATED_BY
				  ,TO_CHAR (cgs.GROUP_SCH_UPDATED_DATETIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_UPDATED_DATETIME
				  ,cgs.GROUP_SCH_REQUESTSOURCE
				  ,grp.GROUP_NAME
				  ,usr.USER_FIRST_NAME USERFIRSTNAME
				  ,usr.USER_LAST_NAME USERLASTNAME
			  FROM CSM_GROUP_SCHEDULE cgs
			  join CSM_GROUP grp on grp.GROUP_ID = cgs.GROUP_ID
			  join CSM_USER usr on usr.USER_ID = cgs.GROUP_SCH_CREATED_BY
			  where cgs.GROUP_SCH_ID = p_GROUP_SCH_ID;
		end if;		
        end if;	
        end if;	
        
        RETURN CUR;
     End;

    FUNCTION FN_CWT_GetWSConfigOnDemand(p_WIN_SERVICE_ID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
		OPEN CUR FOR SELECT ws.WIN_SERVICE_ID
			  ,ws.ENV_ID
			  ,ws.CONFIG_ID
			  ,ws.WIN_SERVICENAME
			  ,env.ENV_ID
			  ,env.ENV_NAME
			  ,con.CONFIG_HOST_IP
			  ,con.CONFIG_PORT_NUMBER
			  ,con.CONFIG_SERVICE_TYPE
			  ,(case con.CONFIG_SERVICE_TYPE when '1' then 'Content Manager' when '2' then 'Dispatcher' end) SERVICETYPE
		  FROM CSM_WINDOWS_SERVICES ws
		  inner join CSM_CONFIGURATION con on con.CONFIG_ID = ws.CONFIG_ID
		  inner join CSM_ENVIRONEMENT env on env.ENV_ID = con.ENV_ID
		  where ws.WIN_SERVICE_ID = p_WIN_SERVICE_ID;
          
        RETURN CUR;
    End;
    
     FUNCTION FN_CWT_ReportRestartService(   p_SCHEDULETYPE varchar2
                                            ,p_STARTDATE TIMESTAMP
                                            ,p_ENDDATE TIMESTAMP) RETURN o_Cursor IS
    CUR o_Cursor;                                            
    Begin
		if lower(p_SCHEDULETYPE) = 'all' 
		Then
			OPEN CUR FOR SELECT 
				CSM_GROUP_SCHEDULE.GROUP_SCH_ID
			  ,CSM_GROUP_SCHEDULE.GROUP_ID
			  ,CSM_GROUP.GROUP_NAME
			  ,CSM_GROUP_SCHEDULE_DETAIL.ENV_ID
			  ,CSM_ENVIRONEMENT.ENV_NAME
			  ,CSM_CONFIGURATION.CONFIG_HOST_IP
			  ,CSM_CONFIGURATION.CONFIG_PORT_NUMBER
			  ,CSM_CONFIGURATION.CONFIG_DESCRIPTION
			  ,CSM_CONFIGURATION.CONFIG_LOCATION
			  ,(case CSM_CONFIGURATION.CONFIG_SERVICE_TYPE when '1' then 'Content Manager' else 'Dispatcher' End) CONFIG_SERVICE_TYPE
			  ,TO_CHAR (GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
			  ,case GROUP_SCH_ACTION when '1' then 'Start' when '2' then 'Stop' else 'Restart' End GROUP_SCH_ACTION
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS 
					when 'O' then 'Upcoming'
					when 'C' then 'Cancelled'
					when 'U' then 'Unsuccessful'
					else 'Completed'
			   end GROUP_SCH_TYPE
			  ,(case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS 
					when 'O' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Upcoming'
							when 'C' then 'Cancelled'
							when 'S' then 'Skipped'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'U' then 'Unsuccessful'
					when 'N' then 'No Action'
					when 'T' then 'Timed Out'
					when 'R' then 'Unknown'
					else 'Completed'
			   end) GROUP_SCH_DETAIL_TYPE
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS 
					when 'O' then 'Scheduled'
					when 'C' then 'Cancelled'
					when 'S' then 'Success'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
			   end GROUP_SCH_STATUS
			  ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS 
					when 'O' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Scheduled'
							when 'C' then 'Cancelled'
							when 'S' then 'Skipped'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'S' then 'Success'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
			   end GROUP_SCH_DETAIL_STATUS
			  ,GROUP_SCH_COMPLETED_TIME
			  ,GROUP_SCH_COMMENTS
			  ,GROUP_SCH_CREATED_BY
			  ,GROUP_SCH_CREATED_DATETIME
			  ,GROUP_SCH_UPDATED_BY
			  ,GROUP_SCH_UPDATED_DATETIME
			  ,CSM_USER.USER_FIRST_NAME || ' ' || CSM_USER.USER_LAST_NAME  USERNAME
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_ONDEMAND
				when 1 then 'Yes'
				else 'No'
			   end GROUP_SCH_ONDEMAND
			   ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_RESULT
					when 'N/A' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'C' then 'Cancelled'
							when 'O' then 'Scheduled'
							when 'S' then 'Skipped'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'Time Out' then 'Timed Out'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
					else CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_RESULT
				end GROUP_SCH_RESULT
				,CSM_GROUP_SCHEDULE.GROUP_SCH_REQUESTSOURCE	
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_UPDATEDTIME
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_SERVICE_STARTTIME
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_SERVICE_COMPTIME
			FROM CSM_GROUP_SCHEDULE
			inner join CSM_GROUP_SCHEDULE_DETAIL on CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_ID = CSM_GROUP_SCHEDULE.GROUP_SCH_ID
			inner join CSM_GROUP on CSM_GROUP.GROUP_ID = CSM_GROUP_SCHEDULE.GROUP_ID
			inner join CSM_ENVIRONEMENT on CSM_ENVIRONEMENT.ENV_ID = CSM_GROUP_SCHEDULE_DETAIL.ENV_ID
			inner join CSM_CONFIGURATION on CSM_CONFIGURATION.CONFIG_ID =  CSM_GROUP_SCHEDULE_DETAIL.CONFIG_ID
			inner join CSM_USER on CSM_USER.USER_ID = CSM_GROUP_SCHEDULE.GROUP_SCH_CREATED_BY
			WHERE 
				CSM_GROUP.GROUP_IS_ACTIVE = 1
				and (
					TO_TIMESTAMP(to_char(GROUP_SCH_TIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') between  
					TO_TIMESTAMP(to_char(p_STARTDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') and 
					TO_TIMESTAMP(to_char(p_ENDDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')	
					OR
					TO_TIMESTAMP(to_char(GROUP_SCH_UPDATEDTIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') between  
					TO_TIMESTAMP(to_char(p_STARTDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') and 
					TO_TIMESTAMP(to_char(p_ENDDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')	
					);
		Else if lower(p_SCHEDULETYPE) = 's' or lower(p_SCHEDULETYPE) = 'c'
		Then
			OPEN CUR FOR SELECT 
				CSM_GROUP_SCHEDULE.GROUP_SCH_ID
			  ,CSM_GROUP_SCHEDULE.GROUP_ID
			  ,CSM_GROUP.GROUP_NAME
			  ,CSM_GROUP_SCHEDULE_DETAIL.ENV_ID
			  ,CSM_ENVIRONEMENT.ENV_NAME
			  ,CSM_CONFIGURATION.CONFIG_HOST_IP
			  ,CSM_CONFIGURATION.CONFIG_PORT_NUMBER
			  ,CSM_CONFIGURATION.CONFIG_DESCRIPTION
			  ,CSM_CONFIGURATION.CONFIG_LOCATION
			  ,case CSM_CONFIGURATION.CONFIG_SERVICE_TYPE when '1' then 'Content Manager' else 'Dispatcher' End CONFIG_SERVICE_TYPE
			  ,TO_CHAR (GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
			  ,case GROUP_SCH_ACTION when '1' then 'Start' when '2' then 'Stop' else 'Restart' End GROUP_SCH_ACTION
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS 
					when 'O' then 'Upcoming'
					when 'C' then 'Cancelled'
					when 'U' then 'Unsuccessful'
					else 'Completed'
			   end GROUP_SCH_TYPE
			  ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS 
					when 'O' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Upcoming'
							when 'S' then 'Skipped'
							when 'C' then 'Cancelled'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'U' then 'Unsuccessful'
					when 'N' then 'No Action'
					when 'T' then 'Timed Out'
					when 'R' then 'Unknown'
					else 'Completed'
			   end GROUP_SCH_DETAIL_TYPE
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS 
					when 'O' then 'Scheduled'
					when 'C' then 'Cancelled'
					when 'S' then 'Success'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
			   end GROUP_SCH_STATUS
			  ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS 
					when 'O' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Scheduled'
							when 'S' then 'Skipped'
							when 'C' then 'Cancelled'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'S' then 'Success'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
			   end GROUP_SCH_DETAIL_STATUS
			  ,GROUP_SCH_COMPLETED_TIME
			  ,GROUP_SCH_COMMENTS
			  ,GROUP_SCH_CREATED_BY
			  ,GROUP_SCH_CREATED_DATETIME
			  ,GROUP_SCH_UPDATED_BY
			  ,GROUP_SCH_UPDATED_DATETIME
			  ,CSM_USER.USER_FIRST_NAME || ' ' || CSM_USER.USER_LAST_NAME USERNAME
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_ONDEMAND
				when 1 then 'Yes'
				else 'No'
			   end GROUP_SCH_ONDEMAND
			   ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_RESULT
					when 'N/A' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Scheduled'
							when 'S' then 'Skipped'
							when 'U' then 'Skipped'
							when 'C' then 'Cancelled'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'Time Out' then 'Timed Out'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
					else CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_RESULT
				end GROUP_SCH_RESULT
				,CSM_GROUP_SCHEDULE.GROUP_SCH_REQUESTSOURCE	
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_UPDATEDTIME
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_SERVICE_STARTTIME
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_SERVICE_COMPTIME
			FROM CSM_GROUP_SCHEDULE
			inner join CSM_GROUP_SCHEDULE_DETAIL on CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_ID = CSM_GROUP_SCHEDULE.GROUP_SCH_ID
			inner join CSM_GROUP on CSM_GROUP.GROUP_ID = CSM_GROUP_SCHEDULE.GROUP_ID
			inner join CSM_ENVIRONEMENT on CSM_ENVIRONEMENT.ENV_ID = CSM_GROUP_SCHEDULE_DETAIL.ENV_ID
			inner join CSM_CONFIGURATION on CSM_CONFIGURATION.CONFIG_ID =  CSM_GROUP_SCHEDULE_DETAIL.CONFIG_ID
			inner join CSM_USER on CSM_USER.USER_ID = CSM_GROUP_SCHEDULE.GROUP_SCH_CREATED_BY
			WHERE CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS = p_SCHEDULETYPE
				and CSM_GROUP.GROUP_IS_ACTIVE = 1
				and (
					TO_TIMESTAMP(to_char(GROUP_SCH_TIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') between  
					TO_TIMESTAMP(to_char(p_STARTDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')	 and 
					TO_TIMESTAMP(to_char(p_ENDDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')	
					OR
                    TO_TIMESTAMP(to_char(GROUP_SCH_UPDATEDTIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') between  
					TO_TIMESTAMP(to_char(p_STARTDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')	 and 
					TO_TIMESTAMP(to_char(p_ENDDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
					);				
		Else if lower(p_SCHEDULETYPE) = 'o'
		Then
			OPEN CUR FOR SELECT 
				CSM_GROUP_SCHEDULE.GROUP_SCH_ID
			  ,CSM_GROUP_SCHEDULE.GROUP_ID
			  ,CSM_GROUP.GROUP_NAME
			  ,CSM_GROUP_SCHEDULE_DETAIL.ENV_ID
			  ,CSM_ENVIRONEMENT.ENV_NAME
			  ,CSM_CONFIGURATION.CONFIG_HOST_IP
			  ,CSM_CONFIGURATION.CONFIG_PORT_NUMBER
			  ,CSM_CONFIGURATION.CONFIG_DESCRIPTION
			  ,CSM_CONFIGURATION.CONFIG_LOCATION
			  ,case CSM_CONFIGURATION.CONFIG_SERVICE_TYPE when '1' then 'Content Manager' else 'Dispatcher' End CONFIG_SERVICE_TYPE
			  ,TO_CHAR (GROUP_SCH_TIME, 'mm/dd/yyyy HH24:MI:SS') GROUP_SCH_TIME
			  ,case GROUP_SCH_ACTION when '1' then 'Start' when '2' then 'Stop' else 'Restart' End GROUP_SCH_ACTION
					  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS 
					when 'O' then 'Upcoming'
					when 'C' then 'Cancelled'
					when 'U' then 'Unsuccessful'
					else 'Completed'
			   end GROUP_SCH_TYPE
			  ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS 
					when 'O' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Upcoming'
							when 'S' then 'Skipped'
							when 'C' then 'Cancelled'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'U' then 'Unsuccessful'
					when 'N' then 'No Action'
					when 'T' then 'Timed Out'
					when 'R' then 'Unknown'
					else 'Completed'
			   end GROUP_SCH_DETAIL_TYPE
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS 
					when 'C' then 'Cancelled'
					when 'O' then 'Scheduled'
					when 'S' then 'Success'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
			   end GROUP_SCH_STATUS
			  ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS 
					when 'O' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Scheduled'
							when 'C' then 'Cancelled'
							when 'S' then 'Skipped'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'S' then 'Success'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
			   end GROUP_SCH_DETAIL_STATUS
			  ,GROUP_SCH_COMPLETED_TIME
			  ,GROUP_SCH_COMMENTS
			  ,GROUP_SCH_CREATED_BY
			  ,GROUP_SCH_CREATED_DATETIME
			  ,GROUP_SCH_UPDATED_BY
			  ,GROUP_SCH_UPDATED_DATETIME
			  ,CSM_USER.USER_FIRST_NAME || ' ' || CSM_USER.USER_LAST_NAME USERNAME
			  ,case CSM_GROUP_SCHEDULE.GROUP_SCH_ONDEMAND
				when 1 then 'Yes'
				else 'No'
			   end GROUP_SCH_ONDEMAND
			   ,case CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_RESULT
					when 'N/A' then 
						case CSM_GROUP_SCHEDULE.GROUP_SCH_STATUS
							when 'O' then 'Scheduled'
							when 'S' then 'Skipped'
							when 'C' then 'Cancelled'
							when 'U' then 'Skipped'
							when 'N' then 'No Action'
						else
							'Unknown'
						end
					when 'C' then 'Cancelled'
					when 'Time Out' then 'Timed Out'
					when 'U' then 'Unsuccess'
					when 'N' then 'No Action'
					else CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_RESULT
				end GROUP_SCH_RESULT
				,CSM_GROUP_SCHEDULE.GROUP_SCH_REQUESTSOURCE	
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_UPDATEDTIME
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_SERVICE_STARTTIME
				,CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_SERVICE_COMPTIME
			FROM CSM_GROUP_SCHEDULE
			inner join CSM_GROUP_SCHEDULE_DETAIL on CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_ID = CSM_GROUP_SCHEDULE.GROUP_SCH_ID
			inner join CSM_GROUP on CSM_GROUP.GROUP_ID = CSM_GROUP_SCHEDULE.GROUP_ID
			inner join CSM_ENVIRONEMENT on CSM_ENVIRONEMENT.ENV_ID = CSM_GROUP_SCHEDULE_DETAIL.ENV_ID
			inner join CSM_CONFIGURATION on CSM_CONFIGURATION.CONFIG_ID =  CSM_GROUP_SCHEDULE_DETAIL.CONFIG_ID
			inner join CSM_USER on CSM_USER.USER_ID = CSM_GROUP_SCHEDULE.GROUP_SCH_CREATED_BY
			WHERE CSM_GROUP_SCHEDULE_DETAIL.GROUP_SCH_STATUS = 'O'--@SCHEDULETYPE
				and CSM_GROUP.GROUP_IS_ACTIVE = 1
				and TO_TIMESTAMP(to_char(GROUP_SCH_TIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') >= Systimestamp;
				--and CONVERT(date, CONVERT(VARCHAR(10), GROUP_SCH_TIME, 101)) between  
				--	CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
		end if;
        end if;
        end if;
        
        RETURN CUR;
    End;
   
    
END COSMO_WINSERVICE_PACKAGE;
/

Exit;