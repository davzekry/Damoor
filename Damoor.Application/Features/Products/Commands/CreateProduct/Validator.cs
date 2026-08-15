using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.CreateProduct;

public sealed class Validator : AbstractValidator<CreateProductCommand>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0);
    }
}
