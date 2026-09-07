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
        VisitsRequiredValueLabel.Text = "10";
    }

    private void ApplyTranslations()
    {
        EyebrowLabel.Text = LocalizationService.Get("AddCardEyebrow");
        TitleLabel.Text = LocalizationService.Get("NewLoyaltyCardTitle");
        CustomerNameLabelText.Text = LocalizationService.Get("CustomerNameLabel");
        CustomerNameEntry.Placeholder = LocalizationService.Get("CustomerNamePlaceholder");
        CustomerContactLabelText.Text = LocalizationService.Get("CustomerContactLabel");
        CustomerContactEntry.Placeholder = LocalizationService.Get("CustomerContactPlaceholder");
        VisitsRequiredLabelText.Text = LocalizationService.Get("VisitsRequiredLabel");
        RewardLabelText.Text = LocalizationService.Get("RewardLabel");
        RewardEntry.Placeholder = LocalizationService.Get("RewardPlaceholder");
        CreateButton.Text = LocalizationService.Get("CreateCardButton");
    }

    private void OnDecrementVisits(object sender, EventArgs e)
    {
        int val = int.Parse(VisitsRequiredValueLabel.Text);
        if (val > 1) VisitsRequiredValueLabel.Text = (val - 1).ToString();
    }

    private void OnIncrementVisits(object sender, EventArgs e)
    {
        int val = int.Parse(VisitsRequiredValueLabel.Text);
        if (val < 50) VisitsRequiredValueLabel.Text = (val + 1).ToString();
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text))
        {
            await DisplayAlert(
                LocalizationService.Get("ErrorTitle"),
                LocalizationService.Get("CustomerNameRequiredMessage"),
                "OK");
            return;
        }

        var card = new LoyaltyCard
        {
            CustomerName = CustomerNameEntry.Text,
            CustomerContact = CustomerContactEntry.Text ?? string.Empty,
            VisitsRequired = int.Parse(VisitsRequiredValueLabel.Text),
            RewardDescription = RewardEntry.Text ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await _cardService.AddAsync(card);
        await DisplayAlert(
            LocalizationService.Get("SuccessTitle"),
            LocalizationService.Get("CardCreatedMessage"),
            "OK");
        await Navigation.PopAsync();
    }
}