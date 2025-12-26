using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Abstractions;

public interface IBeanOfTheDayService
{
    Task<BeanOfTheDay> GetBeanOfTheDayAsync();
}
