--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_LDAP_CONFIGURATION
--------------------------------------------------------

  CREATE TABLE "CSM_LDAP_CONFIGURATION" 
   (	"LDAP_ID" NUMBER(10,0), 
	"LDAP_SERVER_NAME" VARCHAR2(100 BYTE), 
	"LDAP_AUTH_USER_ID" VARCHAR2(100 BYTE), 
	"LDAP_AUTH_USER_PWD" VARCHAR2(100 BYTE), 
	"LDAP_IS_ACTIVE" NUMBER(1,0), 
	"LDAP_CREATED_BY" VARCHAR2(100 BYTE), 
	"LDAP_CREATED_DATE" TIMESTAMP (3), 
	"LDAP_UPDATED_BY" VARCHAR2(100 BYTE), 
	"LDAP_UPDATED_DATE" TIMESTAMP (3), 
	"LDAP_COMMENTS" VARCHAR2(2000 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_CSM_LDAP_CONFIGURATION
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_CSM_LDAP_CONFIGURATION" ON "CSM_LDAP_CONFIGURATION" ("LDAP_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_LDAP_CONFIGURATION_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_LDAP_CONFIGURATION_SEQ_TR" 
 BEFORE INSERT ON CSM_LDAP_CONFIGURATION FOR EACH ROW
  WHEN (NEW.LDAP_ID IS NULL) BEGIN
 SELECT CSM_LDAP_CONFIGURATION_seq.NEXTVAL INTO :NEW.LDAP_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_LDAP_CONFIGURATION_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_LDAP_CONFIGURATION
--------------------------------------------------------

  ALTER TABLE "CSM_LDAP_CONFIGURATION" ADD CONSTRAINT "PK_CSM_LDAP_CONFIGURATION" PRIMARY KEY ("LDAP_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;
  ALTER TABLE "CSM_LDAP_CONFIGURATION" MODIFY ("LDAP_IS_ACTIVE" NOT NULL ENABLE);
  ALTER TABLE "CSM_LDAP_CONFIGURATION" MODIFY ("LDAP_AUTH_USER_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_LDAP_CONFIGURATION" MODIFY ("LDAP_SERVER_NAME" NOT NULL ENABLE);
  ALTER TABLE "CSM_LDAP_CONFIGURATION" MODIFY ("LDAP_ID" NOT NULL ENABLE);

exit;