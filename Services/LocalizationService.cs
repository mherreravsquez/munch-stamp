using System.Globalization;
using System.Text.Json;

namespace munch_stamp.Services;

// Loads UI text for the current language from a JSON file bundled
// under Resources/Raw/Strings/. Static because the whole app needs
// the same language at once
public static class LocalizationService
{
    private const string PreferenceKey = "app_language";
    private static Dictionary<string, string> _strings = new();

    public static string CurrentLanguage { get; private set; } = "en";

    // Call this once, at app startup, before any page loads.
    public static async Task InitializeAsync()
    {
        var saved = Preferences.Get(PreferenceKey, string.Empty);

        string languageToLoad;
        if (!string.IsNullOrEmpty(saved))
        {
            languageToLoad = saved;
        }
        else
        {
            // No saved preference yet: guess from the device's own
            // language setting, falling back to English.
            var deviceLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            languageToLoad = deviceLanguage == "es" ? "es" : "en";
        }

        await SetLanguageAsync(languageToLoad);
    }

    public static async Task SetLanguageAsync(string languageCode)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync($"Strings/{languageCode}.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                   ?? new Dictionary<string, string>();

        CurrentLanguage = languageCode;
        Preferences.Set(PreferenceKey, languageCode);
    }

    // Falls back to the key itself if it's missing — makes a typo'd
    // or forgotten translation visible instead of crashing.
    public static string Get(string key) =>
        _strings.TryGetValue(key, out var value) ? value : key;
}