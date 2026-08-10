using munch_stamp.Services;

namespace munch_stamp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Show a blank page immediately — the UI thread is never blocked.
        MainPage = new ContentPage();

        // Fire off the real startup work without blocking anything.
        // "async void" is normally avoided (errors inside it can't be
        // awaited/caught by a caller), but at the app's root, with no
        // caller to report back to, it's the accepted exception.
        InitializeAppAsync();
    }

    private async void InitializeAppAsync()
    {
        await DatabaseService.InitializeAsync();
        await MigrationService.MigrateFromJsonIfNeededAsync();
        await LocalizationService.InitializeAsync();

        MainPage = new AppShell();
    }
}