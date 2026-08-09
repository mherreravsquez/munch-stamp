using munch_stamp.Services;

namespace munch_stamp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BusinessTabContent.Title = LocalizationService.Get("BusinessTab");
        CardsTabContent.Title = LocalizationService.Get("CardsTab");
    }
}