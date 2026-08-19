using System.Text.Json.Serialization;
using TestIjnterview.Domain.Entities;

namespace TestIjnterview.Features.Products.Dtos;

public record ProductResponse
{
    [JsonPropertyName("productId")]
    public Guid ProductId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "USD";

    [JsonPropertyName("availableStock")]
    public int AvailableStock { get; init; }

    public static ProductResponse FromDomain(Product product)
    {
        return new ProductResponse
        {
            ProductId = product.Id,
            Name = product.Name,
            UnitPrice = product.UnitPrice,
            Currency = product.Currency,
            AvailableStock = product.AvailableStock
        };
    }
}
