using System.Text.Json.Serialization;

namespace AllTheBeans.Infrastructure.Seeding;

public class BeansJsonRecord
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = string.Empty;
    public int index { get; set; }
    public bool isBOTD { get; set; }
    public string Cost { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string colour { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
