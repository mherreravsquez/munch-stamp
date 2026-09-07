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
        SaveButton.Text = LocalizationService.Get("SaveButton");
    }

    private async void OnEnglishClicked(object? sender, EventArgs e) => await ChangeLanguage("en");
    private async void OnSpanishClicked(object? sender, EventArgs e) => await ChangeLanguage("es");

    private async Task ChangeLanguage(string code)
    {
        if (code == LocalizationService.CurrentLanguage) return;

        await LocalizationService.SetLanguageAsync(code);
        ApplyTranslations();

        if (Shell.Current is AppShell appShell)
            appShell.UpdateTitles();

        await DisplayAlert(LocalizationService.Get("SavedTitle"), LocalizationService.Get("LanguageChangedMessage"), "OK");
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        var existing = await _profileService.LoadAsync();
        if (existing is not null)
        {
            _currentBusiness = existing;
            NameEntry.Text = existing.Name;
            CategoryEntry.Text = existing.Category;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert(LocalizationService.Get("MissingNameTitle"), LocalizationService.Get("MissingNameMessage"), "OK");
            return;
        }

        _currentBusiness.Name = NameEntry.Text;
        _currentBusiness.Category = CategoryEntry.Text;

        await _profileService.SaveAsync(_currentBusiness);
        await DisplayAlert(LocalizationService.Get("SavedTitle"), LocalizationService.Get("SavedMessage"), "OK");
    }
}