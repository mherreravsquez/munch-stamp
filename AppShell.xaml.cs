using munch_stamp.Services;

namespace munch_stamp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        HomeTabContent.Title = LocalizationService.Get("HomeTab");
        CardsTabContent.Title = LocalizationService.Get("CardsTab");
        ScanTabContent.Title = LocalizationService.Get("ScanTab");
        BusinessTabContent.Title = LocalizationService.Get("BusinessTab");
        BackupTabContent.Title = LocalizationService.Get("BackupTab");
    }
}