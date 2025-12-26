using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Mapping;
using AllTheBeans.Api.Contracts.Orders;
using Microsoft.AspNetCore.Mvc;
using AllTheBeans.Api.Validation;

namespace AllTheBeans.Api.Endpoints;

public static class OrdersEndpoints
{
    public static RouteGroupBuilder MapOrdersEndpoints(this RouteGroupBuilder group)
    {
        group.WithTags("Orders");

        // POST /orders
        group.MapPost("/", async ([FromBody] CreateOrderRequest request,
                                  [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
                                  IOrderService service) =>
        {
            OrderValidators.ValidateCreateOrder(request);
            var items = request.Items.Select(dto => (dto.BeanId, dto.Quantity)).ToList();
            var order = await service.CreateOrderAsync(items, idempotencyKey);
            var response = DtoMapping.ToOrderResponse(order);
            return Results.Created($"/orders/{response.Id}", response);
        })
        .Produces<OrderResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        // GET /orders/{id}
        group.MapGet("/{id:int}", async (int id, IOrderService service) =>
        {
            var order = await service.GetOrderAsync(id);
            var response = DtoMapping.ToOrderResponse(order);
            return Results.Ok(response);
        })
        .Produces<OrderResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
