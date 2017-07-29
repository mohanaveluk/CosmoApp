--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_REPORT_SUBSCRIPTION
--------------------------------------------------------

  CREATE TABLE "CSM_REPORT_SUBSCRIPTION" 
   (	"SUBSCRIPTION_ID" NUMBER(10,0), 
	"SUBSCRIPTION_TYPE" VARCHAR2(100 BYTE), 
	"SUBSCRIPTION_TIME" VARCHAR2(100 BYTE), 
	"SUBSCRIPTION_ISACTIVE" NUMBER(1,0) DEFAULT (1), 
	"CREATED_BY" VARCHAR2(100 BYTE), 
	"CREATED_DATE" TIMESTAMP (3), 
	"UPDATED_BY" VARCHAR2(100 BYTE), 
	"UPDATED_DATE" TIMESTAMP (3), 
	"SCH_LASTJOBRAN_TIME" TIMESTAMP (3), 
	"SCH_NEXTJOBRAN_TIME" TIMESTAMP (3)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_REPORT_SUBSCRIPTION
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_REPORT_SUBSCRIPTION" ON "CSM_REPORT_SUBSCRIPTION" ("SUBSCRIPTION_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_REPORT_SUBSCRIPTION_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_REPORT_SUBSCRIPTION_SEQ_TR" 
 BEFORE INSERT ON CSM_REPORT_SUBSCRIPTION FOR EACH ROW
  WHEN (NEW.SUBSCRIPTION_ID IS NULL) BEGIN
 SELECT CSM_REPORT_SUBSCRIPTION_seq.NEXTVAL INTO :NEW.SUBSCRIPTION_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_REPORT_SUBSCRIPTION_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_REPORT_SUBSCRIPTION
--------------------------------------------------------

  ALTER TABLE "CSM_REPORT_SUBSCRIPTION" ADD CONSTRAINT "PK_REPORT_SUBSCRIPTION" PRIMARY KEY ("SUBSCRIPTION_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;
  ALTER TABLE "CSM_REPORT_SUBSCRIPTION" MODIFY ("SUBSCRIPTION_ISACTIVE" NOT NULL ENABLE);
  ALTER TABLE "CSM_REPORT_SUBSCRIPTION" MODIFY ("SUBSCRIPTION_TIME" NOT NULL ENABLE);
  ALTER TABLE "CSM_REPORT_SUBSCRIPTION" MODIFY ("SUBSCRIPTION_TYPE" NOT NULL ENABLE);
  ALTER TABLE "CSM_REPORT_SUBSCRIPTION" MODIFY ("SUBSCRIPTION_ID" NOT NULL ENABLE);

exit;