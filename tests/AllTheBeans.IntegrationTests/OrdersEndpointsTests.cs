using AllTheBeans.Api.Contracts.Orders;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AllTheBeans.IntegrationTests;

public class OrdersEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public OrdersEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrder()
    {
        // Get real seeded beans so we use valid IDs
        var beansResp = await _client.GetAsync("/beans");
        beansResp.EnsureSuccessStatusCode();

        var beans = await beansResp.Content.ReadFromJsonAsync<List<AllTheBeans.Api.Contracts.Beans.BeanResponse>>();
        Assert.NotNull(beans);
        Assert.True(beans!.Count >= 2);

        var orderRequest = new CreateOrderRequest
        {
            Items = new List<CreateOrderRequest.OrderItemDto>
        {
            new() { BeanId = beans[0].Id, Quantity = 2 },
            new() { BeanId = beans[1].Id, Quantity = 1 }
        }
        };

        var response = await _client.PostAsJsonAsync("/orders", orderRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);

        decimal sumLineCost = 0;
        foreach (var line in order!.Items)
        {
            Assert.True(line.LineCost >= line.UnitCost * line.Quantity);
            sumLineCost += line.LineCost;
        }

        Assert.Equal(sumLineCost, order.TotalCost);

        var getResp = await _client.GetAsync($"/orders/{order.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        var fetchedOrder = await getResp.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(fetchedOrder);
        Assert.Equal(order.Id, fetchedOrder!.Id);
        Assert.Equal(order.TotalCost, fetchedOrder.TotalCost);
    }

    [Fact]
    public async Task CreateOrder_WithDuplicateIdempotencyKey_IsIdempotent()
    {
        // Get one real bean id
        var beansResp = await _client.GetAsync("/beans");
        beansResp.EnsureSuccessStatusCode();

        var beans = await beansResp.Content.ReadFromJsonAsync<List<AllTheBeans.Api.Contracts.Beans.BeanResponse>>();
        Assert.NotNull(beans);
        Assert.True(beans!.Count >= 1);

        var orderRequest = new CreateOrderRequest
        {
            Items = new List<CreateOrderRequest.OrderItemDto>
        {
            new() { BeanId = beans[0].Id, Quantity = 1 }
        }
        };

        string key = "test-key-123";

        HttpRequestMessage Build()
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, "/orders")
            {
                Content = JsonContent.Create(orderRequest)
            };
            msg.Headers.Add("Idempotency-Key", key);
            return msg;
        }

        var firstResp = await _client.SendAsync(Build());
        Assert.Equal(HttpStatusCode.Created, firstResp.StatusCode);

        var secondResp = await _client.SendAsync(Build());
        Assert.Equal(HttpStatusCode.Conflict, secondResp.StatusCode);
    }

}
