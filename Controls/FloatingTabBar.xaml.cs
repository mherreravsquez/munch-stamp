using munch_stamp.Services;

namespace munch_stamp.Controls;

// Drives tab navigation now that the native Shell TabBar is hidden
// (Shell.TabBarIsVisible="False" in AppShell.xaml). Each of the five
// root pages (Home, Cards, Scan, Business, Backup) embeds one instance
// of this control, floating over its own content, and calls Refresh(tag)
// on appearing so the correct tab lights up and labels stay translated.
//
// Navigation itself still goes through Shell — GoToAsync("//route") —
// so the existing TabBar/ShellContent routing in AppShell.xaml keeps
// working exactly as before; only the visual tab bar changed.
public partial class FloatingTabBar : ContentView
{
    public FloatingTabBar()
    {
        InitializeComponent();
    }

    public void Refresh(string activeTab)
    {
        HomeTabLabel.Text = LocalizationService.Get("HomeTab");
        CardsTabLabel.Text = LocalizationService.Get("CardsTab");
        ScanTabLabel.Text = LocalizationService.Get("ScanTab");
        BusinessTabLabel.Text = LocalizationService.Get("BusinessTab");
        BackupTabLabel.Text = LocalizationService.Get("BackupTab");

        SetState(HomeTabIcon, HomeTabLabel, activeTab == "home");
        SetState(CardsTabIcon, CardsTabLabel, activeTab == "cards");
        SetState(ScanTabIcon, ScanTabLabel, activeTab == "scan");
        SetState(BusinessTabIcon, BusinessTabLabel, activeTab == "business");
        SetState(BackupTabIcon, BackupTabLabel, activeTab == "backup");
    }

    private static void SetState(Label icon, Label label, bool active)
    {
        var color = active
            ? (Color)(Application.Current?.Resources["Primary"] ?? Colors.Orange)
            : (Color)(Application.Current?.Resources["TextMuted"] ?? Colors.Gray);
        icon.TextColor = color;
        label.TextColor = color;
    }

    private async void OnHomeTapped(object? sender, EventArgs e) => await GoTo("home");
    private async void OnCardsTapped(object? sender, EventArgs e) => await GoTo("cards");
    private async void OnScanTapped(object? sender, EventArgs e) => await GoTo("scan");
    private async void OnBusinessTapped(object? sender, EventArgs e) => await GoTo("business");
    private async void OnBackupTapped(object? sender, EventArgs e) => await GoTo("backup");

    private static async Task GoTo(string route)
    {
        if (Shell.Current is not null && Shell.Current.CurrentState.Location.OriginalString != $"//{route}")
            await Shell.Current.GoToAsync($"//{route}");
    }
}
