using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Mapping;
using AllTheBeans.Api.Contracts.Beans;
using Microsoft.AspNetCore.Mvc;
using AllTheBeans.Api.Validation;

namespace AllTheBeans.Api.Endpoints;

public static class BeansEndpoints
{
    public static RouteGroupBuilder MapBeansEndpoints(this RouteGroupBuilder group)
    {
        group.WithTags("Beans");

        // GET /beans (with optional query parameters)
        group.MapGet("/", async ([AsParameters] BeanSearchQuery query, IBeanService service) =>
        {
            var beans = await service.SearchBeansAsync(query);
            var responseList = beans.Select(DtoMapping.ToBeanResponse).ToList();
            return Results.Ok(responseList);
        })
        .Produces<IEnumerable<BeanResponse>>(StatusCodes.Status200OK);

        // GET /beans/{id}
        group.MapGet("/{id:int}", async (int id, IBeanService service) =>
        {
            var bean = await service.GetBeanAsync(id);
            var response = DtoMapping.ToBeanResponse(bean);
            return Results.Ok(response);
        })
        .Produces<BeanResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /beans
        group.MapPost("/", async ([FromBody] BeanCreateRequest request, IBeanService service) =>
        {
            BeanValidators.ValidateForCreate(request);
            var domainBean = DtoMapping.ToBean(request);
            var createdBean = await service.CreateBeanAsync(domainBean);
            var response = DtoMapping.ToBeanResponse(createdBean);
            return Results.Created($"/beans/{response.Id}", response);
        })
        .Produces<BeanResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /beans/{id}
        group.MapPut("/{id:int}", async (int id, BeanUpdateRequest request, IBeanService service) =>
        {
            BeanValidators.ValidateForUpdate(request);
            var updatedData = DtoMapping.ToBean(request);
            await service.UpdateBeanAsync(id, updatedData);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        // DELETE /beans/{id}
        group.MapDelete("/{id:int}", async (int id, IBeanService service) =>
        {
            await service.DeleteBeanAsync(id);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
