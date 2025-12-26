namespace AllTheBeans.Api.Contracts.Orders;

public class CreateOrderRequest
{
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

    public class OrderItemDto
    {
        public int BeanId { get; set; }
        public int Quantity { get; set; }
    }
}
