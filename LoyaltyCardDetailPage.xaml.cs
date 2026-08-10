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
        ApplyTranslations();
        _card = card;
        RefreshDisplay();
    }

    private void ApplyTranslations()
    {
        Title = LocalizationService.Get("CardDetailsTitle");
        QrCodeSectionLabelText.Text = LocalizationService.Get("QrCodeSectionLabel");
        RedeemButton.Text = LocalizationService.Get("RedeemButton");
    }

    private void RefreshDisplay()
    {
        CustomerNameLabel.Text = _card.CustomerName;
        ProgressLabel.Text = _card.ProgressText;
        RewardLabel.Text = _card.RewardDescription;
        QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(GenerateQrCodePng(_card.QrCodeId)));

        RedeemButton.IsVisible = RewardService.HasEarnedReward(_card);
    }

    private async void OnRedeemClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            LocalizationService.Get("RedeemConfirmTitle"),
            LocalizationService.Get("RedeemConfirmMessage"),
            "OK", "Cancel");

        if (!confirmed) return;

        await _cardService.RedeemRewardAsync(_card.Id);

        // Reload this specific card fresh from storage so _card reflects the reset.
        var allCards = await _cardService.LoadAllAsync();
        _card = allCards.First(c => c.Id == _card.Id);

        RefreshDisplay();
        await DisplayAlert(LocalizationService.Get("RewardEarnedTitle"), LocalizationService.Get("RedeemedMessage"), "OK");
    }

    private static byte[] GenerateQrCodePng(string content)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(data);
        return pngQrCode.GetGraphic(20);
    }
}