using munch_stamp.Models;
using munch_stamp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Media;
using System.Threading.Tasks;

namespace munch_stamp;

public partial class BusinessProfilePage : ContentPage
{
    private readonly BusinessProfileService _profileService = new();
    private Business _currentBusiness = new();

    private Color _bgColor = Color.FromArgb("#1a1a2e");
    private Color _grad1Color = Color.FromArgb("#6c5ce7");
    private Color _grad2Color = Color.FromArgb("#fd79a8");

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
        PreviewButton.Text = LocalizationService.Get("PreviewButton");

        LogoLabelText.Text = LocalizationService.Get("BusinessLogoLabel");
        ChangeLogoButton.Text = LocalizationService.Get("ChangeLogoButton");
        RemoveLogoButton.Text = LocalizationService.Get("RemoveLogoButton");
        SemanticProperties.SetDescription(LogoPreviewBorder, LocalizationService.Get("ChangeLogoButton"));

        SemanticProperties.SetDescription(BgColorSwatch, LocalizationService.Get("BackgroundColorLabel"));
        SemanticProperties.SetDescription(Grad1ColorSwatch, LocalizationService.Get("Gradient1Label"));
        SemanticProperties.SetDescription(Grad2ColorSwatch, LocalizationService.Get("Gradient2Label"));
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

        _bgColor = ParseHexColor(_currentBusiness.CardBackgroundColor, "#1a1a2e");
        _grad1Color = ParseHexColor(_currentBusiness.CardGradient1Color, "#6c5ce7");
        _grad2Color = ParseHexColor(_currentBusiness.CardGradient2Color, "#fd79a8");
        RefreshColorSwatches();

        RefreshLogoPreview();
    }

    private void RefreshColorSwatches()
    {
        BgColorSwatch.BackgroundColor = _bgColor;
        BgColorHexLabel.Text = ToHex(_bgColor);

        Grad1ColorSwatch.BackgroundColor = _grad1Color;
        Grad1ColorHexLabel.Text = ToHex(_grad1Color);

        Grad2ColorSwatch.BackgroundColor = _grad2Color;
        Grad2ColorHexLabel.Text = ToHex(_grad2Color);
    }

    private async void OnBgColorTapped(object? sender, EventArgs e)
    {
        var picked = await PickColorAsync("BackgroundColorLabel", _bgColor);
        if (picked is Color color)
        {
            _bgColor = color;
            RefreshColorSwatches();
        }
    }

    private async void OnGrad1ColorTapped(object? sender, EventArgs e)
    {
        var picked = await PickColorAsync("Gradient1Label", _grad1Color);
        if (picked is Color color)
        {
            _grad1Color = color;
            RefreshColorSwatches();
        }
    }

    private async void OnGrad2ColorTapped(object? sender, EventArgs e)
    {
        var picked = await PickColorAsync("Gradient2Label", _grad2Color);
        if (picked is Color color)
        {
            _grad2Color = color;
            RefreshColorSwatches();
        }
    }

    private async Task<Color?> PickColorAsync(string titleKey, Color initial)
    {
        var page = new ColorPickerPage(titleKey, initial);
        await Navigation.PushModalAsync(page);
        return await page.GetResultAsync();
    }

    // Shared by the circle tap and the "Change logo" button.
    private async void OnLogoTapped(object? sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = LocalizationService.Get("ChangeLogoButton")
            });

            if (photo is null) return; // user cancelled

            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            _currentBusiness.LogoImageBytes = memoryStream.ToArray();
            RefreshLogoPreview();
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert(LocalizationService.Get("Error"), "Photo picking isn't supported on this device.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert(LocalizationService.Get("Error"), "Permission to access photos was denied.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(LocalizationService.Get("Error"), ex.Message, "OK");
        }
    }

    private void OnRemoveLogoClicked(object? sender, EventArgs e)
    {
        _currentBusiness.LogoImageBytes = null;
        RefreshLogoPreview();
    }

    private void RefreshLogoPreview()
    {
        var hasLogo = _currentBusiness.LogoImageBytes is { Length: > 0 };
        LogoPlaceholderLabel.IsVisible = !hasLogo;
        LogoPreviewImage.IsVisible = hasLogo;
        RemoveLogoButton.IsVisible = hasLogo;

        if (hasLogo)
        {
            var bytes = _currentBusiness.LogoImageBytes!;
            LogoPreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        }
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
        _currentBusiness.CardBackgroundColor = ToHex(_bgColor);
        _currentBusiness.CardGradient1Color = ToHex(_grad1Color);
        _currentBusiness.CardGradient2Color = ToHex(_grad2Color);
        // LogoImageBytes is already set directly on _currentBusiness by
        // OnLogoTapped/OnRemoveLogoClicked, so no extra work needed here.

        await _profileService.SaveAsync(_currentBusiness);
        await DisplayAlert(LocalizationService.Get("SavedTitle"), LocalizationService.Get("SavedMessage"), "OK");
    }

    private async void OnPreviewClicked(object? sender, EventArgs e)
    {
        _currentBusiness.CardBackgroundColor = ToHex(_bgColor);
        _currentBusiness.CardGradient1Color = ToHex(_grad1Color);
        _currentBusiness.CardGradient2Color = ToHex(_grad2Color);

        var cardView = new Controls.LoyaltyCardView
        {
            Business = _currentBusiness,
            CustomerName = NameEntry.Text ?? "Customer",
            QrCodeImageSource = ImageSource.FromFile("qr.png"),
            QrCodeId = Guid.NewGuid().ToString(),
            JoinedDate = DateTime.Now
        };

        if (_currentBusiness.LogoImageBytes is { Length: > 0 } logoBytes)
            cardView.LogoImageSource = ImageSource.FromStream(() => new MemoryStream(logoBytes));

        byte[]? pngBytes;
        try
        {
            pngBytes = await CardRenderService.RenderToPngAsync(PreviewRenderTarget, cardView);
        }
        catch (Exception ex)
        {
            await DisplayAlert(LocalizationService.Get("Error"), ex.Message, "OK");
            return;
        }

        if (pngBytes is null) return;

        double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        var previewImage = new Image
        {
            Source = ImageSource.FromStream(() => new MemoryStream(pngBytes)),
            Aspect = Aspect.AspectFit,
            MaximumWidthRequest = screenWidth * 0.85
        };

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
                            Children = { previewImage }
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