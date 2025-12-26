namespace AllTheBeans.Api.Contracts.Orders;

public class OrderResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<OrderLineResponse> Items { get; set; } = new List<OrderLineResponse>();
}
