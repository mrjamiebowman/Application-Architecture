-- Customers table and the stored procedures CustomerSqlRepository calls.
-- Run against the database in Database:Primary (sqlcmd or SSMS; GO separates batches).
IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers
    (
        CustomerId  INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
        FirstName   NVARCHAR(100)      NULL,
        LastName    NVARCHAR(100)      NULL,
        Email       NVARCHAR(256)      NULL,
        PhoneNumber NVARCHAR(32)       NULL
    );

    INSERT INTO dbo.Customers (FirstName, LastName, Email, PhoneNumber)
    VALUES (N'Ada', N'Lovelace', N'ada@example.com', N'555-0100'),
           (N'Alan', N'Turing', N'alan@example.com', N'555-0101');
END;
GO

CREATE OR ALTER PROCEDURE dbo.Customer_GetById
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END;
GO
