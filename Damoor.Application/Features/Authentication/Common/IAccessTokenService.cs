using Damoor.Infrastructure.Identity;

namespace Damoor.Application.Features.Authentication.Common;

public interface IAccessTokenService
{
    GeneratedAccessToken Create(
        AppUser user,
        IReadOnlyCollection<string> roles);
}
