namespace AllTheBeans.Api.Contracts.Orders;

public class OrderLineResponse
{
    public int BeanId { get; set; }
    public string BeanName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineCost { get; set; }
}
