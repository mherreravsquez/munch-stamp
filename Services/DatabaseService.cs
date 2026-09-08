using SQLite;
using munch_stamp.Models;

namespace munch_stamp.Services;

public static class DatabaseService
{
    private static SQLiteAsyncConnection? _connection;

    public static SQLiteAsyncConnection Connection
    {
        get
        {
            if (_connection is null)
                throw new InvalidOperationException("DatabaseService.InitializeAsync() must run before use.");
            return _connection;
        }
    }

    public static async Task InitializeAsync()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "munchstamp.db3");
        _connection = new SQLiteAsyncConnection(dbPath);

        await _connection.CreateTableAsync<Business>();

        // Migration: add columns if they don't exist yet.
        // Keyed by column name -> SQLite column type.
        var columnsToAdd = new Dictionary<string, string>
        {
            ["CardBackgroundColor"] = "TEXT",
            ["CardGradient1Color"] = "TEXT",
            ["CardGradient2Color"] = "TEXT",
            ["LogoImageBytes"] = "BLOB",
        };

        foreach (var (col, sqlType) in columnsToAdd)
        {
            var exists = await _connection.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM pragma_table_info('Business') WHERE name = '{col}'");
            if (exists == 0)
                await _connection.ExecuteAsync($"ALTER TABLE Business ADD COLUMN {col} {sqlType}");
        }

        await _connection.CreateTableAsync<LoyaltyCard>();
        await _connection.CreateTableAsync<Visit>();
    }
}