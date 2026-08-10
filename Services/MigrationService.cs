using System.Text.Json;
using munch_stamp.Models;

namespace munch_stamp.Services;

// Runs once: if old JSON files exist and the database is still empty,
// imports their data into SQLite, then renames the JSON files so this
// never runs again. Safe to call every startup — it's a no-op once done.
public static class MigrationService
{
    public static async Task MigrateFromJsonIfNeededAsync()
    {
        var businessJsonPath = Path.Combine(FileSystem.AppDataDirectory, "business.json");
        var cardsJsonPath = Path.Combine(FileSystem.AppDataDirectory, "loyalty_cards.json");

        var alreadyHasData = await DatabaseService.Connection.Table<Business>().CountAsync() > 0;
        if (alreadyHasData) return;

        if (File.Exists(businessJsonPath))
        {
            var json = await File.ReadAllTextAsync(businessJsonPath);
            var business = JsonSerializer.Deserialize<Business>(json);
            if (business is not null)
                await DatabaseService.Connection.InsertOrReplaceAsync(business);

            File.Move(businessJsonPath, businessJsonPath + ".migrated");
        }

        if (File.Exists(cardsJsonPath))
        {
            var json = await File.ReadAllTextAsync(cardsJsonPath);
            var cards = JsonSerializer.Deserialize<List<LoyaltyCard>>(json) ?? new();

            foreach (var card in cards)
            {
                await DatabaseService.Connection.InsertAsync(card);
                // Old JSON model embedded visits directly on the card;
                // migrate each one into its own new Visit row.
                foreach (var visit in card.Visits)
                {
                    visit.LoyaltyCardId = card.Id;
                    await DatabaseService.Connection.InsertAsync(visit);
                }
            }

            File.Move(cardsJsonPath, cardsJsonPath + ".migrated");
        }
    }
}