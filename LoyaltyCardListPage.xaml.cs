using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class LoyaltyCardListPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();

    public LoyaltyCardListPage()
    {
        InitializeComponent();
        ApplyTranslations();
        Loaded += async (s, e) => await RefreshList();
    }

    private void ApplyTranslations()
    {
        EyebrowLabel.Text = LocalizationService.Get("LoyaltyEyebrow");
        TitleLabel.Text = LocalizationService.Get("CardsTab");
        // The "+" header button has no visible text, so give screen
        // readers a label describing what it does.
        SemanticProperties.SetDescription(AddCardButton, LocalizationService.Get("AddCardButton"));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshList();
    }

    private async Task RefreshList()
    {
        ApplyTranslations();
        TabBar.Refresh("cards");

        var cards = await _cardService.LoadAllAsync();
        CardsCollectionView.ItemsSource = cards.Where(c => c.IsActive).ToList();
    }

    private async void OnAddCardClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AddLoyaltyCardPage());
    }

    private async void OnCardSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LoyaltyCard selectedCard)
        {
            await Navigation.PushAsync(new LoyaltyCardDetailPage(selectedCard));
            CardsCollectionView.SelectedItem = null;
        }
    }
}
