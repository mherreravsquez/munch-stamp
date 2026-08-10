using System.Text.Json;
using munch_stamp.Models;

namespace munch_stamp.Services;

public class LoyaltyCardService
{
    private readonly string _filePath;
    
    public enum VisitRegistrationOutcome { Success, CardNotFound, TooSoon }

    public record VisitRegistrationResult(VisitRegistrationOutcome Outcome, LoyaltyCard? Card);

    public LoyaltyCardService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "loyalty_cards.json");
    }

    public async Task<List<LoyaltyCard>> LoadAllAsync()
    {
        if (!File.Exists(_filePath))
            return new List<LoyaltyCard>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<LoyaltyCard>>(json) ?? new List<LoyaltyCard>();
    }

    public async Task SaveAllAsync(List<LoyaltyCard> cards)
    {
        var json = JsonSerializer.Serialize(cards);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task AddAsync(LoyaltyCard card)
    {
        var cards = await LoadAllAsync();
        cards.Add(card);
        await SaveAllAsync(cards);
    }
    
    public async Task<VisitRegistrationResult> RegisterVisitAsync(string qrCodeId)
    {
        var cards = await LoadAllAsync();
        var card = cards.FirstOrDefault(c => c.QrCodeId == qrCodeId && c.IsActive);

        if (card is null)
            return new VisitRegistrationResult(VisitRegistrationOutcome.CardNotFound, null);

        var lastVisit = card.Visits.OrderByDescending(v => v.Timestamp).FirstOrDefault();
        if (lastVisit is not null && DateTime.UtcNow - lastVisit.Timestamp < TimeSpan.FromSeconds(60))
            return new VisitRegistrationResult(VisitRegistrationOutcome.TooSoon, card);

        card.Visits.Add(new Visit());
        await SaveAllAsync(cards);

        return new VisitRegistrationResult(VisitRegistrationOutcome.Success, card);
    }
    
    public async Task RedeemRewardAsync(string cardId)
    {
        var cards = await LoadAllAsync();
        var card = cards.FirstOrDefault(c => c.Id == cardId);
        if (card is null) return;

        card.Visits.Clear();
        await SaveAllAsync(cards);
    }
}