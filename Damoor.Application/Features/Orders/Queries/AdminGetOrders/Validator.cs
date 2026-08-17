using FluentValidation;

namespace Damoor.Application.Features.Orders.Queries.AdminGetOrders;

public sealed class Validator : AbstractValidator<AdminGetOrdersQuery>
{
    public Validator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => x.Search is not null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);
    }
}
