using FluentValidation;

namespace Damoor.Application.Features.Categories.Commands.DeleteCategory;

public sealed class Validator : AbstractValidator<DeleteCategoryCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
