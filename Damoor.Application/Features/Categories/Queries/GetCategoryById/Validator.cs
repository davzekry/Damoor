using FluentValidation;

namespace Damoor.Application.Features.Categories.Queries.GetCategoryById;

public sealed class Validator : AbstractValidator<GetCategoryByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
