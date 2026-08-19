using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TestIjnterview.Domain.Entities;
using TestIjnterview.Domain.Repositories;
using TestIjnterview.Features.Orders.CreateOrder;
using TestIjnterview.Features.Orders.GetOrder;
using TestIjnterview.Features.Orders.GetOrders;
using TestIjnterview.Features.Products.GetProducts;
using TestIjnterview.Infrastructure.Persistence;
using Xunit;

namespace TestIjnterview.Tests;

public class OrderProcessingTests
{
    [Fact]
    public void Order_ShouldCalculateTotalPrice_CorrectlyFromItems()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), "USD");
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(productId, "Wireless Noise-Cancelling Headphones", 49.99m, 3);

        // Assert
        Assert.Equal(149.97m, order.TotalPrice);
        Assert.Single(order.Items);
        Assert.Equal(149.97m, order.Items.First().LineTotal);
    }

    [Fact]
    public void Product_TryReserveStock_ShouldPreventOverselling()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Gaming Mouse", 29.99m, initialStock: 5);

        // Act
        var firstReservation = product.TryReserveStock(3);
        var secondReservation = product.TryReserveStock(3); // Should fail (only 2 left)
        var thirdReservation = product.TryReserveStock(2);  // Should succeed

        // Assert
        Assert.True(firstReservation);
        Assert.False(secondReservation);
        Assert.True(thirdReservation);
        Assert.Equal(0, product.AvailableStock);
    }

    [Fact]
    public async Task CreateOrderValidator_ShouldFail_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var validator = new CreateOrderValidator();
        var command = new CreateOrderCommand(new List<CreateOrderItemRequest>
        {
            new(Guid.NewGuid(), 0)
        });

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public async Task CreateOrderHandler_ShouldCreateOrder_WithCalculatedPricesAndEnvelope()
    {
        // Arrange
        var productRepo = new InMemoryProductRepository();
        var orderRepo = new InMemoryOrderRepository();
        var logger = NullLogger<CreateOrderHandler>.Instance;
        var handler = new CreateOrderHandler(productRepo, orderRepo, logger);

        var sampleProductId = Guid.Parse("7ca85f64-5717-4562-b3fc-2c963f66afa1");
        var sampleProduct = new Product(sampleProductId, "Wireless Noise-Cancelling Headphones", 49.99m, 50, "USD");
        await productRepo.AddAsync(sampleProduct);

        var command = new CreateOrderCommand(new List<CreateOrderItemRequest>
        {
            new(sampleProductId, 3)
        });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(149.97m, result.Value.TotalPrice);
        Assert.Equal("USD", result.Value.Currency);
        Assert.Single(result.Value.Items);
        Assert.Equal(49.99m, result.Value.Items[0].UnitPrice);
        Assert.Equal(3, result.Value.Items[0].Quantity);
        Assert.Equal(149.97m, result.Value.Items[0].LineTotal);
    }

    [Fact]
    public async Task DataSeeder_ShouldSeedProductsAndSampleOrders()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        var provider = services.BuildServiceProvider();

        // Act
        await DataSeeder.SeedInitialDataAsync(provider);

        // Assert Products
        var productRepo = provider.GetRequiredService<IProductRepository>();
        var products = await productRepo.GetAllAsync();
        Assert.True(products.Count >= 6);

        var headphones = await productRepo.GetByIdAsync(Guid.Parse("7ca85f64-5717-4562-b3fc-2c963f66afa1"));
        Assert.NotNull(headphones);
        Assert.Equal("Wireless Noise-Cancelling Headphones", headphones.Name);
        Assert.Equal(49.99m, headphones.UnitPrice);

        // Assert Orders
        var orderRepo = provider.GetRequiredService<IOrderRepository>();
        var orders = await orderRepo.GetAllAsync();
        Assert.Equal(4, orders.Count);

        var sampleOrder = await orderRepo.GetByIdAsync(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"));
        Assert.NotNull(sampleOrder);
        Assert.Equal(149.97m, sampleOrder.TotalPrice);
        Assert.Single(sampleOrder.Items);
    }

    [Fact]
    public async Task GetProductsHandler_ShouldReturnSeededCatalog()
    {
        // Arrange
        var productRepo = new InMemoryProductRepository();
        var product = new Product(Guid.NewGuid(), "Mechanical Keyboard", 89.99m, 20);
        await productRepo.AddAsync(product);

        var handler = new GetProductsHandler(productRepo);

        // Act
        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal("Mechanical Keyboard", result.Value[0].Name);
    }

    [Fact]
    public async Task GetOrdersHandler_ShouldReturnAllOrders()
    {
        // Arrange
        var orderRepo = new InMemoryOrderRepository();
        var order = new Order(Guid.NewGuid(), "USD");
        order.AddItem(Guid.NewGuid(), "Sample Item", 50m, 1);
        await orderRepo.AddAsync(order);

        var handler = new GetOrdersHandler(orderRepo);

        // Act
        var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(50m, result.Value[0].TotalPrice);
    }
}
