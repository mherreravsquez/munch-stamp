namespace munch_stamp.Models;

///<summary>
/// Represents the one business profile stored on this device.
/// This app is meant to run on a single business owner's phone,
/// so there's only ever one Business record.
/// </summary> 
public class Business
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DefaultVisitsRequired { get; set; } = 10;
    public string DefaultReward { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}