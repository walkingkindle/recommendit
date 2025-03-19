using System.Runtime.InteropServices.JavaScript;

namespace Recommendit.Result;

public static class DatabaseErrors
{
    public static readonly Error NullCollectionError = new Error("Database.NullCollectionError","There was a  problem with retreving your collection of shows");

    public static readonly Error NullShowError = new Error("Database.NullShowError",
        "There was a problem accessing the database and we couldn't retrieve the show requested.");

    public static readonly Error DatabaseInsertError = new Error("Database.InsertFailError",
        "We could not insert the shows at this time. Please check the logs.");



}