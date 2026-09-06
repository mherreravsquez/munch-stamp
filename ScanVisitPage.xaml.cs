using BarcodeScanning;
using munch_stamp.Services;
using System.Linq;

namespace munch_stamp;

public partial class ScanVisitPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();
    private bool _isAnimating;

    public ScanVisitPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
        // Simulate scan on screen tap (instead of command)
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnScannerTapped;
        Scanner.GestureRecognizers.Add(tapGesture);
        // Also attach the gesture to the parent grid
        var parentGrid = (Grid)Scanner.Parent;
        parentGrid.GestureRecognizers.Add(tapGesture);
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        // Start scan animation
        StartScanAnimation();
    }

    private async void StartScanAnimation()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        var startY = -105;
        var endY = 105;
        while (_isAnimating)
        {
            await ScanLine.TranslateTo(0, startY, 800, Easing.SinInOut);
            await ScanLine.TranslateTo(0, endY, 800, Easing.SinInOut);
        }
    }

    private void StopScanAnimation()
    {
        _isAnimating = false;
        ScanLine.TranslateTo(0, 0, 0);
    }

    private async void OnScannerTapped(object sender, EventArgs e)
    {
        await SimulateScan();
    }

    private async Task SimulateScan()
    {
        // Simulate a known QR (for example, the first card)
        // In a real case, process the scanned code here.
        var cards = await _cardService.LoadAllAsync();
        if (cards.Count == 0)
        {
            await DisplayAlert("Sin tarjetas", "No hay tarjetas activas para escanear.", "OK");
            return;
        }

        var qrId = cards.First().QrCodeId;
        var result = await _cardService.RegisterVisitAsync(qrId);
        if (result.Outcome == LoyaltyCardService.VisitRegistrationOutcome.Success)
        {
            await DisplayAlert("Éxito", $"Visita registrada para {result.Card!.CustomerName}.", "OK");
        }
        else if (result.Outcome == LoyaltyCardService.VisitRegistrationOutcome.TooSoon)
        {
            await DisplayAlert("Aviso", "Debes esperar al menos 60 segundos.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "No se pudo registrar la visita.", "OK");
        }
    }

    // Camera detection handler: processes detected barcodes and registers visits
    private async void OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
    {
        var results = e?.BarcodeResults;
        if (results == null || !results.Any())
            return;

        var firstResult = results.FirstOrDefault();
        if (firstResult is null) return;

        var qrCode = firstResult.RawValue;
        var result = await _cardService.RegisterVisitAsync(qrCode);

        if (result.Outcome == LoyaltyCardService.VisitRegistrationOutcome.Success)
        {
            await DisplayAlert("Éxito", $"Visita registrada para {result.Card!.CustomerName}.", "OK");
        }
        else if (result.Outcome == LoyaltyCardService.VisitRegistrationOutcome.TooSoon)
        {
            await DisplayAlert("Aviso", "Debes esperar al menos 60 segundos.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "No se pudo registrar la visita.", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Enable camera when page appears
        Scanner.CameraEnabled = true;
    }

    protected override void OnDisappearing()
    {
        // Disable camera and stop animation when leaving
        Scanner.CameraEnabled = false;
        StopScanAnimation();
        base.OnDisappearing();
    }
}