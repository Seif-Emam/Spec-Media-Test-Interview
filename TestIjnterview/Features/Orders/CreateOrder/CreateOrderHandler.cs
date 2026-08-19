using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Domain.Entities;
using TestIjnterview.Domain.Repositories;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        ILogger<CreateOrderHandler> logger)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productMap = products.ToDictionary(p => p.Id);

        // 1. Verify existence of all requested products
        var missingProductIds = productIds.Where(id => !productMap.ContainsKey(id)).ToList();
        if (missingProductIds.Any())
        {
            return Result<OrderResponse>.Failure(
                $"The following product(s) do not exist: {string.Join(", ", missingProductIds)}",
                statusCode: 404);
        }

        // 2. Perform atomic inventory reservations to prevent race conditions
        var reservedItems = new List<(Guid ProductId, int Quantity)>();
        foreach (var item in request.Items)
        {
            var reserved = await _productRepository.ReserveStockAsync(item.ProductId, item.Quantity, cancellationToken);
            if (!reserved)
            {
                // Rollback any items already reserved in this transaction
                foreach (var rollback in reservedItems)
                {
                    if (productMap.TryGetValue(rollback.ProductId, out var productToRelease))
                    {
                        productToRelease.ReleaseStock(rollback.Quantity);
                    }
                }

                var productName = productMap[item.ProductId].Name;
                _logger.LogWarning("Insufficient stock for product {ProductName} ({ProductId})", productName, item.ProductId);
                return Result<OrderResponse>.Conflict($"Insufficient inventory available for product '{productName}'.");
            }

            reservedItems.Add((item.ProductId, item.Quantity));
        }

        // 3. Construct Order domain entity and calculate totals securely
        var order = new Order(Guid.NewGuid(), request.Currency);
        foreach (var item in request.Items)
        {
            var product = productMap[item.ProductId];
            order.AddItem(product.Id, product.Name, product.UnitPrice, item.Quantity);
        }

        // 4. Persist order
        await _orderRepository.AddAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} successfully processed with total {TotalPrice} {Currency}",
            order.Id, order.TotalPrice, order.Currency);

        var responseDto = OrderResponse.FromDomain(order);
        return Result<OrderResponse>.Success(responseDto, "Order processed successfully.", statusCode: 200);
    }
}
