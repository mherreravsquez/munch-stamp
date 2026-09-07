using SQLite;

namespace munch_stamp.Models;

public class Business
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string CardBackgroundColor { get; set; } = "#1a1a2e";
    public string CardGradient1Color { get; set; } = "#6c5ce7";
    public string CardGradient2Color { get; set; } = "#fd79a8";
}