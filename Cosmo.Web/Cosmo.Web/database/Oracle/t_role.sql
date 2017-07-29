--Type
create or replace type t_role as object( RoleId number);
/
create or replace type t_role_type as table of t_role;
/

exit;