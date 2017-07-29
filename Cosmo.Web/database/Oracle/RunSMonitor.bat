D:
cd D:\Projects\Cognos\Monitor\XP\Latest\Cosmo.Web\Cosmo.Web\database\Oracle
@Echo Off
FOR /f %%i IN (ServiceManager.sql) do call :RunScript %%i
GOTO :END
:RunScript
Echo Executing %1
sqlplus cosmo_042317/cosmo_042317@xe @"%1"
Echo Completed %1
:END
