D:
cd D:\Projects\Cognos\Monitor\XP\Latest\Cosmo.Web\Cosmo.Web\database\SqlServer
@Echo Off
FOR /f %%i IN ('DIR ServiceManagerScript.sql /B') do call :RunScript %%i
GOTO :END
:RunScript
Echo Executing %1
"D:\Projects\Cognos\Monitor\XP\Latest\Cosmo.Web\Cosmo.Web\database\SqlServer\sqlcmd.exe" -U sa -P sonata$9 -S snowflake -d Cosmo0417 -i %1 -o "D:\Projects\Cognos\Monitor\XP\Latest\Cosmo.Web\Cosmo.Web\database\SqlServer\ServiceManageOutput.txt"
Echo Completed %1
:END
