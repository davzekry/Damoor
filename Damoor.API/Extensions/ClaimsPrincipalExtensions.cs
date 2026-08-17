using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Damoor.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
