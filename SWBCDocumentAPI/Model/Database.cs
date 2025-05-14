using Npgsql;

namespace SWBCDocumentAPI.Model;

public class Database
{
    public static NpgsqlConnection? Connection { get; set; }
}
