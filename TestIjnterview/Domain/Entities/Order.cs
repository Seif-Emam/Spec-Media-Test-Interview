namespace TestIjnterview.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalPrice => _items.Sum(i => i.LineTotal);
    public string Currency { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Order() 
    {
        Currency = "USD";
    }

    public Order(Guid id, string currency = "USD")
    {
        Id = id;
        Currency = currency;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            var updatedQuantity = existing.Quantity + quantity;
            _items.Remove(existing);
            _items.Add(new OrderItem(productId, productName, unitPrice, updatedQuantity));
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
        }
    }
}
