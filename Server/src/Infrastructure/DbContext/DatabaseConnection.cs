using Microsoft.Data.SqlClient;

namespace Infrastructure.DbContext;

public class DatabaseConnection
{
    private readonly Dictionary<string, string> _connectionStrings;

    public DatabaseConnection(Dictionary<string, string> connectionStrings)
    {
        _connectionStrings = connectionStrings;
    }

    public SqlConnection CreateConnection(string databaseName = "Default")
    {
        if (!_connectionStrings.ContainsKey(databaseName))
        {
            throw new ArgumentException($"Connection string for database '{databaseName}' not found.");
        }

        return new SqlConnection(_connectionStrings[databaseName]);
    }

    public bool HasDatabase(string databaseName)
    {
        return _connectionStrings.ContainsKey(databaseName);
    }

    public IEnumerable<string> GetAvailableDatabases()
    {
        return _connectionStrings.Keys;
    }
} 