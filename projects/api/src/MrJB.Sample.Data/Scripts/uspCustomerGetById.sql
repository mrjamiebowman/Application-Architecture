-- Read by CustomerSqlRepository.GetCustomerByIdAsync. Requires dbo.Customers (Customers.sql).
CREATE OR ALTER PROCEDURE dbo.uspCustomerGetById
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END;
