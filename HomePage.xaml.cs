using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class HomePage : ContentPage
{
    private readonly LoyaltyCardService _cardService = new();
    private readonly BusinessProfileService _profileService = new();

    public HomePage()
    {
        InitializeComponent();
        ApplyTranslations();
        Loaded += async (s, e) => await RefreshAsync();
    }

    private void ApplyTranslations()
    {
        HomeEyebrowLabel.Text = LocalizationService.Get("HomeEyebrow");
        HeroBrandLabel.Text = $"☕ {LocalizationService.Get("HeroBrand")}";
        HeroHeadlineLabel.Text = LocalizationService.Get("HeroHeadline");
        HeroSubtitleLabel.Text = LocalizationService.Get("HeroSubtitle");
        ActiveCustomersLabelText.Text = LocalizationService.Get("ActiveCustomersLabel");
        VisitsRecordedLabelText.Text = LocalizationService.Get("VisitsRecordedLabel");
        RecentActivityLabelText.Text = LocalizationService.Get("RecentActivityLabel");
        ViewAllLabelText.Text = LocalizationService.Get("ViewAllLabel");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        // Re-applied here too (not just in the constructor) so switching
        // language on the Business tab is reflected the next time this
        // tab appears, same as the tab bar's own labels below.
        ApplyTranslations();
        TabBar.Refresh("home");

        var business = await _profileService.LoadAsync();
        HomeTitleLabel.Text = string.IsNullOrWhiteSpace(business?.Name)
            ? LocalizationService.Get("HomeTitle")
            : business.Name;

        var cards = await _cardService.LoadAllAsync();

        ActiveCustomersValueLabel.Text = cards.Count.ToString();
        VisitsRecordedValueLabel.Text = cards.Sum(c => c.VisitsCount).ToString();

        CardsCollectionView.ItemsSource = cards
            .OrderByDescending(c => c.Visits.OrderByDescending(v => v.Timestamp).FirstOrDefault()?.Timestamp ?? c.CreatedAt)
            .Take(5)
            .ToList();
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
