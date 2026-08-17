using FluentValidation;

namespace Damoor.Application.Features.Orders.Queries.GetOrderById;

public sealed class Validator : AbstractValidator<GetOrderByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionToken))
            .WithMessage("Authentication or the X-Shopping-Session header is required.");
    }
}
