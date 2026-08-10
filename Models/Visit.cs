using SQLite;

namespace munch_stamp.Models;

public class Visit
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Indexed]
    public string LoyaltyCardId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
}