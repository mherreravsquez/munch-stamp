using munch_stamp.Models;
using munch_stamp.Services;

namespace munch_stamp;

public partial class BusinessProfilePage : ContentPage
{
    private readonly BusinessProfileService _profileService = new();
    private Business _currentBusiness = new();

    public BusinessProfilePage()
    {
        InitializeComponent();
        ApplyTranslations();
        Loaded += OnPageLoaded;
    }

    private void ApplyTranslations()
    {
        Title = LocalizationService.Get("BusinessTab");
        BusinessNameLabelText.Text = LocalizationService.Get("BusinessNameLabel");
        NameEntry.Placeholder = LocalizationService.Get("BusinessNamePlaceholder");
        CategoryLabelText.Text = LocalizationService.Get("CategoryLabel");
        CategoryEntry.Placeholder = LocalizationService.Get("CategoryPlaceholder");
        VisitsRequiredLabelText.Text = LocalizationService.Get("VisitsRequiredLabel");
        RewardLabelText.Text = LocalizationService.Get("RewardLabel");
        RewardEntry.Placeholder = LocalizationService.Get("RewardPlaceholder");
        SaveButton.Text = LocalizationService.Get("SaveButton");
    }
    
    private async void OnEnglishClicked(object? sender, EventArgs e) => await ChangeLanguage("en");
    private async void OnSpanishClicked(object? sender, EventArgs e) => await ChangeLanguage("es");

    private async Task ChangeLanguage(string code)
    {
        if (code == LocalizationService.CurrentLanguage) return;

        await LocalizationService.SetLanguageAsync(code);
        await DisplayAlert(
            LocalizationService.Get("SavedTitle"),
            LocalizationService.Get("RestartNoticeMessage"),
            "OK");
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        var existing = await _profileService.LoadAsync();
        if (existing is not null)
        {
            _currentBusiness = existing;
            NameEntry.Text = existing.Name;
            CategoryEntry.Text = existing.Category;
            RewardEntry.Text = existing.DefaultReward;
            VisitsRequiredValueLabel.Text = existing.DefaultVisitsRequired.ToString();
        }
    }
    
    private void OnVisitsRequiredChanged(object? sender, ValueChangedEventArgs e)
    {
        VisitsRequiredValueLabel.Text = ((int)e.NewValue).ToString();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Missing name", "Please enter a business name.", "OK");
            return;
        }

        _currentBusiness.Name = NameEntry.Text;
        _currentBusiness.Category = CategoryEntry.Text;
        _currentBusiness.DefaultReward = RewardEntry.Text;
        _currentBusiness.DefaultVisitsRequired = int.Parse(VisitsRequiredValueLabel.Text);

        await _profileService.SaveAsync(_currentBusiness);
        await DisplayAlert("Saved", "Business profile saved.", "OK");
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
}