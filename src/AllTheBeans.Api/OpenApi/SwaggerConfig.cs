using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AllTheBeans.Api.OpenApi;

public static class SwaggerConfig
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AllTheBeans API",
            Version = "v1",
            Description = "API for managing All The Beans products, Bean of the Day, and Orders."
        });
    }
}
