using SQLite;
using munch_stamp.Models;

namespace munch_stamp.Services;

// Owns the single shared database connection for the whole app.
// Every other service goes through this instead of opening its own
// connection — similar in spirit to a singleton ScriptableObject in
// Unity that everything else references rather than duplicating.
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
        
        // Migration: add color columns if they don't exist
        var columns = new[] { "CardBackgroundColor", "CardGradient1Color", "CardGradient2Color" };
        foreach (var col in columns)
        {
            var exists = await _connection.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM pragma_table_info('Business') WHERE name = '{col}'");
            if (exists == 0)
                await _connection.ExecuteAsync($"ALTER TABLE Business ADD COLUMN {col} TEXT");
        }
        
        await _connection.CreateTableAsync<LoyaltyCard>();
        await _connection.CreateTableAsync<Visit>();
    }
    
    
}