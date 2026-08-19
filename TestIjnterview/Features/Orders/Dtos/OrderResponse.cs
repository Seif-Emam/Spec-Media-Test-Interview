using System.Text.Json.Serialization;
using TestIjnterview.Domain.Entities;

namespace TestIjnterview.Features.Orders.Dtos;

public record OrderItemDto
{
    [JsonPropertyName("productId")]
    public Guid ProductId { get; init; }

    [JsonPropertyName("productName")]
    public string ProductName { get; init; } = string.Empty;

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; init; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; init; }
}

public record OrderResponse
{
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; init; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "USD";

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; init; }

    [JsonPropertyName("items")]
    public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();

    public static OrderResponse FromDomain(Order order)
    {
        return new OrderResponse
        {
            OrderId = order.Id,
            TotalPrice = order.TotalPrice,
            Currency = order.Currency,
            CreatedAtUtc = order.CreatedAtUtc,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
}
