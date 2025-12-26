using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Mapping;
using AllTheBeans.Api.Contracts.BeanOfTheDay;

namespace AllTheBeans.Api.Endpoints;

public static class BeanOfTheDayEndpoints
{
    public static RouteGroupBuilder MapBeanOfTheDayEndpoints(this RouteGroupBuilder group)
    {
        group.WithTags("BeanOfTheDay");

        // GET /bean-of-the-day
        group.MapGet("/", async (IBeanOfTheDayService service) =>
        {
            var botd = await service.GetBeanOfTheDayAsync();
            var response = DtoMapping.ToBeanOfTheDayResponse(botd);
            return Results.Ok(response);
        })
        .Produces<BeanOfTheDayResponse>(StatusCodes.Status200OK)
        .WithName("GetBeanOfTheDay");

        return group;
    }
}
