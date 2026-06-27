namespace Damoor.Application.Features.Authentication.Common;

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    AuthUserDto User);

public sealed record AuthUserDto(
    int Id,
    string FullName,
    string Email,
    IReadOnlyCollection<string> Roles);

public sealed record GeneratedAccessToken(
    string Value,
    DateTime ExpiresAtUtc);
