using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.UpdateProduct;

public sealed class Validator : AbstractValidator<UpdateProductCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

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
