namespace munch_stamp.Models;

public class Visit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
}