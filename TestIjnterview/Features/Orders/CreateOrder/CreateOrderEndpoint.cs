using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.CreateOrder;

public static class CreateOrderEndpoint
{
    public static void MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/orders", async (CreateOrderCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            var apiResponse = result.ToApiResponse();
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        })
        .WithName("CreateOrder")
        .WithSummary("Process a new order")
        .WithDescription("Creates a new order with server-authoritative pricing and atomic stock reservation.")
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status409Conflict)
        .WithOpenApi();
    }
}
