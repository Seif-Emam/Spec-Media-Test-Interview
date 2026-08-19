using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Domain.Repositories;
using TestIjnterview.Features.Products.Dtos;

namespace TestIjnterview.Features.Products.GetProducts;

public record GetProductsQuery : IRequest<Result<IReadOnlyList<ProductResponse>>>;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<IReadOnlyList<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var dtos = products.Select(ProductResponse.FromDomain).ToList();
        return Result<IReadOnlyList<ProductResponse>>.Success(dtos, "Products retrieved successfully.", 200);
    }
}

public static class GetProductsEndpoint
{
    public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/products", async (ISender mediator) =>
        {
            var result = await mediator.Send(new GetProductsQuery());
            var apiResponse = result.ToApiResponse();
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        })
        .WithName("GetProducts")
        .WithSummary("List all catalog products")
        .WithDescription("Retrieves the full catalog of seeded products including real-time stock levels.")
        .Produces<ApiResponse<IReadOnlyList<ProductResponse>>>(StatusCodes.Status200OK);
    }
}
