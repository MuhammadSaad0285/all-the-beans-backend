using AllTheBeans.Domain.ValueObjects;

namespace AllTheBeans.Domain.Entities;

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int BeanId { get; set; }
    public Bean? Bean { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; } = new Money();
}
