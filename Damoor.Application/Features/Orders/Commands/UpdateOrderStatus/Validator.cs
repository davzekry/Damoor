using FluentValidation;

namespace Damoor.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed class Validator : AbstractValidator<UpdateOrderStatusCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Status).IsInEnum();
    }
}
