using QRCoder;
using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class LoyaltyCardDetailPage : ContentPage
{
    public LoyaltyCardDetailPage(LoyaltyCard card)
    {
        InitializeComponent();
        ApplyTranslations();

        Title = LocalizationService.Get("CardDetailsTitle");
        CustomerNameLabel.Text = card.CustomerName;
        ProgressLabel.Text = card.ProgressText;
        RewardLabel.Text = card.RewardDescription;

        // The QR code encodes only the card's QrCodeId — never the
        // customer's name or contact info. Scanning it later just
        // gives the app a random-looking ID to look up, nothing more.
        QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(GenerateQrCodePng(card.QrCodeId)));
    }

    private void ApplyTranslations()
    {
        QrCodeSectionLabelText.Text = LocalizationService.Get("QrCodeSectionLabel");
    }

    // Generates a QR code as PNG bytes for the given text.
    // ECCLevel.Q = error-correction level "Quartile" (~25% recoverable),
    // a reasonable default for a code that might get scuffed on a phone screen.
    private static byte[] GenerateQrCodePng(string content)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(data);
        return pngQrCode.GetGraphic(20); // 20 = pixels per QR "module" (block)
    }
}