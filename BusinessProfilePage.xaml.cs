using munch_stamp.Models;
using munch_stamp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Threading.Tasks;

namespace munch_stamp;

public partial class BusinessProfilePage : ContentPage
{
    private readonly BusinessProfileService _profileService = new();
    private Business _currentBusiness = new();

    public BusinessProfilePage()
    {
        InitializeComponent();
        ApplyTranslations();
        Loaded += OnPageLoaded;
    }

    private void ApplyTranslations()
    {
        Title = LocalizationService.Get("BusinessTab");
        BusinessNameLabelText.Text = LocalizationService.Get("BusinessNameLabel");
        NameEntry.Placeholder = LocalizationService.Get("BusinessNamePlaceholder");
        CategoryLabelText.Text = LocalizationService.Get("CategoryLabel");
        CategoryEntry.Placeholder = LocalizationService.Get("CategoryPlaceholder");
        CardDesignLabelText.Text = LocalizationService.Get("CardDesignLabel");
        BackgroundColorLabelText.Text = LocalizationService.Get("BackgroundColorLabel");
        Gradient1LabelText.Text = LocalizationService.Get("Gradient1Label");
        Gradient2LabelText.Text = LocalizationService.Get("Gradient2Label");
        SaveButton.Text = LocalizationService.Get("SaveButton");
    }

    private async void OnEnglishClicked(object? sender, EventArgs e) => await ChangeLanguage("en");
    private async void OnSpanishClicked(object? sender, EventArgs e) => await ChangeLanguage("es");

    private async Task ChangeLanguage(string code)
    {
        if (code == LocalizationService.CurrentLanguage) return;

        await LocalizationService.SetLanguageAsync(code);
        ApplyTranslations();

        if (Shell.Current is AppShell appShell)
            appShell.UpdateTitles();

        await DisplayAlert(LocalizationService.Get("SavedTitle"), LocalizationService.Get("LanguageChangedMessage"), "OK");
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        var existing = await _profileService.LoadAsync();
        _currentBusiness = existing ?? new Business();

        NameEntry.Text = _currentBusiness.Name;
        CategoryEntry.Text = _currentBusiness.Category;

        // Use hex text entries for colors (BgColorEntry, Grad1ColorEntry, Grad2ColorEntry)
        BgColorEntry.Text = string.IsNullOrWhiteSpace(_currentBusiness.CardBackgroundColor) ? "#1a1a2e" : _currentBusiness.CardBackgroundColor;
        Grad1ColorEntry.Text = string.IsNullOrWhiteSpace(_currentBusiness.CardGradient1Color) ? "#6c5ce7" : _currentBusiness.CardGradient1Color;
        Grad2ColorEntry.Text = string.IsNullOrWhiteSpace(_currentBusiness.CardGradient2Color) ? "#fd79a8" : _currentBusiness.CardGradient2Color;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert(LocalizationService.Get("MissingNameTitle"), LocalizationService.Get("MissingNameMessage"), "OK");
            return;
        }

        _currentBusiness.Name = NameEntry.Text.Trim();
        _currentBusiness.Category = CategoryEntry.Text?.Trim() ?? string.Empty;
        // Parse the hex text entries into Color and store as normalized hex
        _currentBusiness.CardBackgroundColor = ToHex(ParseHexColor(BgColorEntry.Text, "#1a1a2e"));
        _currentBusiness.CardGradient1Color = ToHex(ParseHexColor(Grad1ColorEntry.Text, "#6c5ce7"));
        _currentBusiness.CardGradient2Color = ToHex(ParseHexColor(Grad2ColorEntry.Text, "#fd79a8"));

        await _profileService.SaveAsync(_currentBusiness);
        await DisplayAlert(LocalizationService.Get("SavedTitle"), LocalizationService.Get("SavedMessage"), "OK");
    }

        private async void OnPreviewClicked(object? sender, EventArgs e)
    {
        // Update _currentBusiness color properties from the entries (but do not save)
        _currentBusiness.CardBackgroundColor = string.IsNullOrWhiteSpace(BgColorEntry.Text) ? "#1a1a2e" : BgColorEntry.Text.Trim();
        _currentBusiness.CardGradient1Color = string.IsNullOrWhiteSpace(Grad1ColorEntry.Text) ? "#6c5ce7" : Grad1ColorEntry.Text.Trim();
        _currentBusiness.CardGradient2Color = string.IsNullOrWhiteSpace(Grad2ColorEntry.Text) ? "#fd79a8" : Grad2ColorEntry.Text.Trim();

        // Create a LoyaltyCardView populated with the business and a sample customer name
        var cardView = new Controls.LoyaltyCardView
        {
            Business = _currentBusiness,
            CustomerName = NameEntry.Text ?? "Customer",
            QrCodeImageSource = ImageSource.FromFile("qr.png")
        };

        // Compute a preview size that keeps the original aspect ratio (600x840 -> 1.4 height/width)
        double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        double previewWidth = Math.Min(360, Math.Max(200, screenWidth * 0.8));
        double previewHeight = previewWidth * (840.0 / 600.0);
        cardView.WidthRequest = previewWidth;
        cardView.HeightRequest = previewHeight;

        // Create a simple modal preview page
        var previewPage = new ContentPage
        {
            BackgroundColor = Color.FromArgb("#80000000"),
            Content = new Grid
            {
                Children =
                {
                    new ScrollView
                    {
                        Content = new VerticalStackLayout
                        {
                            Padding = 20,
                            Children = { cardView }
                        },
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    },
                    new Button
                    {
                        Text = "Close",
                        BackgroundColor = Colors.Transparent,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Start,
                        Margin = new Thickness(10)
                    }
                }
            }
        };

        // Close handler for the button
        ((Button)((Grid)previewPage.Content).Children[1]).Clicked += async (s, ea) => await Navigation.PopModalAsync();

        await Navigation.PushModalAsync(previewPage);
    }

    private static Color ParseHexColor(string? value, string fallback)
    {
        try
        {
            return Color.FromArgb(string.IsNullOrWhiteSpace(value) ? fallback : value);
        }
        catch
        {
            return Color.FromArgb(fallback);
        }
    }

    private static string ToHex(Color color)
    {
        var red = (int)Math.Round(color.Red * 255);
        var green = (int)Math.Round(color.Green * 255);
        var blue = (int)Math.Round(color.Blue * 255);
        return $"#{red:X2}{green:X2}{blue:X2}".ToLowerInvariant();
    }
}