using FluentValidation;
using MediatR;
using TestIjnterview.Common.Models;
using TestIjnterview.Features.Orders.Dtos;

namespace TestIjnterview.Features.Orders.GetOrder;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderResponse>>;

public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required.");
    }
}
