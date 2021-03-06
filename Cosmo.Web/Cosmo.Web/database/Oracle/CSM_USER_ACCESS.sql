--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_USER_ACCESS
--------------------------------------------------------

  CREATE TABLE "CSM_USER_ACCESS" 
   (	"ACCESS_ID" NUMBER(10,0), 
	"ACCESS_CODE" VARCHAR2(100 BYTE), 
	"ACCESS_FIRSTNAME" VARCHAR2(100 BYTE), 
	"ACCESS_LASTTNAME" VARCHAR2(100 BYTE), 
	"ACCESS_EMAIL" VARCHAR2(100 BYTE), 
	"ACCESS_MOBILE" VARCHAR2(100 BYTE), 
	"DATE_CREATED" TIMESTAMP (3)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Index PK_CSM_USER_ACCESS
--------------------------------------------------------

  CREATE UNIQUE INDEX "PK_CSM_USER_ACCESS" ON "CSM_USER_ACCESS" ("ACCESS_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_USER_ACCESS_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_USER_ACCESS_SEQ_TR" 
 BEFORE INSERT ON CSM_USER_ACCESS FOR EACH ROW
  WHEN (NEW.ACCESS_ID IS NULL) BEGIN
 SELECT CSM_USER_ACCESS_seq.NEXTVAL INTO :NEW.ACCESS_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_USER_ACCESS_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_USER_ACCESS
--------------------------------------------------------

  ALTER TABLE "CSM_USER_ACCESS" ADD CONSTRAINT "PK_CSM_USER_ACCESS" PRIMARY KEY ("ACCESS_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;
  ALTER TABLE "CSM_USER_ACCESS" MODIFY ("ACCESS_CODE" NOT NULL ENABLE);
  ALTER TABLE "CSM_USER_ACCESS" MODIFY ("ACCESS_ID" NOT NULL ENABLE);

exit;