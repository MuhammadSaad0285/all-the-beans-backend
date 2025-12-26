using AllTheBeans.Api.Contracts.BeanOfTheDay;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AllTheBeans.IntegrationTests;

public class BeanOfTheDayEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public BeanOfTheDayEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBeanOfTheDay_ReturnsBeanAndConsistentAcrossCalls()
    {
        var response1 = await _client.GetAsync("/bean-of-the-day");
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        var botd1 = await response1.Content.ReadFromJsonAsync<BeanOfTheDayResponse>();
        Assert.NotNull(botd1);
        var response2 = await _client.GetAsync("/bean-of-the-day");
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        var botd2 = await response2.Content.ReadFromJsonAsync<BeanOfTheDayResponse>();
        Assert.NotNull(botd2);
        Assert.Equal(botd1!.Id, botd2!.Id);
        Assert.Equal(botd1.Name, botd2.Name);
    }
}
