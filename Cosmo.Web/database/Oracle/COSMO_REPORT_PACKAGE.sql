CREATE OR REPLACE PACKAGE "COSMO_REPORT_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;

    FUNCTION FN_CWT_ReportBuildHistory( p_ENV_ID number
                                        ,p_STARTDATE TIMESTAMP
                                        ,p_ENDDATE TIMESTAMP) RETURN o_Cursor;
    
    FUNCTION FN_CWT_ReportServiceStatus(
                                        p_ENV_ID number
                                        ,p_STARTDATE TIMESTAMP
                                        ,p_ENDDATE TIMESTAMP
                                    )  RETURN o_Cursor;
    
END COSMO_REPORT_PACKAGE;
/

CREATE OR REPLACE PACKAGE BODY "COSMO_REPORT_PACKAGE" AS
    --CUR o_Cursor;
    FUNCTION FN_CWT_ReportBuildHistory( p_ENV_ID number
                                        ,p_STARTDATE TIMESTAMP
                                        ,p_ENDDATE TIMESTAMP) RETURN o_Cursor IS
    CUR o_Cursor;                                        
    Begin
    
        if p_ENV_ID > 0
        Then
            open CUR for select
                mon.CONFIG_ID,
                mon.BUILD_VERSION MON_COMMENTS,
                mon.BUILD_DATE MON_BUILD_DATE,
                TO_CHAR(mon.CREATED_DATE, 'MM/DD/YYYY HH24:MI:SS') MON_CREATED_DATE,
                case con.CONFIG_SERVICE_TYPE 
                    when '1' then 'Content Manager'
                    when '2' then 'Dispatcher'
                end 	CONFIG_SERVICE_TYPE
                ,con.CONFIG_PORT_NUMBER,
                con.CONFIG_DESCRIPTION,
                con.CONFIG_HOST_IP,
                con.CONFIG_LOCATION,
                con.CONFIG_ISPRIMARY
            from CSM_SERVICEBUILD mon
            inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
            where con.ENV_ID = p_ENV_ID
                --and CONVERT(date, CONVERT(VARCHAR(10), mon.CREATED_DATE, 101)) between  
                --	CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
                --and mon.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [Bootcosmo].[dbo].[CSM_SERVICEBUILD] group by CONFIG_ID)
            order by CONFIG_ID, MON_CREATED_DATE;
        else
            open CUR for select
                mon.CONFIG_ID,
                mon.BUILD_VERSION MON_COMMENTS,
                mon.BUILD_DATE MON_BUILD_DATE,
                TO_CHAR(mon.CREATED_DATE, 'MM/DD/YYYY HH24:MI:SS') MON_CREATED_DATE,
                case con.CONFIG_SERVICE_TYPE 
                    when '1' then 'Content Manager'
                    when '2' then 'Dispatcher'
                end 	CONFIG_SERVICE_TYPE,
                con.CONFIG_PORT_NUMBER,
                con.CONFIG_DESCRIPTION,
                con.CONFIG_HOST_IP,
                con.CONFIG_LOCATION,
                con.CONFIG_ISPRIMARY
                ,con.ENV_ID
            from CSM_SERVICEBUILD mon
            inner join CSM_CONFIGURATION con on con.CONFIG_ID = mon.CONFIG_ID
            --where 	CONVERT(date, CONVERT(VARCHAR(10), mon.CREATED_DATE, 101)) between  
            --		CONVERT(date,@STARTDATE) and CONVERT(date,@ENDDATE)
            --		and mon.CREATED_DATE in (select MAX(CREATED_DATE) CREATED_DATE from [Bootcosmo].[dbo].[CSM_SERVICEBUILD] group by CONFIG_ID)
            order by CONFIG_ID, MON_CREATED_DATE;
        End if;       
        
        RETURN CUR;
    End;

    FUNCTION FN_CWT_ReportServiceStatus(
                                        p_ENV_ID number
                                        ,p_STARTDATE TIMESTAMP
                                        ,p_ENDDATE TIMESTAMP
                                    )  RETURN o_Cursor IS
     CUR o_Cursor;                                        
    Begin
        OPEN cur FOR SELECT 
            mds.MON_TRACK_ID
            ,mds.MON_ID
            ,mds.ENV_ID
            ,mds.CONFIG_ID
            ,con.CONFIG_SERVICE_TYPE
            ,con.CONFIG_HOST_IP
            ,con.CONFIG_DESCRIPTION
            ,con.CONFIG_PORT_NUMBER
            ,con.CONFIG_DESCRIPTION
            ,TO_CHAR(mds.MON_TRACK_DATE,'MM/DD/YYYY') MON_CREATED_DATE --MON_TRACK_DATE
            ,case 
                when mds.MON_TRACK_STATUS = 'Running' and con.CONFIG_SERVICE_TYPE = '1' then 'A'
                when mds.MON_TRACK_STATUS = 'Running' and con.CONFIG_SERVICE_TYPE = '2' then 'R'
                when mds.MON_TRACK_STATUS = 'Standby' then 'S'
                when mds.MON_TRACK_STATUS = 'Stopped' then 'D'
                when mds.MON_TRACK_STATUS = 'Not Ready' then 'D'
                when mds.MON_TRACK_STATUS = 'Not running.  See logs.' then 'D'
                else 'N/A'
            end MON_STATUS --MON_TRACK_STATUS
            ,mds.MON_TRACK_COMMENTS MON_COMMENTS
          FROM CSM_MON_DAILY_STATUS mds
          inner join CSM_CONFIGURATION con on con.CONFIG_ID = mds.CONFIG_ID
          where mds.ENV_ID = p_ENV_ID
            and (TO_TIMESTAMP(to_char(mds.MON_TRACK_DATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') between
                TO_TIMESTAMP(to_char(p_STARTDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS') and
                TO_TIMESTAMP(to_char(p_ENDDATE,'MM/DD/YYYY HH24:MI:SS'),'MM/DD/YYYY HH24:MI:SS'))
            --and mds.MON_TRACK_DATE between p_STARTDATE and p_ENDDATE
            and con.CONFIG_IS_ACTIVE = 1
            and con.CONFIG_ISPRIMARY = 1
          order by 
            mds.ENV_ID, 
            mds.CONFIG_ID, 
            con.CONFIG_SERVICE_TYPE, 
            MON_TRACK_DATE;    
        RETURN CUR;
    End;

END COSMO_REPORT_PACKAGE;
/

Exit;