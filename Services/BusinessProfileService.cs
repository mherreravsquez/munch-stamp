using munch_stamp.Models;

namespace munch_stamp.Services;

public class BusinessProfileService
{
    public async Task SaveAsync(Business business)
    {
        // InsertOrReplaceAsync: updates the row if this Id already
        // exists, inserts a new one otherwise. Since this app only
        // ever has one Business row, this always "updates" after the
        // first save.
        await DatabaseService.Connection.InsertOrReplaceAsync(business);
    }

    public async Task<Business?> LoadAsync()
    {
        return await DatabaseService.Connection.Table<Business>().FirstOrDefaultAsync();
    }
}