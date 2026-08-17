using FluentValidation;

namespace Damoor.Application.Features.Orders.Queries.GetOrders;

public sealed class Validator : AbstractValidator<GetOrdersQuery>
{
    public Validator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionToken))
            .WithMessage("Authentication or the X-Shopping-Session header is required.");
    }
}
