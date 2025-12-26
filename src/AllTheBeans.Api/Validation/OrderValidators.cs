using AllTheBeans.Api.Contracts.Orders;
using AllTheBeans.Application.Errors;

namespace AllTheBeans.Api.Validation;

public static class OrderValidators
{
    public static void ValidateCreateOrder(CreateOrderRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new ValidationException("Order must contain at least one item.");
        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                throw new ValidationException($"Quantity for Bean {item.BeanId} must be at least 1.");
        }
    }
}
