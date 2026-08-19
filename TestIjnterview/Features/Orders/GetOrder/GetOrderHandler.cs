using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Domain.Repositories;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.GetOrder;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order == null)
        {
            return Result<OrderResponse>.NotFound($"Order with ID '{request.Id}' was not found.");
        }

        var responseDto = OrderResponse.FromDomain(order);
        return Result<OrderResponse>.Success(responseDto, "Order retrieved successfully.", 200);
    }
}
