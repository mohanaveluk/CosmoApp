--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_MONITOR
--------------------------------------------------------

  CREATE TABLE "CSM_MONITOR" 
   (	"MON_ID" NUMBER(10,0), 
	"SCH_ID" NUMBER(10,0), 
	"CONFIG_ID" NUMBER(10,0), 
	"ENV_ID" NUMBER(10,0), 
	"MON_STATUS" VARCHAR2(200 BYTE), 
	"MON_START_DATE_TIME" VARCHAR2(200 BYTE), 
	"MON_END_DATE_TIME" VARCHAR2(200 BYTE), 
	"MON_IS_ACTIVE" NUMBER(1,0), 
	"MON_CREATED_BY" VARCHAR2(100 BYTE), 
	"MON_CREATED_DATE" TIMESTAMP (3), 
	"MON_UPDATED_BY" VARCHAR2(100 BYTE), 
	"MON_UPDATED_DATE" TIMESTAMP (3), 
	"MON_COMMENTS" VARCHAR2(2000 BYTE), 
	"MON_ISACKNOWLEDGE" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_CSM_MONITOR
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_CSM_MONITOR" ON "CSM_MONITOR" ("MON_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_MONITOR_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_MONITOR_SEQ_TR" 
 BEFORE INSERT ON CSM_MONITOR FOR EACH ROW
  WHEN (NEW.MON_ID IS NULL) BEGIN
 SELECT CSM_MONITOR_seq.NEXTVAL INTO :NEW.MON_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_MONITOR_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_MONITOR
--------------------------------------------------------

  ALTER TABLE "CSM_MONITOR" ADD CONSTRAINT "PK_CSM_MONITOR" PRIMARY KEY ("MON_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;
  ALTER TABLE "CSM_MONITOR" MODIFY ("MON_IS_ACTIVE" NOT NULL ENABLE);
  ALTER TABLE "CSM_MONITOR" MODIFY ("CONFIG_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_MONITOR" MODIFY ("SCH_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_MONITOR" MODIFY ("MON_ID" NOT NULL ENABLE);

exit;