using System.Data;
using System.Data.SQLite;
using Amethyst.Core.Profiles;

namespace Amethyst.Storages.SQL;

public class SQLiteProvider : ISQLProvider, IDisposable
{
    private readonly string _connectionString;
    private SQLiteConnection? _connection;

    public SQLiteProvider(string databasePath)
    {
        _connectionString = $"Data Source={databasePath};Version=3;";
    }

    public SQLiteProvider(ServerProfile profile, string? name = null)
    {
        name ??= profile.Name;

        string databaseFileName = $"{name}.sqlite"; // Use profile name for database file
        string databasePath = Path.Combine(profile.SavePath, databaseFileName); // Store in profile directory

        _connectionString = $"Data Source={databasePath};Version=3;";
    }

    public void OpenConnection()
    {
        _connection ??= new SQLiteConnection(_connectionString);

        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void CloseConnection()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
        }
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
    {
        using SQLiteCommand cmd = new(query, _connection);

        AddParameters(cmd, parameters);

        return cmd.ExecuteNonQuery();
    }

    public object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
    {
        using SQLiteCommand cmd = new(query, _connection);

        AddParameters(cmd, parameters);

        return cmd.ExecuteScalar();
    }

    public List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
    {
        using SQLiteCommand cmd = new(query, _connection);

        AddParameters(cmd, parameters);

        using SQLiteDataReader reader = cmd.ExecuteReader();

        List<Dictionary<string, object>> result = [];

        while (reader.Read())
        {
            Dictionary<string, object> row = [];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }

            result.Add(row);
        }

        return result;
    }

    private static void AddParameters(SQLiteCommand cmd, Dictionary<string, object>? parameters)
    {
        if (parameters == null)
        {
            return;
        }

        foreach (KeyValuePair<string, object> param in parameters)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }

                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
