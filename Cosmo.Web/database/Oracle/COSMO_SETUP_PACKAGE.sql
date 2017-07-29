--select * from CSM_USER;

CREATE OR REPLACE PACKAGE "COSMO_SETUP_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    FUNCTION FN_CWT_GetLogin(p_USERID IN VARCHAR2, p_PASSWORD in VARCHAR2) RETURN INTEGER;
    FUNCTION FN_CWT_GetMenuItems(p_ROLEID IN varchar2) Return o_Cursor;
    FUNCTION FN_CWT_GetMailServerDetail Return o_Cursor;
END COSMO_SETUP_PACKAGE;
/

CREATE OR REPLACE PACKAGE BODY "COSMO_SETUP_PACKAGE" AS
    FUNCTION FN_CWT_GetLogin(p_USERID IN VARCHAR2, p_PASSWORD   in VARCHAR2) RETURN INTEGER IS
    v_tempUserCount number(10);
	v_tempUser number(10);
    p_SCOPE_OUTPUT number(1);
    BEGIN
        if p_USERID is not null and p_USERID <> '' 	Then
            Select count(*) INTO v_tempUserCount  from CSM_USER cuser 
                WHERE USER_IS_ACTIVE = 'true'
                  and 	USER_EMAIL_ADDRESS = p_USERID;
            if v_tempUserCount > 0
            Then 
                SELECT cuser.USER_ID INTO v_tempUser 
                FROM CSM_USER cuser
                  WHERE USER_IS_ACTIVE = 'true'
                  and USER_IS_DELETED = 'false'
                  and 	USER_EMAIL_ADDRESS = p_USERID 
                  and USER_PASSWORD = p_PASSWORD;
                if v_tempUser > 0
                Then  
    
                      p_SCOPE_OUTPUT := v_tempUser; --valid user
                else
                    p_SCOPE_OUTPUT := -1; --Password mismatch not present
                End if;
            else
                p_SCOPE_OUTPUT := 0; --user not present
            End if;
        End if;	
        RETURN p_SCOPE_OUTPUT;
    END;
    
    FUNCTION FN_CWT_GetMenuItems(p_ROLEID IN varchar2) Return o_Cursor IS
    CUR O_Cursor;
    BEGIN
        if p_ROLEID = '1'	Then
            Open CUR FOR Select crm.RM_ID
                ,crm.ROLE_ID
                ,crm.MENU_ID
                ,crm.RM_ISACTIVE
                ,role.ROLE_NAME
                ,role.ROLE_TYPE
                ,MENU_MAIN
                ,MENU_SUB
                ,MENU_PATH
                ,MENU_ISPOPUP
                ,MENU_ISACTIVE		
                ,MENU_MAIN_ORDER
                ,MENU_SUB_ORDER
            From CSM_ROLEMENU crm 
            inner join CSM_ROLES role on role.ROLE_ID = crm.ROLE_ID
            inner join CSM_MENUS menu on menu.MENU_ID = crm.MENU_ID
            Where RM_ISACTIVE = 1
                and MENU_ISACTIVE =1
            order by MENU_MAIN_ORDER, MENU_SUB_ORDER;
        else
            Open CUR FOR Select crm.RM_ID
                ,crm.ROLE_ID
                ,crm.MENU_ID
                ,crm.RM_ISACTIVE
                ,role.ROLE_NAME
                ,role.ROLE_TYPE
                ,MENU_MAIN
                ,MENU_SUB
                ,MENU_PATH
                ,MENU_ISPOPUP
                ,MENU_ISACTIVE		
                ,MENU_MAIN_ORDER
                ,MENU_SUB_ORDER
            From CSM_ROLEMENU crm 
            inner join CSM_ROLES role on role.ROLE_ID = crm.ROLE_ID
            inner join CSM_MENUS menu on menu.MENU_ID = crm.MENU_ID
            Where RM_ISACTIVE = 1
                and MENU_ISACTIVE = 1
                --and crm.ROLE_ID in (Replace(@ROLEID,'''',''))
                --and CONVERT(varchar(10), crm.ROLE_ID) in (@ROLEID)
                and crm.ROLE_ID in (
                    select RoleId from table(funcListToTableInt_orcl(p_ROLEID,','))
                )
            order by MENU_MAIN_ORDER, MENU_SUB_ORDER;
        End if;
        RETURN CUR;
    END;
    
    FUNCTION FN_CWT_GetMailServerDetail Return o_Cursor IS
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
    END;
END;
/

Exit;