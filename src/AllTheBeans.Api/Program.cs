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

// Configure services
builder.Services.AddDbContext<AllTheBeansDbContext>(options =>
{
    // Use SQLite database file (in current directory)
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
// Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => SwaggerConfig.Configure(c));

var app = builder.Build();

// Apply migrations and seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AllTheBeansDbContext>();

    // Create schema (you said: no migrations)
    db.Database.EnsureCreated();

    // Seed beans using an absolute path (works under WebApplicationFactory)
    var seedPath = ResolveBeansJsonPath(app);
    AllTheBeans.Infrastructure.Seeding.BeansJsonSeeder.SeedFromFile(db, seedPath);
}

static string ResolveBeansJsonPath(WebApplication app)
{
    // 1) If beans.json is next to the running binaries
    var baseDir = AppContext.BaseDirectory;
    var p1 = Path.Combine(baseDir, "beans.json");
    if (File.Exists(p1)) return p1;

    // 2) If beans.json is in API content root (src/AllTheBeans.Api)
    var p2 = Path.Combine(app.Environment.ContentRootPath, "beans.json");
    if (File.Exists(p2)) return p2;

    // 3) If beans.json is kept in Infrastructure/Seeding (your current setup)
    // Find solution root by walking up until we find AllTheBeans.sln
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


// Configure middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "AllTheBeans API Docs";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AllTheBeans API v1");
    });
}

// Map endpoints groups for Beans, BeanOfTheDay, and Orders
app.MapGroup("/beans").MapBeansEndpoints();
app.MapGroup("/bean-of-the-day").MapBeanOfTheDayEndpoints();
app.MapGroup("/orders").MapOrdersEndpoints();

// Run the web application
app.Run();

// Partial Program class for integration testing
public partial class Program { }
