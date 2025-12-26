using AllTheBeans.Api.Contracts.BeanOfTheDay;
using AllTheBeans.Api.Contracts.Beans;
using AllTheBeans.Api.Contracts.Orders;
using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Mapping;

public static class DtoMapping
{
    public static BeanResponse ToBeanResponse(Bean bean)
    {
        return new BeanResponse
        {
            Id = bean.Id,
            Name = bean.Name,
            Colour = bean.Colour,
            Country = bean.Country,
            Description = bean.Description,
            ImageUrl = bean.ImageUrl,
            Cost = bean.Cost.Amount,
            Currency = bean.Cost.Currency
        };
    }

    public static BeanOfTheDayResponse ToBeanOfTheDayResponse(BeanOfTheDay botd)
    {
        return new BeanOfTheDayResponse
        {
            //Date = botd.Date,
            Date = botd.Date,
            Id = botd.BeanId,
            Name = botd.Bean?.Name ?? string.Empty,
            Colour = botd.Bean?.Colour ?? string.Empty,
            Country = botd.Bean?.Country ?? string.Empty,
            Description = botd.Bean?.Description ?? string.Empty,
            ImageUrl = botd.Bean?.ImageUrl ?? string.Empty,
            Cost = botd.Bean?.Cost.Amount ?? 0,
            Currency = botd.Bean?.Cost.Currency ?? string.Empty
        };
    }

    public static OrderResponse ToOrderResponse(Order order)
    {
        var response = new OrderResponse
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt
        };
        // All lines should share same currency if order is consistent
        string currency = order.Lines.FirstOrDefault()?.UnitPrice.Currency ?? string.Empty;
        decimal totalCost = 0;
        foreach (var line in order.Lines)
        {
            totalCost += line.UnitPrice.Amount * line.Quantity;
            response.Items.Add(new OrderLineResponse
            {
                BeanId = line.BeanId,
                BeanName = line.Bean?.Name ?? string.Empty,
                Quantity = line.Quantity,
                UnitCost = line.UnitPrice.Amount,
                LineCost = line.UnitPrice.Amount * line.Quantity
            });
        }
        response.TotalCost = totalCost;
        response.Currency = currency;
        return response;
    }

    public static Bean ToBean(BeanCreateRequest request)
    {
        return new Bean
        {
            Name = request.Name,
            Colour = request.Colour,
            Country = request.Country,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Cost = new Domain.ValueObjects.Money(request.Cost, request.Currency)
        };
    }

    public static Bean ToBean(BeanUpdateRequest request)
    {
        return new Bean
        {
            Name = request.Name,
            Colour = request.Colour,
            Country = request.Country,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Cost = new Domain.ValueObjects.Money(request.Cost, request.Currency)
        };
    }
}
