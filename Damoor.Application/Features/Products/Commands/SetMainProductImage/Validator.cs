using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.SetMainProductImage;

public sealed class Validator : AbstractValidator<SetMainProductImageCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
