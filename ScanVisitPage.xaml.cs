using BarcodeScanning;
using munch_stamp.Services;

namespace munch_stamp;

public partial class ScanVisitPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();
    private bool _isProcessing; // guards against the same frame firing the event twice
    private bool _isAnimating;

    public ScanVisitPage()
    {
        InitializeComponent();
        ApplyTranslations();
    }

    private void ApplyTranslations()
    {
        ScanEyebrowLabel.Text = LocalizationService.Get("ScanEyebrow");
        ScanTitleLabel.Text = LocalizationService.Get("ScanVisitTitle");
        ScanFrameHintLabel.Text = LocalizationService.Get("ScanFrameHint");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        Scanner.CameraEnabled = status == PermissionStatus.Granted;

        StartScanAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Scanner.CameraEnabled = false; // stop the camera when navigating away
        StopScanAnimation();
    }

    // Purely decorative — runs continuously while the page is visible,
    // independent of any actual scan attempt. Not tied to input.
    private async void StartScanAnimation()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        while (_isAnimating)
        {
            await ScanLine.TranslateTo(0, -105, 800, Easing.SinInOut);
            if (!_isAnimating) break;
            await ScanLine.TranslateTo(0, 105, 800, Easing.SinInOut);
        }
    }

    private void StopScanAnimation()
    {
        _isAnimating = false;
        ScanLine.TranslateTo(0, 0, 0);
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
                if (RewardService.HasEarnedReward(result.Card!))
                {
                    await DisplayAlert(
                        LocalizationService.Get("RewardEarnedTitle"),
                        $"{result.Card.CustomerName}: {result.Card.RewardDescription}",
                        "OK");
                }
                else
                {
                    await DisplayAlert(
                        LocalizationService.Get("VisitRegisteredTitle"),
                        $"{result.Card.CustomerName}: {result.Card.ProgressText}",
                        "OK");
                }
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