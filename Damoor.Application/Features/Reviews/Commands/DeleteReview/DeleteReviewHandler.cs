using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Reviews.Commands.DeleteReview;

public sealed class DeleteReviewHandler
    : IRequestHandler<DeleteReviewCommand>
{
    private readonly DamoorDbContext _db;

    public DeleteReviewHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (review is null)
            throw new NotFoundException("Review", request.Id);

        if (!request.IsAdmin && review.UserId != request.UserId)
            throw new UnauthorizedException("You are not authorized to delete this review.");

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
