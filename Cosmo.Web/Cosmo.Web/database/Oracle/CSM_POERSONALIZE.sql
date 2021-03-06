--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_POERSONALIZE
--------------------------------------------------------

  CREATE TABLE "CSM_POERSONALIZE" 
   (	"PERS_ID" NUMBER(10,0), 
	"USER_ID" NUMBER(10,0), 
	"PERS_DB_REFRESHTIME" NUMBER(10,0), 
	"PERS_ISACTIVE" NUMBER(1,0), 
	"PERS_CREATEDDATE" TIMESTAMP (3), 
	"PERS_CREATEDBY" VARCHAR2(10 BYTE), 
	"PERS_UPDATEDDATE" TIMESTAMP (3), 
	"PERS_UPDATEDBY" VARCHAR2(10 BYTE), 
	"PERS_FUTURE_COL_1" VARCHAR2(100 BYTE), 
	"PERS_FUTURE_COL_2" VARCHAR2(100 BYTE), 
	"PERS_FUTURE_COL_3" VARCHAR2(100 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_POERSONALIZE_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_POERSONALIZE_SEQ_TR" 
 BEFORE INSERT ON CSM_POERSONALIZE FOR EACH ROW
  WHEN (NEW.PERS_ID IS NULL) BEGIN
 SELECT CSM_POERSONALIZE_seq.NEXTVAL INTO :NEW.PERS_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_POERSONALIZE_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_POERSONALIZE
--------------------------------------------------------

  ALTER TABLE "CSM_POERSONALIZE" MODIFY ("USER_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_POERSONALIZE" MODIFY ("PERS_ID" NOT NULL ENABLE);

exit;