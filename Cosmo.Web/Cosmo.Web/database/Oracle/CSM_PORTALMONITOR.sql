--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_PORTALMONITOR
--------------------------------------------------------

  CREATE TABLE "CSM_PORTALMONITOR" 
   (	"PMON_ID" NUMBER(10,0), 
	"URL_ID" NUMBER(10,0), 
	"ENV_ID" NUMBER(10,0), 
	"PMON_STATUS" VARCHAR2(20 BYTE), 
	"PMON_MESSAGE" VARCHAR2(4000 BYTE), 
	"PMON_RESPONSETIME" NUMBER(10,0), 
	"PMON_EXCEPTION" VARCHAR2(4000 BYTE), 
	"PMON_CREATEDBY" VARCHAR2(100 BYTE) DEFAULT 'System', 
	"PMON_CREATEDDATE" TIMESTAMP (3) DEFAULT (systimestamp), 
	"PMON_COMMENTS" VARCHAR2(4000 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_CSM_PORTALMONITOR
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_CSM_PORTALMONITOR" ON "CSM_PORTALMONITOR" ("PMON_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_PORTALMONITOR_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_PORTALMONITOR_SEQ_TR" 
 BEFORE INSERT ON CSM_PORTALMONITOR FOR EACH ROW
  WHEN (NEW.PMON_ID IS NULL) BEGIN
 SELECT CSM_PORTALMONITOR_seq.NEXTVAL INTO :NEW.PMON_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_PORTALMONITOR_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_PORTALMONITOR
--------------------------------------------------------

  ALTER TABLE "CSM_PORTALMONITOR" ADD CONSTRAINT "PK_CSM_PORTALMONITOR" PRIMARY KEY ("PMON_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;
  ALTER TABLE "CSM_PORTALMONITOR" MODIFY ("PMON_CREATEDBY" NOT NULL ENABLE);
  ALTER TABLE "CSM_PORTALMONITOR" MODIFY ("ENV_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_PORTALMONITOR" MODIFY ("URL_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_PORTALMONITOR" MODIFY ("PMON_ID" NOT NULL ENABLE);

exit;