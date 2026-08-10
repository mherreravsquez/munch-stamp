using SQLite;

namespace munch_stamp.Models;

public class Business
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DefaultVisitsRequired { get; set; } = 10;
    public string DefaultReward { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}