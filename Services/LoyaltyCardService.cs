using munch_stamp.Models;

namespace munch_stamp.Services;

public class LoyaltyCardService
{
    public enum VisitRegistrationOutcome { Success, CardNotFound, TooSoon }
    public record VisitRegistrationResult(VisitRegistrationOutcome Outcome, LoyaltyCard? Card);

    public async Task<List<LoyaltyCard>> LoadAllAsync()
    {
        var cards = await DatabaseService.Connection.Table<LoyaltyCard>()
            .Where(c => c.IsActive)
            .ToListAsync();

        foreach (var card in cards)
            card.Visits = await LoadVisitsAsync(card.Id);

        return cards;
    }

    public async Task AddAsync(LoyaltyCard card)
    {
        await DatabaseService.Connection.InsertAsync(card);
    }

    public async Task<VisitRegistrationResult> RegisterVisitAsync(string qrCodeId)
    {
        var card = await DatabaseService.Connection.Table<LoyaltyCard>()
            .Where(c => c.QrCodeId == qrCodeId && c.IsActive)
            .FirstOrDefaultAsync();

        if (card is null)
            return new VisitRegistrationResult(VisitRegistrationOutcome.CardNotFound, null);

        card.Visits = await LoadVisitsAsync(card.Id);
        var lastVisit = card.Visits.OrderByDescending(v => v.Timestamp).FirstOrDefault();

        if (lastVisit is not null && DateTime.UtcNow - lastVisit.Timestamp < TimeSpan.FromSeconds(60))
            return new VisitRegistrationResult(VisitRegistrationOutcome.TooSoon, card);

        var visit = new Visit { LoyaltyCardId = card.Id };
        await DatabaseService.Connection.InsertAsync(visit);
        card.Visits.Add(visit);

        return new VisitRegistrationResult(VisitRegistrationOutcome.Success, card);
    }

    public async Task RedeemRewardAsync(string cardId)
    {
        await DatabaseService.Connection.Table<Visit>()
            .Where(v => v.LoyaltyCardId == cardId)
            .DeleteAsync();
    }

    private async Task<List<Visit>> LoadVisitsAsync(string cardId)
    {
        return await DatabaseService.Connection.Table<Visit>()
            .Where(v => v.LoyaltyCardId == cardId)
            .ToListAsync();
    }
}