using System.Text.Json;
using munch_stamp.Models;

namespace munch_stamp.Services;

// Saves and loads the single Business record as a local JSON file.
// This is a deliberately simple stand-in for a real database — we'll
// replace it with SQLite in Milestone 7, once Business, LoyaltyCard,
// and Visit all need to relate to each other.
public class BusinessProfileService
{
    private readonly string _filePath;

    public BusinessProfileService()
    {
        // FileSystem.AppDataDirectory is a folder MAUI gives every app,
        // private to it, that survives app restarts (but is wiped if the
        // app is uninstalled). Equivalent idea to a browser's IndexedDB
        // origin storage, or a save file next to a Unity build.
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "business.json");
    }

    public async Task SaveAsync(Business business)
    {
        var json = JsonSerializer.Serialize(business);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<Business?> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return null;

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<Business>(json);
    }
}