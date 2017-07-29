--------------------------------------------------------
--  File created - Tuesday-March-28-2017   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Function FUNCLISTTOTABLEINT_ORCL
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "FUNCLISTTOTABLEINT_ORCL" 
      (list1 varchar2,delim varchar2)  return t_role_type
as 
    v_table t_role_type;
    remainder  varchar2(4000);
begin
    remainder := list1;
    v_table  := t_role_type();

    while length(remainder)>0 loop
        --insert into t_role_table
        --       values (substr(remainder,1,instr(remainder||delim,delim)-1));
        --to_number()
        v_table.extend;
        v_table(v_table.count) := t_role(substr(remainder,1,instr(remainder||delim,delim)-1));
        remainder := substr(remainder,instr(remainder||delim,delim)+length(delim));
    end loop;
    return v_table;

    EXCEPTION
    WHEN OTHERS THEN
    v_table.DELETE;
    Return v_table;

end;


--select * from table(funcListToTableInt_orcl('3,4,1,32',','));

/
exit;