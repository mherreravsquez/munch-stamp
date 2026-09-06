using CommunityToolkit.Maui.Storage;

namespace munch_stamp.Services;

// Local, no-account backup: export the SQLite file through the OS
// share sheet (same mechanism as sharing a QR code in Milestone 9),
// so the admin can save it to Files, email it, send it via WhatsApp —
// whatever they already know how to use. Restore reverses this via
// the OS file picker, no special storage permissions needed either way.
public static class BackupService
{
    private const string LastBackupKey = "last_backup_at";
    private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "munchstamp.db3");

    public static async Task ExportAsync()
    {
        var fileName = $"munchstamp-backup-{DateTime.Now:yyyy-MM-dd}.db3";
        var exportPath = Path.Combine(FileSystem.CacheDirectory, fileName);
        File.Copy(DbPath, exportPath, overwrite: true);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = LocalizationService.Get("ShareBackupTitle"),
            File = new ShareFile(exportPath)
        });

        Preferences.Set(LastBackupKey, DateTime.UtcNow.ToString("O"));
    }

    public static DateTime? GetLastBackupTime()
    {
        var saved = Preferences.Get(LastBackupKey, string.Empty);
        return string.IsNullOrEmpty(saved) ? null : DateTime.Parse(saved).ToLocalTime();
    }

    // Returns false if the user cancelled the picker (not an error).
    public static async Task<bool> RestoreAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = LocalizationService.Get("SelectBackupFileTitle")
        });

        if (result is null) return false;

        using var sourceStream = await result.OpenReadAsync();
        using var destStream = File.Create(DbPath);
        await sourceStream.CopyToAsync(destStream);
        destStream.Close();

        // Reconnect so the app actually sees the restored data.
        await DatabaseService.InitializeAsync();
        return true;
    }
    
    public static async Task<bool> SaveToDeviceAsync()
    {
        var fileName = $"munchstamp-backup-{DateTime.Now:yyyy-MM-dd}.db3";
        using var stream = File.OpenRead(DbPath);

        var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);
        if (result.IsSuccessful)
            Preferences.Set(LastBackupKey, DateTime.UtcNow.ToString("O"));

        return result.IsSuccessful;
    }
}