-- Runs against master. $(DatabaseName) is passed by entrypoint.sh (MSSQL_DATABASE, default AppArch).
IF DB_ID(N'$(DatabaseName)') IS NULL
BEGIN
    CREATE DATABASE [$(DatabaseName)];
END;
