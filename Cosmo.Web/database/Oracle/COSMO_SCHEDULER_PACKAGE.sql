
CREATE OR REPLACE PACKAGE "COSMO_SCHEDULER_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    procedure SP_CWT_InsUpdScheduler
                                (p_ENV_ID number,
                                    p_CONFIG_ID number,
                                    p_SCH_INTERVAL number,
                                    p_SCH_DURATION varchar2,
                                    p_SCH_REPEATS varchar2,
                                    p_SCH_STARTBY timestamp,
                                    p_SCH_ENDAS varchar2,
                                    p_SCH_END_OCCURANCE number,
                                    p_SCH_ENDBY timestamp,
                                    p_SCH_IS_ACTIVE number,
                                    p_SCH_CREATED_BY varchar2,
                                    p_SCH_CREATED_DATE timestamp,
                                    p_SCH_UPDATED_BY varchar2,
                                    p_SCH_UPDATED_DATE timestamp,
                                    p_SCH_COMMENTS varchar2
                                );

    PROCEDURE  SP_CWT_UpdateGroupSchStatus
                                        (   p_GROUP_SCH_ID number
                                            ,p_GROUP_SCH_STATUS char
                                            ,p_GROUP_SCH_COMPLETED_TIME timestamp
                                            ,p_GROUP_SCH_UPDATED_BY varchar2
                                            ,p_GROUP_SCH_UPDATED_DATETIME timestamp
                                        );

    PROCEDURE SP_CWT_UpdateServiceStatus(
                                    p_GROUP_SERVICE_SCH_ID number
                                    ,p_GROUP_SERVICE_SCH_STATUS char
                                    ,p_GROUP_SCH_UPDATEDTIME timestamp
                                    ,p_GROUP_SCH_SERVICE_STARTTIME timestamp
                                    ,p_GROUP_SCH_SERVICE_COMPTIME timestamp
                                    );

    FUNCTION FN_CWT_GetSchedulerDetails (p_ENVID number, p_CONFIGID number) RETURN o_Cursor;
    FUNCTION FN_GetGroupOpenSchDetails (
                                             p_CURRENTDATETIME timestamp
                                            ,p_SCHEDULE_STATUS char
                                            ,p_Category varchar2
                                            ,p_GROUP_SCH_ID number
                                            ,p_ENV_ID number) RETURN o_Cursor;
    
End COSMO_SCHEDULER_PACKAGE;
/

CREATE OR REPLACE PACKAGE BODY "COSMO_SCHEDULER_PACKAGE" AS

    procedure SP_CWT_InsUpdScheduler
                                (p_ENV_ID number,
                                    p_CONFIG_ID number,
                                    p_SCH_INTERVAL number,
                                    p_SCH_DURATION varchar2,
                                    p_SCH_REPEATS varchar2,
                                    p_SCH_STARTBY timestamp,
                                    p_SCH_ENDAS varchar2,
                                    p_SCH_END_OCCURANCE number,
                                    p_SCH_ENDBY timestamp,
                                    p_SCH_IS_ACTIVE number,
                                    p_SCH_CREATED_BY varchar2,
                                    p_SCH_CREATED_DATE timestamp,
                                    p_SCH_UPDATED_BY varchar2,
                                    p_SCH_UPDATED_DATE timestamp,
                                    p_SCH_COMMENTS varchar2
                                ) Is
    v_SCHID number:= 0; 
    v_CONFIG_ID_DISP number(10);
    v_count int := 0;
    Begin
        select count(*) into v_count from CSM_SCHEDULE where
		ENV_ID = p_ENV_ID AND
		CONGIG_ID	= p_CONFIG_ID;
        if v_count > 0 then
            select SCH_ID into v_SCHID from CSM_SCHEDULE where
            ENV_ID = p_ENV_ID AND
            CONGIG_ID	= p_CONFIG_ID;
        end if;
        
		-- to add scheduler details for dispatcher incase content manager available
		select count(*) into v_count from CSM_CONFIGURATION 
		where CONFIG_URL_ADDRESS like (select CONFIG_URL_ADDRESS from CSM_CONFIGURATION where CONFIG_ID=p_CONFIG_ID) || '%'
        and   CONFIG_URL_ADDRESS not like (select CONFIG_URL_ADDRESS from CSM_CONFIGURATION where CONFIG_ID=p_CONFIG_ID);
        
        if v_count > 0 then
            select CONFIG_ID into v_CONFIG_ID_DISP from CSM_CONFIGURATION 
            where CONFIG_URL_ADDRESS like (select CONFIG_URL_ADDRESS from CSM_CONFIGURATION where CONFIG_ID=p_CONFIG_ID) || '%'
            and   CONFIG_URL_ADDRESS not like (select CONFIG_URL_ADDRESS from CSM_CONFIGURATION where CONFIG_ID=p_CONFIG_ID)
            and CONFIG_IS_ACTIVE = 1;
        else
            v_CONFIG_ID_DISP := 0;
        end if;
        
        if v_SCHID is null or v_SCHID <= 0 
        Then
            insert into CSM_SCHEDULE (
               ENV_ID
              ,CONGIG_ID
              ,SCH_INTERVAL
              ,SCH_DURATION
              ,SCH_REPEATS
              ,SCH_STARTBY
              ,SCH_ENDAS
              ,SCH_END_OCCURANCE
              ,SCH_ENDBY
              ,SCH_IS_ACTIVE
              ,SCH_CREATED_BY
              ,SCH_CREATED_DATE
              ,SCH_COMMENTS
              )
              values
              (
               p_ENV_ID
              ,p_CONFIG_ID
              ,p_SCH_INTERVAL
              ,p_SCH_DURATION
              ,p_SCH_REPEATS
              ,p_SCH_STARTBY
              ,p_SCH_ENDAS
              ,p_SCH_END_OCCURANCE
              ,p_SCH_ENDBY
              ,p_SCH_IS_ACTIVE
              ,p_SCH_CREATED_BY
              ,p_SCH_CREATED_DATE
              ,p_SCH_COMMENTS
              );
              --to insert schedule detail for dispatch correponds to content manager
              if(v_CONFIG_ID_DISP is not null and v_CONFIG_ID_DISP >0)
              Then
                insert into CSM_SCHEDULE (
                   ENV_ID
                  ,CONGIG_ID
                  ,SCH_INTERVAL
                  ,SCH_DURATION
                  ,SCH_REPEATS
                  ,SCH_STARTBY
                  ,SCH_ENDAS
                  ,SCH_END_OCCURANCE
                  ,SCH_ENDBY
                  ,SCH_IS_ACTIVE
                  ,SCH_CREATED_BY
                  ,SCH_CREATED_DATE
                  ,SCH_COMMENTS
                  )
                  values
                  (
                   p_ENV_ID
                  ,v_CONFIG_ID_DISP
                  ,p_SCH_INTERVAL
                  ,p_SCH_DURATION
                  ,p_SCH_REPEATS
                  ,p_SCH_STARTBY
                  ,p_SCH_ENDAS
                  ,p_SCH_END_OCCURANCE
                  ,p_SCH_ENDBY
                  ,p_SCH_IS_ACTIVE
                  ,p_SCH_CREATED_BY
                  ,p_SCH_CREATED_DATE
                  ,p_SCH_COMMENTS
                  )		 ;
              End if;
         else
            Update CSM_SCHEDULE set 
               ENV_ID			=     p_ENV_ID
              ,CONGIG_ID		=	  p_CONFIG_ID
              ,SCH_INTERVAL	=	  p_SCH_INTERVAL
              ,SCH_DURATION	=	  p_SCH_DURATION
              ,SCH_REPEATS	=	  p_SCH_REPEATS
              ,SCH_STARTBY	=	  p_SCH_STARTBY
              ,SCH_ENDAS		=	  p_SCH_ENDAS
              ,SCH_END_OCCURANCE	= p_SCH_END_OCCURANCE
              ,SCH_ENDBY		=	  p_SCH_ENDBY
              ,SCH_IS_ACTIVE	=	  p_SCH_IS_ACTIVE
              ,SCH_UPDATED_BY	=	  p_SCH_CREATED_BY
              ,SCH_UPDATED_DATE	= p_SCH_CREATED_DATE
              ,SCH_COMMENTS	=	  p_SCH_COMMENTS
              where SCH_ID = v_SCHID;
    
              --to update schedule detail for dispatch correponds to content manager
              if(v_CONFIG_ID_DISP is not null and v_CONFIG_ID_DISP >0)
              Then
              
                    select count(*) into v_count from CSM_SCHEDULE where
                    ENV_ID = p_ENV_ID AND
                    CONGIG_ID	= v_CONFIG_ID_DISP;
                    
                    if v_count >0 then
                        select SCH_ID into v_SCHID from CSM_SCHEDULE where
                        ENV_ID = p_ENV_ID AND
                        CONGIG_ID	= v_CONFIG_ID_DISP;
        
                        Update CSM_SCHEDULE set 
                           ENV_ID			=     p_ENV_ID
                          ,CONGIG_ID		=	  v_CONFIG_ID_DISP
                          ,SCH_INTERVAL	=	  p_SCH_INTERVAL
                          ,SCH_DURATION	=	  p_SCH_DURATION
                          ,SCH_REPEATS	=	  p_SCH_REPEATS
                          ,SCH_STARTBY	=	  p_SCH_STARTBY
                          ,SCH_ENDAS		=	  p_SCH_ENDAS
                          ,SCH_END_OCCURANCE	= p_SCH_END_OCCURANCE
                          ,SCH_ENDBY		=	  p_SCH_ENDBY
                          ,SCH_IS_ACTIVE	=	  p_SCH_IS_ACTIVE
                          ,SCH_UPDATED_BY	=	  p_SCH_CREATED_BY
                          ,SCH_UPDATED_DATE	= p_SCH_CREATED_DATE
                          ,SCH_COMMENTS	=	  p_SCH_COMMENTS
                          where SCH_ID = v_SCHID;
                     End if;   
              End if;
         End if;
    End;
    
        PROCEDURE  SP_CWT_UpdateGroupSchStatus
                                        (   p_GROUP_SCH_ID number
                                            ,p_GROUP_SCH_STATUS char
                                            ,p_GROUP_SCH_COMPLETED_TIME timestamp
                                            ,p_GROUP_SCH_UPDATED_BY varchar2
                                            ,p_GROUP_SCH_UPDATED_DATETIME timestamp
                                        ) IS
    Begin
        if p_GROUP_SCH_COMPLETED_TIME is not null
        Then
            UPDATE CSM_GROUP_SCHEDULE set 
                GROUP_SCH_STATUS = p_GROUP_SCH_STATUS
                ,GROUP_SCH_COMPLETED_TIME = p_GROUP_SCH_COMPLETED_TIME
                ,GROUP_SCH_UPDATED_BY = p_GROUP_SCH_UPDATED_BY
                ,GROUP_SCH_UPDATED_DATETIME = p_GROUP_SCH_UPDATED_DATETIME
                Where GROUP_SCH_ID = p_GROUP_SCH_ID;
        Else
            UPDATE CSM_GROUP_SCHEDULE set 
                GROUP_SCH_STATUS = p_GROUP_SCH_STATUS
                ,GROUP_SCH_UPDATED_BY = p_GROUP_SCH_UPDATED_BY
                ,GROUP_SCH_UPDATED_DATETIME = p_GROUP_SCH_UPDATED_DATETIME
                Where GROUP_SCH_ID = p_GROUP_SCH_ID;
        End if;
    End;
    
    PROCEDURE SP_CWT_UpdateServiceStatus(
                                    p_GROUP_SERVICE_SCH_ID number
                                    ,p_GROUP_SERVICE_SCH_STATUS char
                                    ,p_GROUP_SCH_UPDATEDTIME timestamp
                                    ,p_GROUP_SCH_SERVICE_STARTTIME timestamp
                                    ,p_GROUP_SCH_SERVICE_COMPTIME timestamp
                                    ) IS
    v_tempCompleteStatus varchar2(50);                                    
    Begin
        if p_GROUP_SERVICE_SCH_STATUS = 'S' then
            v_tempCompleteStatus := 'Successful';
        else if p_GROUP_SERVICE_SCH_STATUS = 'T' then
            v_tempCompleteStatus := 'Timed out';
        else if p_GROUP_SERVICE_SCH_STATUS = 'C' then
            v_tempCompleteStatus := 'Cancelled';
        else if p_GROUP_SERVICE_SCH_STATUS = 'A' then
            v_tempCompleteStatus := 'Abonded';
        else if p_GROUP_SERVICE_SCH_STATUS = 'U' then
            v_tempCompleteStatus := 'Unsuccessful';		
        else if p_GROUP_SERVICE_SCH_STATUS = 'N' then
            v_tempCompleteStatus := 'Skipped';
        else
            v_tempCompleteStatus := 'Nothing';
        end if;    
        end if;    
        end if;    
        end if;    
        end if;    
        end if;    
    
        if p_GROUP_SCH_SERVICE_STARTTIME is not null
        Then
            UPDATE CSM_GROUP_SCHEDULE_DETAIL set 
                GROUP_SCH_STATUS = p_GROUP_SERVICE_SCH_STATUS
                ,GROUP_SCH_UPDATEDTIME = SYSTIMESTAMP
                ,GROUP_SCH_RESULT = v_tempCompleteStatus
                ,GROUP_SCH_SERVICE_STARTTIME = p_GROUP_SCH_SERVICE_STARTTIME
                ,GROUP_SCH_SERVICE_COMPTIME = p_GROUP_SCH_SERVICE_COMPTIME
                --,[ENV_ID] = @ENV_ID
                Where GROUP_SERVICE_SCH_ID = p_GROUP_SERVICE_SCH_ID;
        Else
            UPDATE CSM_GROUP_SCHEDULE_DETAIL set 
                GROUP_SCH_STATUS = p_GROUP_SERVICE_SCH_STATUS
                ,GROUP_SCH_RESULT = v_tempCompleteStatus
                --,[ENV_ID] = @ENV_ID
                Where GROUP_SERVICE_SCH_ID = p_GROUP_SERVICE_SCH_ID;
        End if;
        
    End;
    
                          
    FUNCTION  FN_CWT_GetSchedulerDetails (p_ENVID number, p_CONFIGID number) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if(p_ENVID > 0 and p_CONFIGID > 0)
        Then
            OPEN CUR FOR SELECT SCH_ID
              ,ENV_ID
              ,CONGIG_ID
              ,SCH_INTERVAL
              ,SCH_DURATION
              ,SCH_REPEATS
              ,SCH_STARTBY
              ,SCH_ENDAS
              ,SCH_END_OCCURANCE
              ,SCH_ENDBY
              ,SCH_IS_ACTIVE
              ,SCH_LASTJOBRAN_TIME
              ,SCH_NEXTJOBRAN_TIME
              ,SCH_CREATED_BY
              ,SCH_CREATED_DATE
              ,SCH_UPDATED_BY
              ,SCH_UPDATED_DATE
              ,SCH_COMMENTS
            FROM CSM_SCHEDULE
            Where ENV_ID = p_ENVID and CONGIG_ID = p_CONFIGID;
        else if(p_ENVID > 0 and p_CONFIGID <= 0)
        Then
            OPEN CUR FOR SELECT SCH_ID
              ,ENV_ID
              ,CONGIG_ID
              ,SCH_INTERVAL
              ,SCH_DURATION
              ,SCH_REPEATS
              ,SCH_STARTBY
              ,SCH_ENDAS
              ,SCH_END_OCCURANCE
              ,SCH_ENDBY
              ,SCH_IS_ACTIVE
              ,SCH_LASTJOBRAN_TIME
              ,SCH_NEXTJOBRAN_TIME
              ,SCH_CREATED_BY
              ,SCH_CREATED_DATE
              ,SCH_UPDATED_BY
              ,SCH_UPDATED_DATE
              ,SCH_COMMENTS
            FROM CSM_SCHEDULE
            Where ENV_ID = p_ENVID;
        End if;
        end if;  
        
        RETURN CUR;
    End;          
    
    FUNCTION FN_GetGroupOpenSchDetails (
                                             p_CURRENTDATETIME timestamp
                                            ,p_SCHEDULE_STATUS char
                                            ,p_Category varchar2
                                            ,p_GROUP_SCH_ID number
                                            ,p_ENV_ID number) RETURN o_Cursor IS
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
          --join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
          where cgsd.GROUP_SCH_ID = p_GROUP_SCH_ID
                --con.CONFIG_IS_ACTIVE = 1
                --and GROUP_SCH_TIME <= CONVERT(VARCHAR(19), @CURRENTDATETIME, 120)
                and cgs.GROUP_SCH_STATUS = p_SCHEDULE_STATUS
                and cgsd.GROUP_SCH_STATUS  = p_SCHEDULE_STATUS
                and cgs.GROUP_SCH_ISACTIVE = 1;
                
        else if p_Category = 'cfg'
        Then
        OPEN CUR FOR SELECT  cgs.GROUP_SCH_ID
              ,cgs.GROUP_ID
              ,TO_CHAR (cgs.GROUP_SCH_TIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_TIME
              ,cgs.GROUP_SCH_ACTION
              ,cgs.GROUP_SCH_STATUS
              ,TO_CHAR (cgs.GROUP_SCH_COMPLETED_TIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
              ,cgs.GROUP_SCH_COMMENTS
              ,cgs.GROUP_SCH_CREATED_BY
              ,TO_CHAR (cgs.GROUP_SCH_CREATED_DATETIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_CREATED_DATETIME
              ,cgs.GROUP_SCH_UPDATED_BY
              ,TO_CHAR (cgs.GROUP_SCH_UPDATED_DATETIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_UPDATED_DATETIME
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
              ,(cu.USER_FIRST_NAME || ' ' || cu.USER_LAST_NAME) USER_FIRST_NAME
          FROM CSM_GROUP_SCHEDULE cgs
          join CSM_GROUP_SCHEDULE_DETAIL cgsd on cgsd.GROUP_SCH_ID = cgs.GROUP_SCH_ID
          join CSM_CONFIGURATION con on con.CONFIG_ID = cgsd.CONFIG_ID
          join CSM_ENVIRONEMENT env on env.ENV_ID = con.ENV_ID
          join CSM_WINDOWS_SERVICES win on win.CONFIG_ID = con.CONFIG_ID
          join CSM_USER cu on cu.USER_ID = cgs.GROUP_SCH_CREATED_BY
          where cgsd.GROUP_SCH_ID = p_GROUP_SCH_ID
                and cgsd.ENV_ID = p_ENV_ID 
                and cgs.GROUP_SCH_STATUS = p_SCHEDULE_STATUS
                and cgsd.GROUP_SCH_STATUS  = p_SCHEDULE_STATUS
                and cgs.GROUP_SCH_ISACTIVE = 1
                and cgsd.GROUP_SCH_ISACTIVE = 1
                order by cgsd.GROUP_SERVICE_SCH_ID;
          
                --and GROUP_SCH_TIME <= CONVERT(VARCHAR(19), @CURRENTDATETIME, 120)
    
        else if p_Category = 'sch'
        Then
        OPEN CUR FOR SELECT  cgs.GROUP_SCH_ID
              ,cgs.GROUP_ID
              ,TO_CHAR (cgs.GROUP_SCH_TIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_TIME
              ,cgs.GROUP_SCH_ACTION
              ,cgs.GROUP_SCH_STATUS
              ,TO_CHAR (cgs.GROUP_SCH_COMPLETED_TIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_COMPLETED_TIME
              ,cgs.GROUP_SCH_COMMENTS
              ,cgs.GROUP_SCH_CREATED_BY
              ,TO_CHAR (cgs.GROUP_SCH_CREATED_DATETIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_CREATED_DATETIME
              ,cgs.GROUP_SCH_UPDATED_BY
              ,TO_CHAR (cgs.GROUP_SCH_UPDATED_DATETIME, 'MM/DD/YYYY HH24:MI:SS') GROUP_SCH_UPDATED_DATETIME
              ,cgs.GROUP_SCH_REQUESTSOURCE
              ,grp.GROUP_NAME
              ,(cu.USER_FIRST_NAME || ' ' || cu.USER_LAST_NAME) USER_FIRST_NAME
          FROM CSM_GROUP_SCHEDULE cgs
          join CSM_GROUP grp on grp.GROUP_ID = cgs.GROUP_ID
          join CSM_USER cu on cu.USER_ID = cgs.GROUP_SCH_CREATED_BY
          where 
                GROUP_SCH_TIME <= TO_TIMESTAMP(to_char(p_CURRENTDATETIME,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS')
                and GROUP_SCH_STATUS = p_SCHEDULE_STATUS
                and cgs.GROUP_SCH_ISACTIVE = 1;
    
        end if;    
        end if;    
        end if;    
        RETURN CUR;
    End;
    
    
End COSMO_SCHEDULER_PACKAGE;
/

Exit;