--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_URLCONFIGURATION
--------------------------------------------------------

  CREATE TABLE "CSM_URLCONFIGURATION" 
   (	"URL_ID" NUMBER(10,0), 
	"ENV_ID" NUMBER(10,0), 
	"URL_TYPE" VARCHAR2(50 BYTE), 
	"URL_ADDRESS" VARCHAR2(500 BYTE), 
	"URL_DISPLAYNAME" VARCHAR2(500 BYTE), 
	"URL_MATCHCONTENT" VARCHAR2(500 BYTE), 
	"URL_INTERVAL" NUMBER(10,0), 
	"URL_USERNAME" VARCHAR2(500 BYTE), 
	"URL_PASSWORD" VARCHAR2(500 BYTE), 
	"URL_STATUS" NUMBER(1,0) DEFAULT (1), 
	"URL_ISACTIVE" NUMBER(1,0) DEFAULT (1), 
	"URL_CREATEDBY" VARCHAR2(100 BYTE), 
	"URL_CREATEDDATE" TIMESTAMP (3) DEFAULT (systimestamp), 
	"URL_UPDATEDBY" VARCHAR2(100 BYTE), 
	"URL_UPDATEDDATE" TIMESTAMP (3), 
	"URL_COMMENTS" VARCHAR2(4000 BYTE), 
	"URL_LASTJOBRUNTIME" TIMESTAMP (3), 
	"URL_NEXTJOBRUNTIME" TIMESTAMP (3) DEFAULT (systimestamp)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_CSM_URLCONFIGURATION
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_CSM_URLCONFIGURATION" ON "CSM_URLCONFIGURATION" ("URL_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_URLCONFIGURATION_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_URLCONFIGURATION_SEQ_TR" 
 BEFORE INSERT ON CSM_URLCONFIGURATION FOR EACH ROW
  WHEN (NEW.URL_ID IS NULL) BEGIN
 SELECT CSM_URLCONFIGURATION_seq.NEXTVAL INTO :NEW.URL_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_URLCONFIGURATION_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_URLCONFIGURATION
--------------------------------------------------------

  ALTER TABLE "CSM_URLCONFIGURATION" ADD CONSTRAINT "PK_CSM_URLCONFIGURATION" PRIMARY KEY ("URL_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;
  ALTER TABLE "CSM_URLCONFIGURATION" MODIFY ("URL_CREATEDBY" NOT NULL ENABLE);
  ALTER TABLE "CSM_URLCONFIGURATION" MODIFY ("URL_ADDRESS" NOT NULL ENABLE);
  ALTER TABLE "CSM_URLCONFIGURATION" MODIFY ("URL_TYPE" NOT NULL ENABLE);
  ALTER TABLE "CSM_URLCONFIGURATION" MODIFY ("ENV_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_URLCONFIGURATION" MODIFY ("URL_ID" NOT NULL ENABLE);

exit;