using System.Data;
using MySql.Data.MySqlClient;

namespace Amethyst.Storages.SQL;

public class MySQLProvider : ISQLProvider, IDisposable
{
    private readonly string _connectionString;
    private MySqlConnection? _connection;

    public MySQLProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MySQLProvider(string server, string database, string username, string password)
    {
        _connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";
    }

    public void OpenConnection()
    {
        _connection ??= new MySqlConnection(_connectionString);

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
        using MySqlCommand cmd = new(query, _connection);

        AddParameters(cmd, parameters);

        return cmd.ExecuteNonQuery();
    }

    public object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
    {
        using MySqlCommand cmd = new(query, _connection);

        AddParameters(cmd, parameters);

        return cmd.ExecuteScalar();
    }

    public List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
    {
        using MySqlCommand cmd = new MySqlCommand(query, _connection);
        AddParameters(cmd, parameters);

        using MySqlDataReader reader = cmd.ExecuteReader();

        List<Dictionary<string, object>> result = new ();

        while (reader.Read())
        {
            Dictionary<string, object> row = new ();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }

            result.Add(row);
        }
        return result;
    }

    private static void AddParameters(MySqlCommand cmd, Dictionary<string, object>? parameters)
    {
        if (parameters == null)
        {
            return;
        }

        foreach (KeyValuePair<string, object> param in parameters)
        {
            // Ensure parameter names are prefixed with '@' if needed.
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
