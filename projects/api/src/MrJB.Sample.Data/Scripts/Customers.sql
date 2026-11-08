-- Customers table read by CustomerSqlRepository. Run against the database in Database:Customers.
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
