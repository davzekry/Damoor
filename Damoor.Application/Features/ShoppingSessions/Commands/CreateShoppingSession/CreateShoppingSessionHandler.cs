using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;

namespace Damoor.Application.Features.ShoppingSessions.Commands.CreateShoppingSession;

public sealed class CreateShoppingSessionHandler
    : IRequestHandler<CreateShoppingSessionCommand, CreateShoppingSessionResult>
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(30);

    private readonly DamoorDbContext _db;

    public CreateShoppingSessionHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CreateShoppingSessionResult> Handle(
        CreateShoppingSessionCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var session = new ShoppingSession
        {
            SessionToken = Guid.NewGuid().ToString("N"),
            UserId = request.UserId,
            ExpiresAt = now.Add(SessionLifetime),
            Cart = new Cart()
        };

        _db.ShoppingSessions.Add(session);
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateShoppingSessionResult
        {
            SessionToken = session.SessionToken,
            ExpiresAtUtc = session.ExpiresAt,
            CartId = session.Cart.Id
        };
    }
}
