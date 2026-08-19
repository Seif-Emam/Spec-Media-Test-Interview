using Microsoft.EntityFrameworkCore;
using TestIjnterview.Domain.Entities;
using TestIjnterview.Domain.Repositories;

namespace TestIjnterview.Infrastructure.Persistence;

public class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public EfProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await _context.Products
            .Where(p => idList.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Atomic database-level stock reservation preventing race conditions across distributed instances.
    /// </summary>
    public async Task<bool> ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0) return false;

        var rowsAffected = await _context.Products
            .Where(p => p.Id == productId && p.AvailableStock >= quantity)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.AvailableStock, p => p.AvailableStock - quantity), cancellationToken);

        return rowsAffected > 0;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public EfOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
