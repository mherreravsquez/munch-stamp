using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class AddLoyaltyCardPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();

    public AddLoyaltyCardPage()
    {
        InitializeComponent();
    }

    private void OnVisitsRequiredChanged(object? sender, ValueChangedEventArgs e)
    {
        VisitsRequiredLabel.Text = ((int)e.NewValue).ToString();
    }

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text))
        {
            await DisplayAlert("Missing name", "Please enter a customer name.", "OK");
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

        // Go back to the list — OnAppearing() there will refresh it.
        await Navigation.PopAsync();
    }
}