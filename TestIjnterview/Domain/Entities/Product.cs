namespace TestIjnterview.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = "USD";
    public int AvailableStock { get; private set; }

    private readonly object _stockLock = new();

    private Product() { }

    public Product(Guid id, string name, decimal unitPrice, int initialStock, string currency = "USD")
    {
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        if (initialStock < 0)
            throw new ArgumentException("Initial stock cannot be negative.", nameof(initialStock));

        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UnitPrice = unitPrice;
        AvailableStock = initialStock;
        Currency = currency;
    }

    /// <summary>
    /// Thread-safe atomic inventory reservation to prevent race conditions.
    /// </summary>
    public bool TryReserveStock(int quantity)
    {
        if (quantity <= 0) return false;

        lock (_stockLock)
        {
            if (AvailableStock < quantity)
            {
                return false;
            }

            AvailableStock -= quantity;
            return true;
        }
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0) return;

        lock (_stockLock)
        {
            AvailableStock += quantity;
        }
    }
}
