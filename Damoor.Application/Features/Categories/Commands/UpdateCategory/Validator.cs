using FluentValidation;

namespace Damoor.Application.Features.Categories.Commands.UpdateCategory;

public sealed class Validator : AbstractValidator<UpdateCategoryCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);
    }
}
