using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace munch_stamp.Controls;

// Replaces the old ProgressBar on card list items and the detail page.
// Pure code-behind control (no .xaml needed) — mirrors the mockup's
// ".stamp-dots" / ".stamp-dots.lg" rows: one small circle per visit
// required, filled solid once that visit has happened.
public class StampDotsView : ContentView
{
    public static readonly BindableProperty FilledProperty =
        BindableProperty.Create(nameof(Filled), typeof(int), typeof(StampDotsView), 0,
            propertyChanged: OnAnyPropertyChanged);

    public static readonly BindableProperty TotalProperty =
        BindableProperty.Create(nameof(Total), typeof(int), typeof(StampDotsView), 1,
            propertyChanged: OnAnyPropertyChanged);

    // "lg" variant used on the card detail page (bigger dots).
    public static readonly BindableProperty LargeProperty =
        BindableProperty.Create(nameof(Large), typeof(bool), typeof(StampDotsView), false,
            propertyChanged: OnAnyPropertyChanged);

    public int Filled
    {
        get => (int)GetValue(FilledProperty);
        set => SetValue(FilledProperty, value);
    }

    public int Total
    {
        get => (int)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }

    public bool Large
    {
        get => (bool)GetValue(LargeProperty);
        set => SetValue(LargeProperty, value);
    }

    private readonly FlexLayout _layout = new()
    {
        Wrap = FlexWrap.Wrap,
        JustifyContent = FlexJustify.Start,
        AlignItems = FlexAlignItems.Center
    };

    public StampDotsView()
    {
        Content = _layout;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        // Colors come from Application.Current.Resources, which isn't
        // guaranteed to be ready in the constructor — rebuild once we're
        // actually attached to the visual tree.
        Rebuild();
    }

    private static void OnAnyPropertyChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((StampDotsView)bindable).Rebuild();

    private void Rebuild()
    {
        _layout.Children.Clear();

        var accent = (Color)(Application.Current?.Resources["Primary"] ?? Colors.Orange);
        var line = (Color)(Application.Current?.Resources["LineColor"] ?? Colors.Gray);

        double size = Large ? 14 : 9;
        double strokeWidth = Large ? 2 : 1.5;
        double gap = Large ? 8 : 6;

        int total = Math.Max(Total, 0);
        for (int i = 1; i <= total; i++)
        {
            bool isFilled = i <= Filled;
            var dot = new Border
            {
                WidthRequest = size,
                HeightRequest = size,
                Margin = new Thickness(0, 0, gap, gap),
                StrokeThickness = strokeWidth,
                Stroke = isFilled ? accent : line,
                BackgroundColor = isFilled ? accent : Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = size / 2 }
            };
            _layout.Children.Add(dot);
        }
    }
}
