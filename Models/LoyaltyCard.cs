namespace munch_stamp.Models;

public class LoyaltyCard
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerContact { get; set; } = string.Empty;

    // The identifier that will go inside the QR code
    // Deliberately separate from Id
    public string QrCodeId { get; set; } = Guid.NewGuid().ToString();

    public int VisitsRequired { get; set; } = 10;
    public string RewardDescription { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Visit> Visits { get; set; } = new();
    
    // No longer settable directly — always reflects the real visit history.
    public int VisitsCount => Visits.Count;
    public string ProgressText => $"{VisitsCount} / {VisitsRequired} visits";    
}