using AllTheBeans.Infrastructure.Persistence;
using AllTheBeans.Infrastructure.Time;
using AllTheBeans.Infrastructure.Idempotency;
using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Services;
using AllTheBeans.Api.Endpoints;
using AllTheBeans.Api.Middleware;
using AllTheBeans.Api.OpenApi;
using AllTheBeans.Api.Validation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AllTheBeansDbContext>(options =>
{
    string connString = builder.Configuration.GetConnectionString("Default")
                        ?? "Data Source=AllTheBeans.db";
    options.UseSqlite(connString);
});
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IBeanService, BeanService>();
builder.Services.AddScoped<IBeanOfTheDayService, BeanOfTheDayService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IIdempotencyStore, EfIdempotencyStore>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => SwaggerConfig.Configure(c));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AllTheBeansDbContext>();
    db.Database.EnsureCreated();

    var seedPath = ResolveBeansJsonPath(app);
    AllTheBeans.Infrastructure.Seeding.BeansJsonSeeder.SeedFromFile(db, seedPath);
}

static string ResolveBeansJsonPath(WebApplication app)
{
    var baseDir = AppContext.BaseDirectory;
    var p1 = Path.Combine(baseDir, "beans.json");
    if (File.Exists(p1)) return p1;

    var p2 = Path.Combine(app.Environment.ContentRootPath, "beans.json");
    if (File.Exists(p2)) return p2;

    var dir = new DirectoryInfo(app.Environment.ContentRootPath);
    while (dir != null && !dir.GetFiles("AllTheBeans.sln").Any())
        dir = dir.Parent;

    if (dir != null)
    {
        var p3 = Path.Combine(dir.FullName, "src", "AllTheBeans.Infrastructure", "Seeding", "beans.json");
        if (File.Exists(p3)) return p3;

        var p4 = Path.Combine(dir.FullName, "src", "AllTheBeans.Api", "beans.json");
        if (File.Exists(p4)) return p4;
    }

    throw new FileNotFoundException(
        "beans.json not found for seeding. Place it in AllTheBeans.Api, copy to output, or keep it under Infrastructure/Seeding.",
        "beans.json");
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AngularDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "AllTheBeans API Docs";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AllTheBeans API v1");
    });
}

app.MapGroup("/beans").MapBeansEndpoints();
app.MapGroup("/bean-of-the-day").MapBeanOfTheDayEndpoints();
app.MapGroup("/orders").MapOrdersEndpoints();

app.Run();

public partial class Program { }
