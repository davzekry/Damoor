using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class Review : SoftDeletableEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
