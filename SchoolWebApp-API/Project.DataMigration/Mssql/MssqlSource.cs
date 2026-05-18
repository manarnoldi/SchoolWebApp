using Microsoft.Data.SqlClient;

namespace Project.DataMigration.Mssql;

// Thin wrapper around SqlConnection that owns the connection lifetime and exposes
// strongly-typed readers for the legacy 'swikunda' MSSQL schema. Each method opens
// a reader, maps rows via a delegate, and yields strongly-typed records.
public sealed class MssqlSource : IAsyncDisposable
{
    private readonly SqlConnection _conn;

    public MssqlSource(string connectionString)
    {
        _conn = new SqlConnection(connectionString);
    }

    public async Task OpenAsync(CancellationToken ct = default) => await _conn.OpenAsync(ct);

    public async Task<List<T>> QueryAsync<T>(string sql, Func<SqlDataReader, T> map, CancellationToken ct = default)
    {
        await using var cmd = new SqlCommand(sql, _conn) { CommandTimeout = 120 };
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        var rows = new List<T>();
        while (await reader.ReadAsync(ct)) rows.Add(map(reader));
        return rows;
    }

    public async ValueTask DisposeAsync() => await _conn.DisposeAsync();

    // Reader helpers -------------------------------------------------------
    public static string? GetNullableString(SqlDataReader r, string col)
        => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetString(r.GetOrdinal(col));

    public static int? GetNullableInt(SqlDataReader r, string col)
    {
        var i = r.GetOrdinal(col);
        if (r.IsDBNull(i)) return null;
        return Convert.ToInt32(r.GetValue(i));
    }

    public static long GetLong(SqlDataReader r, string col) => r.GetInt64(r.GetOrdinal(col));

    public static DateTime? GetNullableDateTime(SqlDataReader r, string col)
    {
        var i = r.GetOrdinal(col);
        if (r.IsDBNull(i)) return null;
        return Convert.ToDateTime(r.GetValue(i));
    }

    public static bool? GetNullableBool(SqlDataReader r, string col)
    {
        var i = r.GetOrdinal(col);
        if (r.IsDBNull(i)) return null;
        return r.GetBoolean(i);
    }
}
