namespace MrJB.Sample.ServiceDefaults;

/// <summary>
///  Custom span names
/// </summary>
public static class Spans
{
    public const string CustomerId = "CustomerId";

    public const string Result = "Result";

    public const string Outcome = "Outcome";

    public const string DbSystem = "Db.System.Name";

    public const string DbStoredProcedure = "Db.StoredProcedure.Name";

    public static class Values
    {
        public const string Found = "found";
        public const string NotFound = "not_found";
        public const string Error = "error";
        public const string Success = "success";

        public const string SqlServer = "microsoft.sql_server";
    }
}
