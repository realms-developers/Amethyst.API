namespace Amethyst.Storages.SQL;

public interface ISQLProvider
{
    void OpenConnection();

    void CloseConnection();

    int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null);

    object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null);

    List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object>? parameters = null);
}
