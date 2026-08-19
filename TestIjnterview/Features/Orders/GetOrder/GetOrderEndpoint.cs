using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.GetOrder;

public static class GetOrderEndpoint
{
    public static void MapGetOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/orders/{id:guid}", async (Guid id, ISender mediator) =>
        {
            var query = new GetOrderByIdQuery(id);
            var result = await mediator.Send(query);
            var apiResponse = result.ToApiResponse();
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        })
        .WithName("GetOrderById")
        .WithSummary("Get order details by ID")
        .WithDescription("Retrieves an existing order by its unique identifier.")
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}
