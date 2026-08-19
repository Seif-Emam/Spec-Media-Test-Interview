using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Domain.Repositories;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.GetOrders;

public record GetOrdersQuery : IRequest<Result<IReadOnlyList<OrderResponse>>>;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<IReadOnlyList<OrderResponse>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        var dtos = orders.Select(OrderResponse.FromDomain).ToList();
        return Result<IReadOnlyList<OrderResponse>>.Success(dtos, "Orders retrieved successfully.", 200);
    }
}

public static class GetOrdersEndpoint
{
    public static void MapGetOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/orders", async (ISender mediator) =>
        {
            var result = await mediator.Send(new GetOrdersQuery());
            var apiResponse = result.ToApiResponse();
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        })
        .WithName("GetAllOrders")
        .WithSummary("List all orders")
        .WithDescription("Retrieves all placed and seeded orders with their IDs, items, and calculated totals.")
        .Produces<ApiResponse<IReadOnlyList<OrderResponse>>>(StatusCodes.Status200OK);
    }
}
