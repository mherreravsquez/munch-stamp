using Microsoft.Maui.Controls.Shapes;

namespace munch_stamp;

public partial class ColorPickerPage : ContentPage
{
    // Small curated palette — enough variety without overwhelming a
    // small business owner who just wants "something that looks nice".
    private static readonly string[] PresetHexColors =
    {
        "#1a1a2e", "#0f3460", "#16213e", "#6c5ce7", "#a855f7",
        "#ec4899", "#fd79a8", "#ef4444", "#f97316", "#f59e0b",
        "#10b981", "#14b8a6", "#0ea5e9", "#334155", "#ffffff"
    };

    private readonly TaskCompletionSource<Color?> _tcs = new();
    private bool _suppressSliderEvents;

    public ColorPickerPage(string titleKey, Color initialColor)
    {
        InitializeComponent();
        TitleLabel.Text = Services.LocalizationService.Get(titleKey);

        BuildPresets();
        SetFromColor(initialColor);
    }

    // Caller awaits this; resolves with the chosen color, or null if
    // the user cancelled via the X button.
    public Task<Color?> GetResultAsync() => _tcs.Task;

    private void BuildPresets()
    {
        var lineColor = (Color)(Application.Current?.Resources["LineColor"] ?? Colors.Gray);

        foreach (var hex in PresetHexColors)
        {
            var color = Color.FromArgb(hex);
            var swatch = new Border
            {
                WidthRequest = 36,
                HeightRequest = 36,
                Margin = new Thickness(4),
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                StrokeThickness = 1,
                Stroke = lineColor,
                BackgroundColor = color
            };
            swatch.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => SetFromColor(color))
            });
            PresetsLayout.Children.Add(swatch);
        }
    }

    private void SetFromColor(Color color)
    {
        _suppressSliderEvents = true;

        HueSlider.Value = color.GetHue();
        SaturationSlider.Value = color.GetSaturation();
        LightnessSlider.Value = color.GetLuminosity();

        _suppressSliderEvents = false;
        RefreshPreview();
    }

    private void OnSliderChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_suppressSliderEvents) return;
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        var color = Color.FromHsla(HueSlider.Value, SaturationSlider.Value, LightnessSlider.Value);
        PreviewSwatch.BackgroundColor = color;
        HexLabel.Text = ToHex(color);
        HexEntry.Text = ToHex(color);
    }

    private void OnHexEntryCompleted(object? sender, EventArgs e)
    {
        var text = HexEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text)) return;

        if (!TryParseHex(text, out var color))
            return; // invalid hex typed — leave sliders/preview as they were

        SetFromColor(color);
    }

    private static bool TryParseHex(string text, out Color color)
    {
        if (!text.StartsWith('#'))
            text = "#" + text;

        // Accept #RGB, #RRGGBB, #AARRGGBB — reject anything else instead of
        // letting Color.FromArgb throw or silently misinterpret a partial string.
        var isValidLength = text.Length is 4 or 7 or 9;
        var isValidHex = text[1..].All(Uri.IsHexDigit);

        if (isValidLength && isValidHex)
        {
            color = Color.FromArgb(text);
            return true;
        }

        color = Colors.Transparent;
        return false;
    }

    private async void OnSelectClicked(object? sender, EventArgs e)
    {
        // The hex box may hold an edit the user made but never "confirmed"
        // (Unfocused/Completed may not have run yet if Select was tapped
        // straight from the keyboard) — parse it directly here so typing
        // a hex value and immediately tapping Select works reliably.
        Color color;
        if (!string.IsNullOrWhiteSpace(HexEntry.Text) && TryParseHex(HexEntry.Text.Trim(), out var typedColor))
        {
            color = typedColor;
        }
        else
        {
            color = Color.FromHsla(HueSlider.Value, SaturationSlider.Value, LightnessSlider.Value);
        }

        _tcs.TrySetResult(color);
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _tcs.TrySetResult(null);
        await Navigation.PopModalAsync();
    }

    private static string ToHex(Color color)
    {
        var red = (int)Math.Round(color.Red * 255);
        var green = (int)Math.Round(color.Green * 255);
        var blue = (int)Math.Round(color.Blue * 255);
        return $"#{red:X2}{green:X2}{blue:X2}".ToLowerInvariant();
    }
}