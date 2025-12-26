namespace AllTheBeans.Api.Contracts.Beans;

public class BeanResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Currency { get; set; } = string.Empty;
}
