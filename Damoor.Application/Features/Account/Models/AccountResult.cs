namespace Damoor.Application.Features.Account.Models;

public sealed class AccountResult
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Roles { get; set; } = [];
}
