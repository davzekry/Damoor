using MediatR;

namespace Damoor.Application.Features.ShoppingSessions.Commands.CreateShoppingSession;

public sealed record CreateShoppingSessionCommand(
    int? UserId) : IRequest<CreateShoppingSessionResult>;
