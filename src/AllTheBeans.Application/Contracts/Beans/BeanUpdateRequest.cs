namespace AllTheBeans.Api.Contracts.Beans;

public class BeanUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Currency { get; set; } = string.Empty;
}
