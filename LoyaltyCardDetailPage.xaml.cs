using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class LoyaltyCardDetailPage : ContentPage
{
    private readonly LoyaltyCard _card;
    private readonly LoyaltyCardService _cardService = new();

    public LoyaltyCardDetailPage(LoyaltyCard card)
    {
        InitializeComponent();
        _card = card;
        BindingContext = card;

        CustomerNameLabel.Text = card.CustomerName;
        ProgressLabel.Text = card.ProgressText;
        RewardLabel.Text = card.RewardDescription;
        ProgressBar.Progress = card.ProgressPercent;
    }

    private async void OnRegisterVisitClicked(object sender, EventArgs e)
    {
        var result = await _cardService.RegisterVisitAsync(_card.QrCodeId);
        if (result.Outcome == LoyaltyCardService.VisitRegistrationOutcome.Success)
        {
            // Actualizar UI
            ProgressLabel.Text = result.Card!.ProgressText;
            ProgressBar.Progress = result.Card.ProgressPercent;
            await DisplayAlert("Éxito", "Visita registrada.", "OK");
        }
        else if (result.Outcome == LoyaltyCardService.VisitRegistrationOutcome.TooSoon)
        {
            await DisplayAlert("Aviso", "Debes esperar al menos 60 segundos entre visitas.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "No se pudo registrar la visita.", "OK");
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}