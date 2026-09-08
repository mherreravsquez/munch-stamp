using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp.Controls;

public partial class LoyaltyCardView : ContentView
{
    public static readonly BindableProperty BusinessProperty =
        BindableProperty.Create(nameof(Business), typeof(Business), typeof(LoyaltyCardView), null,
            propertyChanged: OnBusinessChanged);

    public static readonly BindableProperty CustomerNameProperty =
        BindableProperty.Create(nameof(CustomerName), typeof(string), typeof(LoyaltyCardView), string.Empty,
            propertyChanged: OnCustomerNameChanged);

    public static readonly BindableProperty QrCodeImageSourceProperty =
        BindableProperty.Create(nameof(QrCodeImageSource), typeof(ImageSource), typeof(LoyaltyCardView), null,
            propertyChanged: OnQrCodeImageSourceChanged);

    public static readonly BindableProperty LogoImageSourceProperty =
        BindableProperty.Create(nameof(LogoImageSource), typeof(ImageSource), typeof(LoyaltyCardView), null,
            propertyChanged: OnLogoImageSourceChanged);

    public static readonly BindableProperty QrCodeIdProperty =
        BindableProperty.Create(nameof(QrCodeId), typeof(string), typeof(LoyaltyCardView), string.Empty,
            propertyChanged: OnQrCodeIdChanged);

    public static readonly BindableProperty JoinedDateProperty =
        BindableProperty.Create(nameof(JoinedDate), typeof(DateTime?), typeof(LoyaltyCardView), null,
            propertyChanged: OnJoinedDateChanged);

    public Business? Business
    {
        get => (Business?)GetValue(BusinessProperty);
        set => SetValue(BusinessProperty, value);
    }

    public string CustomerName
    {
        get => (string)GetValue(CustomerNameProperty);
        set => SetValue(CustomerNameProperty, value);
    }

    public ImageSource? QrCodeImageSource
    {
        get => (ImageSource?)GetValue(QrCodeImageSourceProperty);
        set => SetValue(QrCodeImageSourceProperty, value);
    }

    public ImageSource? LogoImageSource
    {
        get => (ImageSource?)GetValue(LogoImageSourceProperty);
        set => SetValue(LogoImageSourceProperty, value);
    }

    // Full QrCodeId (the same value encoded in the QR itself) — shown as
    // plain text so an admin can still identify the customer's card by
    // eye/manual lookup even without the app (e.g. reading it off a
    // saved/shared image).
    public string QrCodeId
    {
        get => (string)GetValue(QrCodeIdProperty);
        set => SetValue(QrCodeIdProperty, value);
    }

    // When the customer received this card (LoyaltyCard.CreatedAt).
    public DateTime? JoinedDate
    {
        get => (DateTime?)GetValue(JoinedDateProperty);
        set => SetValue(JoinedDateProperty, value);
    }

    public LoyaltyCardView()
    {
        InitializeComponent();
    }

    private static void OnBusinessChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LoyaltyCardView)bindable;
        var business = (Business?)newValue;
        if (business is null)
        {
            control.BusinessNameLabel.Text = string.Empty;
            return;
        }

        control.BusinessNameLabel.Text = business.Name;
        var gradient = control.GradientBrush;
        if (gradient is not null && gradient.GradientStops.Count >= 3)
        {
            gradient.GradientStops[0].Color = Color.FromArgb(business.CardBackgroundColor);
            gradient.GradientStops[1].Color = Color.FromArgb(business.CardGradient1Color);
            gradient.GradientStops[2].Color = Color.FromArgb(business.CardGradient2Color);
        }

        if (business.LogoImageBytes is { Length: > 0 } bytes && control.LogoImageSource is null)
            control.LogoImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
    }

    private static void OnCustomerNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LoyaltyCardView)bindable;
        control.CustomerNameLabel.Text = (string)newValue;
    }

    private static void OnQrCodeImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LoyaltyCardView)bindable;
        control.QrCodeImage.Source = (ImageSource?)newValue;
    }

    private static void OnLogoImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LoyaltyCardView)bindable;
        var source = (ImageSource?)newValue;
        control.LogoImage.Source = source;
        control.LogoImage.IsVisible = source is not null;
        control.LogoPlaceholderLabel.IsVisible = source is null;
    }

    private static void OnQrCodeIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LoyaltyCardView)bindable;
        var id = (string?)newValue;
        control.CardIdLabel.Text = string.IsNullOrWhiteSpace(id)
            ? string.Empty
            : $"···· {LastSegment(id).ToUpperInvariant()}";
    }

// A standard GUID is 8-4-4-4-12 hex digits separated by hyphens.
// The last segment (12 chars) is unique enough for a human to search
// against a customer list without needing to reproduce the whole GUID.
    private static string LastSegment(string qrCodeId)
    {
        var lastDashIndex = qrCodeId.LastIndexOf('-');
        return lastDashIndex >= 0 && lastDashIndex < qrCodeId.Length - 1
            ? qrCodeId[(lastDashIndex + 1)..]
            : qrCodeId; // fallback if it's ever not a standard GUID
    }

    private static void OnJoinedDateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LoyaltyCardView)bindable;
        control.JoinedLabel.Text = FormatJoinedText((DateTime?)newValue);
    }

    private static string FormatJoinedText(DateTime? date)
    {
        if (date is null) return string.Empty;

        var culture = LocalizationService.CurrentLanguage == "es"
            ? new CultureInfo("es-ES")
            : new CultureInfo("en-US");

        var monthYear = date.Value.ToString("MMMM yyyy", culture);
        // CultureInfo lowercases month names in Spanish ("marzo") —
        // capitalize for consistent display regardless of language.
        monthYear = char.ToUpper(monthYear[0]) + monthYear[1..];

        return $"{LocalizationService.Get("JoinedPrefix")} {monthYear}";
    }
}