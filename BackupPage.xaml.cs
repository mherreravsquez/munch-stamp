using munch_stamp.Services;

namespace munch_stamp;

public partial class BackupPage : ContentPage
{
    public BackupPage()
    {
        InitializeComponent();
        ApplyTranslations();
    }

    private void ApplyTranslations()
    {
        Title = LocalizationService.Get("BackupTab");
        ExportButton.Text = $"{LocalizationService.Get("ExportButton")}";
        SaveToDeviceButton.Text = $"{LocalizationService.Get("SaveToDeviceButton")}"; // new
        RestoreButton.Text = $"{LocalizationService.Get("RestoreButton")}";
    }

    private async void OnSaveToDeviceClicked(object? sender, EventArgs e)
    {
        try
        {
            var saved = await BackupService.SaveToDeviceAsync();
            if (saved) RefreshLastBackupLabel();
        }
        catch (Exception ex)
        {
            await DisplayAlert(LocalizationService.Get("BackupFailedTitle"), ex.Message, "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TabBar.Refresh("backup");
        RefreshLastBackupLabel();
    }

    private void RefreshLastBackupLabel()
    {
        var last = BackupService.GetLastBackupTime();
        LastBackupLabel.Text = last is null
            ? LocalizationService.Get("NoBackupYetLabel")
            : $"{LocalizationService.Get("LastBackupLabel")}: {last:g}";
    }

    private async void OnExportClicked(object? sender, EventArgs e)
    {
        try
        {
            await BackupService.ExportAsync();
            RefreshLastBackupLabel();
        }
        catch (Exception ex)
        {
            await DisplayAlert(LocalizationService.Get("BackupFailedTitle"), ex.Message, "OK");
        }
    }

    private async void OnRestoreClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            LocalizationService.Get("RestoreConfirmTitle"),
            LocalizationService.Get("RestoreConfirmMessage"),
            "OK", "Cancel");
        if (!confirmed) return;

        try
        {
            var restored = await BackupService.RestoreAsync();
            if (restored)
                await DisplayAlert(LocalizationService.Get("BackupSuccessTitle"), LocalizationService.Get("RestoreSuccessMessage"), "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(LocalizationService.Get("RestoreFailedTitle"), ex.Message, "OK");
        }
    }
}