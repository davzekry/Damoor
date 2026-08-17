using FluentValidation;

namespace Damoor.Application.Features.Orders.Queries.AdminGetOrderById;

public sealed class Validator : AbstractValidator<AdminGetOrderByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
