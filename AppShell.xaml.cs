using munch_stamp.Services;

namespace munch_stamp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        UpdateTitles();
    }

    // Update tab titles when language changes at runtime
    public void UpdateTitles()
    {
        HomeTabContent.Title = LocalizationService.Get("HomeTab");
        CardsTabContent.Title = LocalizationService.Get("CardsTab");
        ScanTabContent.Title = LocalizationService.Get("ScanTab");
        BusinessTabContent.Title = LocalizationService.Get("BusinessTab");
        BackupTabContent.Title = LocalizationService.Get("BackupTab");
    }
}