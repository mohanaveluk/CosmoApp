--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_GROUP_SCHEDULE
--------------------------------------------------------

  CREATE TABLE "CSM_GROUP_SCHEDULE" 
   (	"GROUP_SCH_ID" NUMBER(10,0), 
	"GROUP_ID" NUMBER(10,0), 
	"GROUP_SCH_TIME" TIMESTAMP (3), 
	"GROUP_SCH_ACTION" VARCHAR2(50 BYTE), 
	"GROUP_SCH_STATUS" VARCHAR2(50 BYTE), 
	"GROUP_SCH_COMPLETED_TIME" TIMESTAMP (3), 
	"GROUP_SCH_COMMENTS" VARCHAR2(4000 BYTE), 
	"GROUP_SCH_CREATED_BY" VARCHAR2(20 BYTE), 
	"GROUP_SCH_CREATED_DATETIME" TIMESTAMP (3), 
	"GROUP_SCH_UPDATED_BY" VARCHAR2(20 BYTE), 
	"GROUP_SCH_UPDATED_DATETIME" TIMESTAMP (3), 
	"GROUP_SCH_ONDEMAND" NUMBER(1,0) DEFAULT 1, 
	"GROUP_SCH_RESULT" VARCHAR2(50 BYTE), 
	"GROUP_SCH_REQUESTSOURCE" VARCHAR2(50 BYTE), 
	"GROUP_SCH_ISACTIVE" NUMBER(1,0) DEFAULT 1, 
	"GROUP_SCH_DUMMY1" VARCHAR2(1000 BYTE), 
	"GROUP_SCH_DUMMY2" VARCHAR2(1000 BYTE), 
	"GROUP_SCH_DUMMY3" VARCHAR2(1000 BYTE), 
	"GROUP_SCH_DUMMY4" VARCHAR2(1000 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_CSM_GROUP_SCHEDULE
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_CSM_GROUP_SCHEDULE" ON "CSM_GROUP_SCHEDULE" ("GROUP_SCH_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_GROUP_SCHEDULE_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_GROUP_SCHEDULE_SEQ_TR" 
 BEFORE INSERT ON CSM_GROUP_SCHEDULE FOR EACH ROW
  WHEN (NEW.GROUP_SCH_ID IS NULL) BEGIN
 SELECT CSM_GROUP_SCHEDULE_seq.NEXTVAL INTO :NEW.GROUP_SCH_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_GROUP_SCHEDULE_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_GROUP_SCHEDULE
--------------------------------------------------------

  ALTER TABLE "CSM_GROUP_SCHEDULE" ADD CONSTRAINT "PK_CSM_GROUP_SCHEDULE" PRIMARY KEY ("GROUP_SCH_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;

exit;