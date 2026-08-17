namespace Damoor.Application.Features.ShoppingSessions.Commands.CreateShoppingSession;

public sealed class CreateShoppingSessionResult
{
    public string SessionToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public int CartId { get; set; }
}
