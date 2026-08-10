using BarcodeScanning;
using munch_stamp.Services;

namespace munch_stamp;

public partial class ScanVisitPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();
    private bool _isProcessing; // guards against the same frame firing the event twice

    public ScanVisitPage()
    {
        InitializeComponent();
        ApplyTranslations();
    }

    private void ApplyTranslations()
    {
        Title = LocalizationService.Get("ScanVisitTitle");
        InstructionsLabel.Text = LocalizationService.Get("ScanInstructions");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        Scanner.CameraEnabled = status == PermissionStatus.Granted;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Scanner.CameraEnabled = false; // stop the camera when navigating away
    }

    private async void OnDetectionFinished(object? sender, OnDetectionFinishedEventArg e)
    {
        if (_isProcessing || e.BarcodeResults.Length == 0)
            return;

        _isProcessing = true;
        Scanner.CameraEnabled = false;

        var scannedValue = e.BarcodeResults[0].DisplayValue;
        var result = await _cardService.RegisterVisitAsync(scannedValue);

        switch (result.Outcome)
        {
            case LoyaltyCardService.VisitRegistrationOutcome.Success:
                await DisplayAlert(
                    LocalizationService.Get("VisitRegisteredTitle"),
                    $"{result.Card!.CustomerName}: {result.Card.ProgressText}",
                    "OK");
                break;

            case LoyaltyCardService.VisitRegistrationOutcome.TooSoon:
                await DisplayAlert(
                    LocalizationService.Get("DuplicateVisitTitle"),
                    LocalizationService.Get("DuplicateVisitMessage"),
                    "OK");
                break;

            case LoyaltyCardService.VisitRegistrationOutcome.CardNotFound:
                await DisplayAlert(
                    LocalizationService.Get("CardNotFoundTitle"),
                    LocalizationService.Get("CardNotFoundMessage"),
                    "OK");
                break;
        }

        _isProcessing = false;
        Scanner.CameraEnabled = true; // ready for the next scan
    }
}