using AllTheBeans.Domain.ValueObjects;

namespace AllTheBeans.Domain.Entities;

public class Bean
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Money Cost { get; set; } = new Money();
}
