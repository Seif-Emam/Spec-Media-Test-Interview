using TestIjnterview.Domain.Entities;
using TestIjnterview.Domain.Repositories;

namespace TestIjnterview.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedInitialDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        // 1. Seed Products Catalog
        var headphones = new Product(
            id: Guid.Parse("7ca85f64-5717-4562-b3fc-2c963f66afa1"),
            name: "Wireless Noise-Cancelling Headphones",
            unitPrice: 49.99m,
            initialStock: 50,
            currency: "USD"
        );

        var keyboard = new Product(
            id: Guid.Parse("8da85f64-5717-4562-b3fc-2c963f66afa2"),
            name: "RGB Mechanical Gaming Keyboard",
            unitPrice: 89.99m,
            initialStock: 30,
            currency: "USD"
        );

        var monitor = new Product(
            id: Guid.Parse("9ea85f64-5717-4562-b3fc-2c963f66afa3"),
            name: "Ultra-Wide 34-Inch Curved Gaming Monitor",
            unitPrice: 499.99m,
            initialStock: 15,
            currency: "USD"
        );

        var chair = new Product(
            id: Guid.Parse("afa85f64-5717-4562-b3fc-2c963f66afa4"),
            name: "Ergonomic Mesh Office Chair",
            unitPrice: 229.50m,
            initialStock: 20,
            currency: "USD"
        );

        var webcam = new Product(
            id: Guid.Parse("bfa85f64-5717-4562-b3fc-2c963f66afa5"),
            name: "4K Ultra HD Streaming Webcam",
            unitPrice: 79.99m,
            initialStock: 40,
            currency: "USD"
        );

        var usbHub = new Product(
            id: Guid.Parse("cfa85f64-5717-4562-b3fc-2c963f66afa6"),
            name: "USB-C Multi-Port Hub (10-in-1)",
            unitPrice: 39.99m,
            initialStock: 100,
            currency: "USD"
        );

        await productRepo.AddAsync(headphones);
        await productRepo.AddAsync(keyboard);
        await productRepo.AddAsync(monitor);
        await productRepo.AddAsync(chair);
        await productRepo.AddAsync(webcam);
        await productRepo.AddAsync(usbHub);

        // 2. Seed Sample Orders with specific IDs for test API

        // Order 1: Audio Set (3x Headphones = $149.97)
        var order1 = new Order(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "USD");
        order1.AddItem(headphones.Id, headphones.Name, headphones.UnitPrice, 3);
        await orderRepo.AddAsync(order1);

        // Order 2: Developer Workstation Bundle (1x Curved Monitor + 1x Mechanical Keyboard + 1x USB Hub = $629.97)
        var order2 = new Order(Guid.Parse("4fa85f64-5717-4562-b3fc-2c963f66afa7"), "USD");
        order2.AddItem(monitor.Id, monitor.Name, monitor.UnitPrice, 1);
        order2.AddItem(keyboard.Id, keyboard.Name, keyboard.UnitPrice, 1);
        order2.AddItem(usbHub.Id, usbHub.Name, usbHub.UnitPrice, 1);
        await orderRepo.AddAsync(order2);

        // Order 3: Ergonomic Remote Setup (1x Ergonomic Chair + 1x 4K Webcam = $309.49)
        var order3 = new Order(Guid.Parse("5fa85f64-5717-4562-b3fc-2c963f66afa8"), "USD");
        order3.AddItem(chair.Id, chair.Name, chair.UnitPrice, 1);
        order3.AddItem(webcam.Id, webcam.Name, webcam.UnitPrice, 1);
        await orderRepo.AddAsync(order3);

        // Order 4: Accessories Pack (2x USB Hubs + 2x Headphones = $179.96)
        var order4 = new Order(Guid.Parse("6fa85f64-5717-4562-b3fc-2c963f66afa9"), "USD");
        order4.AddItem(usbHub.Id, usbHub.Name, usbHub.UnitPrice, 2);
        order4.AddItem(headphones.Id, headphones.Name, headphones.UnitPrice, 2);
        await orderRepo.AddAsync(order4);
    }
}
