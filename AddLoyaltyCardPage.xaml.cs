using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class AddLoyaltyCardPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();

    public AddLoyaltyCardPage()
    {
        InitializeComponent();
        // Inicializar el valor mostrado
        VisitsRequiredValueLabel.Text = "10";
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
            await DisplayAlert("Error", "El nombre del cliente es obligatorio.", "OK");
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
        await DisplayAlert("Éxito", "Tarjeta creada correctamente.", "OK");
        await Navigation.PopAsync();
    }
}