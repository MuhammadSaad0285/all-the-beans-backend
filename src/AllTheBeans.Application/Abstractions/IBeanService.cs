using AllTheBeans.Domain.Entities;
using AllTheBeans.Api.Contracts.Beans;

namespace AllTheBeans.Application.Abstractions;

public interface IBeanService
{
    Task<List<Bean>> SearchBeansAsync(BeanSearchQuery query);
    Task<Bean> GetBeanAsync(int id);
    Task<Bean> CreateBeanAsync(Bean bean);
    Task<Bean> UpdateBeanAsync(int id, Bean updatedBean);
    Task DeleteBeanAsync(int id);
}
