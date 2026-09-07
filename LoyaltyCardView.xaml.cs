using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using munch_stamp.Models;

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
}