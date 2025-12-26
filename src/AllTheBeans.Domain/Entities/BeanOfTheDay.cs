using System;

namespace AllTheBeans.Domain.Entities;

public class BeanOfTheDay
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int BeanId { get; set; }
    public Bean? Bean { get; set; }
}
