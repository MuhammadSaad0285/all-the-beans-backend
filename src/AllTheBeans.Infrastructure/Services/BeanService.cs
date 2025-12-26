using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Errors;
using AllTheBeans.Domain.Entities;
using AllTheBeans.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Application.Services;

public class BeanService : IBeanService
{
    private readonly AllTheBeansDbContext _context;

    public BeanService(AllTheBeansDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bean>> SearchBeansAsync(Api.Contracts.Beans.BeanSearchQuery query)
    {
        // Start with all beans
        IQueryable<Bean> beans = _context.Beans;
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            string term = query.Name.ToLower();
            beans = beans.Where(b => EF.Functions.Like(b.Name.ToLower(), $"%{term}%"));
        }
        if (!string.IsNullOrWhiteSpace(query.Colour))
        {
            string term = query.Colour.ToLower();
            beans = beans.Where(b => EF.Functions.Like(b.Colour.ToLower(), $"%{term}%"));
        }
        if (!string.IsNullOrWhiteSpace(query.Country))
        {
            string term = query.Country.ToLower();
            beans = beans.Where(b => EF.Functions.Like(b.Country.ToLower(), $"%{term}%"));
        }
        return await beans.ToListAsync();
    }

    public async Task<Bean> GetBeanAsync(int id)
    {
        var bean = await _context.Beans.FindAsync(id);
        if (bean == null)
            throw new NotFoundException($"Bean with id {id} was not found.");
        return bean;
    }

    public async Task<Bean> CreateBeanAsync(Bean bean)
    {
        _context.Beans.Add(bean);
        await _context.SaveChangesAsync();
        return bean;
    }

    public async Task<Bean> UpdateBeanAsync(int id, Bean updatedBean)
    {
        var bean = await _context.Beans.FindAsync(id);
        if (bean == null)
            throw new NotFoundException($"Bean with id {id} was not found.");

        bean.Name = updatedBean.Name;
        bean.Colour = updatedBean.Colour;
        bean.Country = updatedBean.Country;
        bean.Description = updatedBean.Description;
        bean.ImageUrl = updatedBean.ImageUrl;
        bean.Cost = updatedBean.Cost;
        await _context.SaveChangesAsync();
        return bean;
    }

    public async Task DeleteBeanAsync(int id)
    {
        var bean = await _context.Beans.FindAsync(id);
        if (bean == null)
            throw new NotFoundException($"Bean with id {id} was not found.");
        _context.Beans.Remove(bean);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Handle if bean is referenced by orders (foreign key constraint)
            throw new ValidationException($"Cannot delete Bean with id {id} because it has related orders.");
        }
    }
}
