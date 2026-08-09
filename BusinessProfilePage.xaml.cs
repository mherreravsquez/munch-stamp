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
        Loaded += OnPageLoaded;
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
            VisitsRequiredStepper.Value = existing.DefaultVisitsRequired;
            VisitsRequiredLabel.Text = existing.DefaultVisitsRequired.ToString();
        }
    }

    private void OnVisitsRequiredChanged(object? sender, ValueChangedEventArgs e)
    {
        VisitsRequiredLabel.Text = ((int)e.NewValue).ToString();
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
        _currentBusiness.DefaultVisitsRequired = (int)VisitsRequiredStepper.Value;

        await _profileService.SaveAsync(_currentBusiness);
        await DisplayAlert("Saved", "Business profile saved.", "OK");
    }
}