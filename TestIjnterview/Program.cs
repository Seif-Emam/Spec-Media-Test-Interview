using FluentValidation;
using MediatR;
using TestIjnterview.Common.Behaviors;
using TestIjnterview.Common.Middleware;
using TestIjnterview.Domain.Repositories;
using TestIjnterview.Features.Orders.CreateOrder;
using TestIjnterview.Features.Orders.GetOrder;
using TestIjnterview.Features.Orders.GetOrders;
using TestIjnterview.Features.Products.GetProducts;
using TestIjnterview.Infrastructure.Persistence;

namespace TestIjnterview;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Core Services & Exception Handling
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // 2. MediatR Pipeline & CQRS Handlers
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 3. FluentValidation Validators
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // 4. Persistence / Repositories
        builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // 5. OpenAPI / Swagger Documentation
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Enterprise Order & Product Processing API",
                Version = "v1",
                Description = "High-performance CQRS API with MediatR, Vertical Slice Architecture, and Seeded Catalog"
            });
        });

        var app = builder.Build();

        // 6. Seed Initial Data (Catalog & Sample Orders)
        await DataSeeder.SeedInitialDataAsync(app.Services);

        // 7. Global Exception Middleware
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order & Product API v1");
            });
        }

        app.UseHttpsRedirection();

        // 8. Map Vertical Slice Endpoints
        app.MapGetProductsEndpoint();
        app.MapGetOrdersEndpoint();
        app.MapGetOrderEndpoint();
        app.MapCreateOrderEndpoint();

        await app.RunAsync();
    }
}
