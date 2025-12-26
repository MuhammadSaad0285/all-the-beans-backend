using AllTheBeans.Api.Contracts.Beans;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AllTheBeans.IntegrationTests;

public class BeansEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public BeansEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBeans_ReturnsInitialSeedBeans()
    {
        var response = await _client.GetAsync("/beans");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var beans = await response.Content.ReadFromJsonAsync<List<BeanResponse>>();
        Assert.NotNull(beans);
        Assert.True(beans!.Count >= 15);
        Assert.Contains(beans, b => b.Name == "TURNABOUT");
    }

    [Fact]
    public async Task GetBeanById_ReturnsBean()
    {
        var response = await _client.GetAsync("/beans/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var bean = await response.Content.ReadFromJsonAsync<BeanResponse>();
        Assert.NotNull(bean);
        Assert.Equal(1, bean!.Id);
        Assert.False(string.IsNullOrEmpty(bean.Name));
    }

    [Fact]
    public async Task GetBeanById_InvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/beans/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchBeans_ByName_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/beans?Name=RON");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var beans = await response.Content.ReadFromJsonAsync<List<BeanResponse>>();
        Assert.NotNull(beans);
        Assert.Contains(beans!, b => b.Name.Contains("RON"));
    }

    [Fact]
    public async Task CreateUpdateDeleteBean_Workflow()
    {
        var newBean = new BeanCreateRequest
        {
            Name = "Test Bean",
            Colour = "medium roast",
            Country = "Testland",
            Description = "A test bean",
            ImageUrl = "http://example.com/image.jpg",
            Cost = 12.34m,
            Currency = "GBP"
        };
        var createResponse = await _client.PostAsJsonAsync("/beans", newBean);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdBean = await createResponse.Content.ReadFromJsonAsync<BeanResponse>();
        Assert.NotNull(createdBean);
        Assert.Equal("Test Bean", createdBean!.Name);

        int createdId = createdBean.Id;
        var update = new BeanUpdateRequest
        {
            Name = "Test Bean Updated",
            Colour = "medium roast",
            Country = "Testland",
            Description = "Updated description",
            ImageUrl = "http://example.com/image.jpg",
            Cost = 15.00m,
            Currency = "GBP"
        };
        var updateResp = await _client.PutAsJsonAsync($"/beans/{createdId}", update);
        Assert.Equal(HttpStatusCode.NoContent, updateResp.StatusCode);
        var getResp = await _client.GetAsync($"/beans/{createdId}");
        var updatedBean = await getResp.Content.ReadFromJsonAsync<BeanResponse>();
        Assert.NotNull(updatedBean);
        Assert.Equal("Test Bean Updated", updatedBean!.Name);
        Assert.Equal(15.00m, updatedBean.Cost);

        var deleteResp = await _client.DeleteAsync($"/beans/{createdId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
        var getAfterDeleteResp = await _client.GetAsync($"/beans/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDeleteResp.StatusCode);
    }
}
