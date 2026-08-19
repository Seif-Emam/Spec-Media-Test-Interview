using System.Collections.Concurrent;
using TestIjnterview.Domain.Entities;
using TestIjnterview.Domain.Repositories;

namespace TestIjnterview.Infrastructure.Persistence;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        var result = _products.Values.Where(p => idSet.Contains(p.Id)).ToList();
        return Task.FromResult<IReadOnlyList<Product>>(result);
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Product>>(_products.Values.ToList());
    }

    public Task<bool> ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (_products.TryGetValue(productId, out var product))
        {
            var reserved = product.TryReserveStock(quantity);
            return Task.FromResult(reserved);
        }

        return Task.FromResult(false);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.TryAdd(product.Id, product);
        return Task.CompletedTask;
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Order>>(_orders.Values.ToList());
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.TryAdd(order.Id, order);
        return Task.CompletedTask;
    }
}
