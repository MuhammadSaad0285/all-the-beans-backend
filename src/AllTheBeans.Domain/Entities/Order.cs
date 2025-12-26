using System;
using System.Collections.Generic;

namespace AllTheBeans.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
}
