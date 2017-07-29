--------------------------------------------------------
--  File created - Monday-March-27-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table CSM_ACKNOWLEDGE
--------------------------------------------------------

  CREATE TABLE "CSM_ACKNOWLEDGE" 
   (	"ACK_ID" NUMBER(10,0), 
	"ENV_ID" NUMBER(10,0), 
	"CONFIG_ID" NUMBER(10,0), 
	"ACK_ISACKNOWLEDGE" NUMBER(1,0), 
	"ACK_ALERT" VARCHAR2(50 BYTE), 
	"ACK_COMMENTS" VARCHAR2(2000 BYTE), 
	"CREATED_BY" VARCHAR2(50 BYTE), 
	"CREATED_DATE" TIMESTAMP (3)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  DDL for Trigger CSM_ACKNOWLEDGE_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CSM_ACKNOWLEDGE_SEQ_TR" 
 BEFORE INSERT ON CSM_ACKNOWLEDGE FOR EACH ROW
  WHEN (NEW.ACK_ID IS NULL) BEGIN
 SELECT CSM_ACKNOWLEDGE_seq.NEXTVAL INTO :NEW.ACK_ID FROM DUAL;
END;

/
ALTER TRIGGER "CSM_ACKNOWLEDGE_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table CSM_ACKNOWLEDGE
--------------------------------------------------------

  ALTER TABLE "CSM_ACKNOWLEDGE" MODIFY ("CONFIG_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_ACKNOWLEDGE" MODIFY ("ENV_ID" NOT NULL ENABLE);
  ALTER TABLE "CSM_ACKNOWLEDGE" MODIFY ("ACK_ID" NOT NULL ENABLE);


exit;