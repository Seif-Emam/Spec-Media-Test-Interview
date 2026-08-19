using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.CreateOrder;

public record CreateOrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderCommand(List<CreateOrderItemRequest> Items, string Currency = "USD") 
    : IRequest<Result<OrderResponse>>;
