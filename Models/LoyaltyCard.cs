using SQLite;

namespace munch_stamp.Models;

public class LoyaltyCard
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerContact { get; set; } = string.Empty;

    [Indexed]
    public string QrCodeId { get; set; } = Guid.NewGuid().ToString();

    public int VisitsRequired { get; set; } = 10;
    public string RewardDescription { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // NOT a database column — [Ignore] tells sqlite-net to skip this
    // property entirely. It's populated manually by the service layer
    // after a separate query for this card's visits, the same way you'd
    // manually join two spreadsheets together in code.
    [Ignore]
    public List<Visit> Visits { get; set; } = new();

    public int VisitsCount => Visits.Count;
    public double ProgressPercent => VisitsRequired <= 0 ? 0 : Math.Min(1, (double)VisitsCount / VisitsRequired);
    public string ProgressText => $"{VisitsCount} / {VisitsRequired} visits";
}