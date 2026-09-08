using CommunityToolkit.Maui.Views;
using QRCoder;
using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class LoyaltyCardDetailPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();
    private LoyaltyCard _card;

    public LoyaltyCardDetailPage(LoyaltyCard card)
    {
        InitializeComponent();
        _card = card;
        ApplyTranslations();
        RefreshDisplay();
    }

    private void ApplyTranslations()
    {
        EyebrowLabel.Text = LocalizationService.Get("CardDetailEyebrow");
        RegisterVisitButtonControl.Text = LocalizationService.Get("RegisterVisitButton");
        ShareButton.Text = LocalizationService.Get("ShareButton");
        UndoVisitButtonControl.Text = LocalizationService.Get("UndoVisitButton");
        RedeemButton.Text = LocalizationService.Get("RedeemButton");
        CloseButtonControl.Text = LocalizationService.Get("CloseButton");
    }

    private void RefreshDisplay()
    {
        CustomerNameLabel.Text = _card.CustomerName;
        ProgressLabel.Text = _card.ProgressText;
        RewardLabel.Text = _card.RewardDescription;
        ProgressBar.Progress = _card.ProgressPercent;
        QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(GenerateQrCodePng(_card.QrCodeId)));
        RedeemButton.IsVisible = RewardService.HasEarnedReward(_card);
    }

    private async void OnRegisterVisitClicked(object? sender, EventArgs e)
    {
        await _cardService.AddManualVisitAsync(_card.Id);
        await ReloadCardAsync();

        if (RewardService.HasEarnedReward(_card))
            await DisplayAlert(LocalizationService.Get("RewardEarnedTitle"), $"{_card.CustomerName}: {_card.RewardDescription}", "OK");
        else
            await DisplayAlert(LocalizationService.Get("VisitRegisteredTitle"), $"{_card.CustomerName}: {_card.ProgressText}", "OK");
    }

    private async void OnUndoVisitClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            LocalizationService.Get("UndoConfirmTitle"),
            LocalizationService.Get("UndoConfirmMessage"),
            "OK", "Cancel");
        if (!confirmed) return;

        var removed = await _cardService.RemoveLastVisitAsync(_card.Id);
        if (!removed)
        {
            await DisplayAlert(LocalizationService.Get("UndoConfirmTitle"), LocalizationService.Get("NoVisitsToUndoMessage"), "OK");
            return;
        }

        await ReloadCardAsync();
        await DisplayAlert(LocalizationService.Get("UndoConfirmTitle"), LocalizationService.Get("UndoSuccessMessage"), "OK");
    }

    private async void OnRedeemClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            LocalizationService.Get("RedeemConfirmTitle"),
            LocalizationService.Get("RedeemConfirmMessage"),
            "OK", "Cancel");
        if (!confirmed) return;

        await _cardService.RedeemRewardAsync(_card.Id);
        await ReloadCardAsync();
        await DisplayAlert(LocalizationService.Get("RewardEarnedTitle"), LocalizationService.Get("RedeemedMessage"), "OK");
    }

    private async void OnShareClicked(object? sender, EventArgs e)
    {
        var business = await new BusinessProfileService().LoadAsync();
        if (business is null)
        {
            await DisplayAlert(LocalizationService.Get("Error"),
                LocalizationService.Get("BusinessMissingMessage"), "OK");
            return;
        }

        var qrBytes = GenerateQrCodePng(_card.QrCodeId);
        var qrImageSource = ImageSource.FromStream(() => new MemoryStream(qrBytes));

        var cardView = new Controls.LoyaltyCardView
        {
            Business = business,
            CustomerName = _card.CustomerName,
            QrCodeImageSource = qrImageSource,
            QrCodeId = _card.QrCodeId,
            JoinedDate = _card.CreatedAt
        };

        try
        {
            var pngBytes = await CardRenderService.RenderToPngAsync(CardRenderTarget, cardView);
            if (pngBytes is null) return;

            var fileName = $"{SanitizeFileName(_card.CustomerName)}-card.png";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, pngBytes);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"{LocalizationService.Get("ShareTitle")} — {_card.CustomerName}",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(LocalizationService.Get("Error"), ex.Message, "OK");
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopAsync();

    private async Task ReloadCardAsync()
    {
        var allCards = await _cardService.LoadAllAsync();
        _card = allCards.First(c => c.Id == _card.Id);
        RefreshDisplay();
    }

    private static string SanitizeFileName(string input) =>
        string.Concat(input.Split(Path.GetInvalidFileNameChars()));

    private static byte[] GenerateQrCodePng(string content)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(data);
        return pngQrCode.GetGraphic(20);
    }
}