using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class HomePage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();

    public HomePage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        var allCards = await _cardService.LoadAllAsync();
        // Mostrar solo las primeras 5 (o las más recientes)
        var recent = allCards.OrderByDescending(c => c.CreatedAt).Take(5).ToList();
        CardsCollectionView.ItemsSource = recent;
    }

    private async void OnCardSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LoyaltyCard card)
        {
            // Abrir detalle
            var detailPage = new LoyaltyCardDetailPage(card);
            await Navigation.PushModalAsync(detailPage);
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}