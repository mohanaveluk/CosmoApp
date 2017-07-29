
CREATE OR REPLACE PACKAGE "COSMO_USER_PACKAGE" AS
    TYPE o_Cursor IS REF CURSOR;
    
    FUNCTION FN_CWT_GetLogin(p_USERID varchar2
                             ,p_PASSWORD varchar2) RETURN INTEGER;
    
    FUNCTION FN_CWT_GetUsers(p_USERID int) RETURN o_Cursor;  
    
    FUNCTION FN_CWT_GetUserRole(p_USERID int) RETURN o_Cursor;  
    
    FUNCTION FN_CWT_GetPersonaliseSetting(p_User_ID  int) RETURN o_Cursor;
    

    PROCEDURE SP_CWT_InsUpdUserRoles(
                                        p_USERID number
                                        ,p_USERROLES varchar2
                                    );
    
    PROCEDURE SP_CWT_InsUpdUser
                                ( 
                                    p_USERID number
                                    ,p_USERFIRSTNAME varchar2
                                    ,p_USERLASTNAME  varchar2
                                    ,p_USEREMAIL  varchar2
                                    ,p_USERPASSWORD varchar
                                    ,p_USERROLES varchar2
                                    ,p_ISACTIVE number
                                    --,@ISDELETED bit
                                    ,p_SCOPE_OUTPUT out number 
                                ) ;

    procedure SP_CWT_UpdateUserPassword(   p_USERID number
                                        ,p_USEREMAIL varchar2
                                        ,p_CURRENTPASSWORD varchar
                                        ,p_NEWPASSWORD varchar
                                        ,p_SCOPE_OUTPUT out number 
                                    );                                       
END COSMO_USER_PACKAGE;
/

CREATE OR REPLACE PACKAGE BODY "COSMO_USER_PACKAGE" AS
    
    FUNCTION FN_CWT_GetLogin(p_USERID varchar2
                             ,p_PASSWORD varchar2) RETURN INTEGER IS
        v_tempUserCount number(10);
        v_tempUser number(10);
        p_SCOPE_OUTPUT number := 0;
    BEGIN
        if p_USERID is not null Then
            Select count(*) INTO v_tempUserCount  from CSM_USER cuser 
                WHERE USER_IS_ACTIVE = 1
                  and 	USER_EMAIL_ADDRESS = p_USERID;
            if v_tempUserCount > 0
            Then 
                SELECT cuser.USER_ID INTO v_tempUser 
                FROM CSM_USER cuser
                  WHERE USER_IS_ACTIVE = 1
                  and USER_IS_DELETED = 0
                  and 	USER_EMAIL_ADDRESS = p_USERID 
                  and USER_PASSWORD = p_PASSWORD;
                if v_tempUser > 0
                Then  
                  p_SCOPE_OUTPUT := v_tempUser; --valid user
                else
                    p_SCOPE_OUTPUT := -1; --Password mismatch not present
                End if;
            Else
                p_SCOPE_OUTPUT := 0; --user not present
            End if;
        End if;	    
        return p_SCOPE_OUTPUT;
    END;
                             
    FUNCTION FN_CWT_GetUsers(p_USERID int) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        if p_USERID is null or p_USERID <=0
            Then
              OPEN CUR FOR SELECT cuser.USER_ID
                  ,cuser.USER_LOGIN_ID
                  ,cuser.USER_FIRST_NAME
                  ,cuser.USER_LAST_NAME
                  ,cuser.USER_EMAIL_ADDRESS
                  ,cuser.USER_ROLE
                  ,cuser.USER_PASSWORD
                  ,cuser.USER_IS_LDAP_USER
                  ,cuser.USER_IS_ACTIVE
                  ,cuser.USER_IS_DELETED
                  ,cuser.USER_ATTEMPS
                  ,cuser.USER_ISLOCKED
                  ,cuser.USER_LOCKEDTIME
                  ,cuser.USER_CREATED_BY
                  ,cuser.USER_CREATED_DATE
                  ,cuser.USER_UPDATED_BY
                  ,cuser.USER_UPDATED_DATE
                  ,cuser.USER_COMMENTS
              FROM CSM_USER cuser
              WHERE USER_IS_DELETED = 0
                    and USER_EMAIL_ADDRESS <> 'admin@cosmo.com';
            else
              OPEN CUR FOR SELECT cuser.USER_ID
                  ,cuser.USER_LOGIN_ID
                  ,cuser.USER_FIRST_NAME
                  ,cuser.USER_LAST_NAME
                  ,cuser.USER_EMAIL_ADDRESS
                  ,cuser.USER_ROLE
                  ,cuser.USER_PASSWORD
                  ,cuser.USER_IS_LDAP_USER
                  ,cuser.USER_IS_ACTIVE
                  ,cuser.USER_IS_DELETED
                  ,cuser.USER_ATTEMPS
                  ,cuser.USER_ISLOCKED
                  ,cuser.USER_LOCKEDTIME
                  ,cuser.USER_CREATED_BY
                  ,cuser.USER_CREATED_DATE
                  ,cuser.USER_UPDATED_BY
                  ,cuser.USER_UPDATED_DATE
                  ,cuser.USER_COMMENTS
              FROM CSM_USER cuser
              WHERE cuser.USER_IS_DELETED = 0
                    and cuser.USER_ID = p_USERID;
                    --and [USER_EMAIL_ADDRESS] <> 'admin@cosmo.com'
            End if;        
        return CUR ;
    End;
    
    FUNCTION FN_CWT_GetUserRole(p_USERID int) RETURN o_Cursor Is
    CUR o_Cursor;
    Begin
        if p_USERID <=0
        Then
            OPEN CUR FOR SELECT cuser.USER_ID
                ,curole.USER_ROLE_ID
                ,curole.ROLE_ID
                ,roles.ROLE_NAME
              FROM CSM_USER cuser
              inner join CSM_USER_ROLE curole on cuser.USER_ID = curole.USER_ID
              inner join CSM_ROLES roles on roles.ROLE_ID = curole.ROLE_ID
              WHERE curole.USER_ROLE_ISACTIVE = 1;
                -- USER_IS_ACTIVE = 'true'
                --and curole.USER_ROLE_ISACTIVE = 1
                --and roles.ROLE_ISACTIVE = 1
        Else
            OPEN CUR FOR SELECT cuser.USER_ID
                ,curole.USER_ROLE_ID
                ,curole.ROLE_ID
                ,roles.ROLE_NAME
              FROM CSM_USER cuser
              inner join CSM_USER_ROLE curole on cuser.USER_ID = curole.USER_ID
              inner join CSM_ROLES roles on roles.ROLE_ID = curole.ROLE_ID
              WHERE cuser.USER_ID = p_USERID
                and curole.USER_ROLE_ISACTIVE = 1;
                --USER_IS_ACTIVE = 'true'
                --and roles.ROLE_ISACTIVE = 1
        END IF;    
        return CUR;
    End;
    
    FUNCTION FN_CWT_GetPersonaliseSetting(p_User_ID  int) RETURN o_Cursor IS
    CUR o_Cursor;
    Begin
        open CUR for select 
		PERS_ID
		,PERS_DB_REFRESHTIME 
		,(Case when 
			PERS_UPDATEDDATE = null then PERS_CREATEDDATE 
			else
			PERS_CREATEDDATE
		End) PERS_UPDATEDDATE
		from  CSM_POERSONALIZE
		Where PERS_ISACTIVE = 1
			AND User_ID = p_User_ID;
        
        RETURN CUR;
    End;
    
    PROCEDURE SP_CWT_InsUpdUserRoles
                                (
                                    p_USERID number
                                    ,p_USERROLES varchar2
                                ) IS
    v_Character CHAR(1)
	;v_StartIndex NUMBER(10)
	;v_EndIndex NUMBER(10)
	;v_Input varchar2(4000)
	;v_tempRoleId number(10)
	;v_tempUserRoleId number(10);                                
    Begin
        v_Character := ',';
        v_Input := p_USERROLES;
        v_StartIndex := 1;
        if p_USERID > 0
        then
            if p_USERROLES is not null
            Then
                IF SUBSTR(v_Input, LENGTH(RTRIM(v_Input)) , LENGTH(RTRIM(v_Input))) <> v_Character
                THEN
                    v_Input := v_Input || v_Character;
                END IF;				
                
                update CSM_USER_ROLE set USER_ROLE_ISACTIVE = 0
                where CSM_USER_ROLE.USER_ID = p_USERID;
                
                WHILE INSTR(v_Input, v_Character) > 0
                Loop
                    v_EndIndex := INSTR(v_Input, v_Character);
                    v_tempRoleId := SUBSTR(v_Input, v_StartIndex, v_EndIndex - 1);
                    
                    select count(USER_ROLE_ID) into v_tempUserRoleId from CSM_USER_ROLE	
                    where CSM_USER_ROLE.ROLE_ID = v_tempRoleId
                        and CSM_USER_ROLE.USER_ID = p_USERID;
                    
                    if(v_tempUserRoleId >0)
                    Then
                        update CSM_USER_ROLE set USER_ROLE_ISACTIVE = 1 
                        where CSM_USER_ROLE.ROLE_ID = v_tempRoleId
                            and CSM_USER_ROLE.USER_ID = p_USERID;
                    else if (v_tempUserRoleId <= 0)
                    Then
                        insert into CSM_USER_ROLE
                        (
                            USER_ID
                            ,ROLE_ID
                            ,USER_ROLE_ISACTIVE
                            
                        )
                        values
                        (
                            p_USERID
                            ,v_tempRoleId
                            ,1 
                        );
                    end if;
                    end if;
                    v_Input := SUBSTR(v_Input, v_EndIndex + 1, LENGTH(RTRIM(v_Input)));
                End LOOP;
            End if;
        End if;        
        
    End;
    
    PROCEDURE SP_CWT_InsUpdUser
                                ( 
                                    p_USERID number
                                    ,p_USERFIRSTNAME varchar2
                                    ,p_USERLASTNAME  varchar2
                                    ,p_USEREMAIL  varchar2
                                    ,p_USERPASSWORD varchar
                                    ,p_USERROLES varchar2
                                    ,p_ISACTIVE number
                                    --,@ISDELETED bit
                                    ,p_SCOPE_OUTPUT out number 
                                ) IS
    	v_UserCount number(10)
	;v_tempUserId number(10);                            
    Begin
        If p_USERID <= 0
        Then
            select COUNT(USER_EMAIL_ADDRESS) into v_UserCount
            from CSM_USER 
            where CSM_USER.USER_EMAIL_ADDRESS = p_USEREMAIL
                and USER_IS_ACTIVE = 1;
            if v_UserCount > 0
            Then
                p_SCOPE_OUTPUT := -1;
            else
                Insert into CSM_USER 
                (
                    USER_LOGIN_ID
                    ,USER_FIRST_NAME
                    ,USER_LAST_NAME
                    ,USER_EMAIL_ADDRESS
                    ,USER_PASSWORD
                    ,USER_IS_ACTIVE
                    --,USER_IS_DELETED
                )
                values
                (
                    p_USEREMAIL
                    ,p_USERFIRSTNAME
                    ,p_USERLASTNAME
                    ,p_USEREMAIL
                    ,p_USERPASSWORD
                    ,p_ISACTIVE
                    --,@ISDELETED
                    
                ) RETURNING USER_ID INTO v_tempUserId;
                --v_tempUserId := IDENT_CURRENT('CSM_USER');
                SP_CWT_InsUpdUserRoles( v_tempUserId, p_USERROLES);
                
                insert into CSM_POERSONALIZE (User_ID, PERS_DB_REFRESHTIME, PERS_ISACTIVE, PERS_CREATEDDATE, PERS_CREATEDBY) values(v_tempUserId,2,1,SYSTIMESTAMP,1);
                
                p_SCOPE_OUTPUT := 1; --IDENT_CURRENT('CSM_USER')
            end if;
        Else if p_USERID > 0
        Then
            Update CSM_USER set 
                USER_LOGIN_ID = p_USEREMAIL
                ,USER_FIRST_NAME = p_USERFIRSTNAME
                ,USER_LAST_NAME = p_USERLASTNAME
                ,USER_EMAIL_ADDRESS = p_USEREMAIL
                ,USER_PASSWORD = p_USERPASSWORD
                ,USER_IS_ACTIVE		= p_ISACTIVE
                --,USER_IS_DELETED = @ISDELETED
            where USER_ID = p_USERID;
            
            v_tempUserId := p_USERID;
            SP_CWT_InsUpdUserRoles( v_tempUserId, p_USERROLES);
            p_SCOPE_OUTPUT := 2;
        End if;
        End if;    
    End;
    
    procedure SP_CWT_UpdateUserPassword(   p_USERID number
                                        ,p_USEREMAIL varchar2
                                        ,p_CURRENTPASSWORD varchar
                                        ,p_NEWPASSWORD varchar
                                        ,p_SCOPE_OUTPUT out number 
                                    ) IS
    v_tempUserCount number(10);                                    
    Begin
        if p_USERID >= 1
        Then
            select COUNT(*) into v_tempUserCount from CSM_USER 
            Where USER_ID = p_USERID
                and USER_PASSWORD = p_CURRENTPASSWORD
                and USER_EMAIL_ADDRESS = p_USEREMAIL;
            if v_tempUserCount >=1
            Then
                Update CSM_USER set 
                    USER_PASSWORD = p_NEWPASSWORD
                Where USER_ID = p_USERID
                    and USER_EMAIL_ADDRESS = p_USEREMAIL;
                p_SCOPE_OUTPUT := 1;
            Else
                p_SCOPE_OUTPUT := 0;
            End if;
        Else
            p_SCOPE_OUTPUT := -1;
        End if;    
    End;
    
    
    
END;
/                

Exit;