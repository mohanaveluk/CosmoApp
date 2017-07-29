D:
cd "D:\Projects\Cognos\Oracle sql\Scripts"
FOR /f %%i IN (ServiceManager.sql) do call :RunScript %%i
GOTO :END
:RunScript
Echo Executing %1
sqlplus cosmodev/cosmodev@xe  @"%1"
Echo Executing %1
:END