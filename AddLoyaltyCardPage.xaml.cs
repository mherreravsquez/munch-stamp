using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class AddLoyaltyCardPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();

    public AddLoyaltyCardPage()
    {
        InitializeComponent();
        ApplyTranslations();
    }

    private void ApplyTranslations()
    {
        Title = LocalizationService.Get("NewLoyaltyCardTitle");
        CustomerNameLabelText.Text = LocalizationService.Get("CustomerNameLabel");
        CustomerNameEntry.Placeholder = LocalizationService.Get("CustomerNamePlaceholder");
        CustomerContactLabelText.Text = LocalizationService.Get("CustomerContactLabel");
        CustomerContactEntry.Placeholder = LocalizationService.Get("CustomerContactPlaceholder");
        VisitsRequiredLabelText.Text = LocalizationService.Get("VisitsRequiredLabel");
        RewardLabelText.Text = LocalizationService.Get("RewardLabel");
        RewardEntry.Placeholder = LocalizationService.Get("RewardPlaceholder");
        CreateCardButtonControl.Text = LocalizationService.Get("CreateCardButton");
    }

    private void OnVisitsRequiredChanged(object? sender, ValueChangedEventArgs e)
    {
        VisitsRequiredValueLabel.Text = ((int)e.NewValue).ToString();
    }

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text))
        {
            await DisplayAlert(
                LocalizationService.Get("MissingNameTitle"),
                LocalizationService.Get("MissingNameMessage"),
                "OK");
            return;
        }

        var card = new LoyaltyCard
        {
            CustomerName = CustomerNameEntry.Text,
            CustomerContact = CustomerContactEntry.Text,
            VisitsRequired = (int)VisitsRequiredStepper.Value,
            RewardDescription = RewardEntry.Text
        };

        await _cardService.AddAsync(card);
        await Navigation.PopAsync();
    }
}