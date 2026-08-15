using FluentValidation;

namespace Damoor.Application.Features.Categories.Commands.CreateCategory;

public sealed class Validator : AbstractValidator<CreateCategoryCommand>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);
    }
}
