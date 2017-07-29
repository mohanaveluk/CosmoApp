CREATE OR REPLACE PACKAGE "COSMO_COMMON_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    PROCEDURE SP_CWT_InsUserAccess(p_ACCESS_CODE IN VARCHAR2);
    PROCEDURE SP_CWT_InsMailServer( p_ServerName varchar2
                                    ,p_ServerPort varchar2
                                    ,p_Username varchar2
                                    ,p_Password varchar2
                                    ,p_SSLEnable number
                                    ,p_IsActive number
                                    ,p_FromEmailId varchar2
                                );
    
    FUNCTION FN_CWT_GetUserAccess RETURN o_Cursor;
    FUNCTION FN_CWT_GetMailServerDetail  RETURN o_Cursor;

END COSMO_COMMON_PACKAGE;
/

create or replace PACKAGE BODY "COSMO_COMMON_PACKAGE" AS
    PROCEDURE SP_CWT_InsUserAccess(p_ACCESS_CODE IN VARCHAR2) IS
    BEGIN
        if p_ACCESS_CODE is not null Then
            insert into CSM_USER_ACCESS
            (
                ACCESS_CODE
                ,DATE_CREATED
            )
            values
            (
                p_ACCESS_CODE
                ,SYSTIMESTAMP
            );
        End if;    
    END;

    PROCEDURE SP_CWT_InsMailServer( p_ServerName varchar2
                                    ,p_ServerPort varchar2
                                    ,p_Username varchar2
                                    ,p_Password varchar2
                                    ,p_SSLEnable number
                                    ,p_IsActive number
                                    ,p_FromEmailId varchar2
                                ) IS
        v_EMSILSERVERID number(10);
    BEGIN
        v_EMSILSERVERID := 0;
        select EMAIL_SERVER_ID into v_EMSILSERVERID 
            FROM CSM_EMAIL_CONFIGURATION
            WHERE EMAIL_SERVER_NAME = p_ServerName
            and EMAIL_AUTH_USER_ID = p_Username;
        IF v_EMSILSERVERID <= 0 or v_EMSILSERVERID is null
        THEN
            delete from CSM_EMAIL_CONFIGURATION;
            INSERT INTO CSM_EMAIL_CONFIGURATION 
            (
              EMAIL_SERVER_NAME
              ,EMAIL_SERVER_IPADDRESS
              ,EMAIL_SERVER_PORT
              ,EMAIL_AUTH_USER_ID
              ,EMAIL_AUTH_USER_PWD
              ,EMAIL_SSL_ENABLE
              ,EMAIL_IS_ACTIVE
              ,EMAIL_CREATED_BY
              ,EMAIL_CREATED_DATE
              ,EMAIL_ADMIN_MAILADRESS
            )
            values
            (
                p_ServerName
                ,p_ServerName
                ,p_ServerPort
                ,p_Username
                ,p_Password
                ,p_SSLEnable
                ,p_IsActive
                ,'Admin'
                ,SYSTIMESTAMP
                ,p_FromEmailId
            );
        ELSE
            UPDATE CSM_EMAIL_CONFIGURATION set
              EMAIL_SERVER_NAME = p_ServerName
              ,EMAIL_SERVER_IPADDRESS = p_ServerName
              ,EMAIL_SERVER_PORT = p_ServerPort
              ,EMAIL_AUTH_USER_ID = p_Username
              ,EMAIL_AUTH_USER_PWD = p_Password
              ,EMAIL_SSL_ENABLE = p_SSLEnable
              ,EMAIL_IS_ACTIVE = p_IsActive
              ,EMAIL_UPDATED_DATE = SYSTIMESTAMP
              ,EMAIL_ADMIN_MAILADRESS = p_FromEmailId
            WHERE EMAIL_SERVER_ID = v_EMSILSERVERID;
        END IF;        
    END;

    FUNCTION FN_CWT_GetUserAccess RETURN o_Cursor IS
    CUR O_Cursor;
    BEGIN
        Open CUR FOR Select * from (Select
		ACCESS_ID
		,ACCESS_CODE
		,ACCESS_FIRSTNAME 
		,ACCESS_LASTTNAME 
		,ACCESS_EMAIL 
		,ACCESS_MOBILE
		,DATE_CREATED 
        From CSM_USER_ACCESS 
        Order by ACCESS_ID Desc) WHERE rownum <= 1;
        RETURN CUR;
    END;

    FUNCTION FN_CWT_GetMailServerDetail  RETURN o_Cursor IS
    CUR O_Cursor;
    BEGIN
	OPEN CUR FOR SELECT EMAIL_SERVER_ID
		  ,EMAIL_SERVER_NAME
		  ,EMAIL_SERVER_IPADDRESS
		  ,EMAIL_SERVER_PORT
		  ,EMAIL_AUTH_USER_ID
		  ,EMAIL_AUTH_USER_PWD
		  ,EMAIL_DEFAULT_SENDER
		  ,EMAIL_IS_ACTIVE
		  ,EMAIL_CREATED_BY
		  ,EMAIL_CREATED_DATE
		  ,EMAIL_UPDATED_BY
		  ,EMAIL_UPDATED_DATE
		  ,EMAIL_COMMENTS
		  ,EMAIL_SSL_ENABLE
		  ,EMAIL_ADMIN_MAILADRESS
	  FROM CSM_EMAIL_CONFIGURATION;
    RETURN CUR;  
    END;

END;
/

Exit;