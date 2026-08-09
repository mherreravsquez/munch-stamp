using System.Text.Json;
using munch_stamp.Models;

namespace munch_stamp.Services;

public class LoyaltyCardService
{
    private readonly string _filePath;

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
}