using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class LoyaltyCardListPage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();

    public LoyaltyCardListPage()
    {
        InitializeComponent();
        Loaded += async (s, e) => await RefreshList();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshList();
    }

    private async Task RefreshList()
    {
        var cards = await _cardService.LoadAllAsync();
        CardsCollectionView.ItemsSource = cards.Where(c => c.IsActive).ToList();
    }

    private async void OnAddCardClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddLoyaltyCardPage());
    }
}