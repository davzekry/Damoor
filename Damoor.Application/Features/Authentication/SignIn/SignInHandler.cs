using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Authentication.Common;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Damoor.Application.Features.Authentication.SignIn;

public sealed class SignInHandler
    : IRequestHandler<SignInCommand, AuthResponse>
{
    private const string InvalidCredentialsMessage =
        "Invalid email or password.";

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IAccessTokenService _accessTokenService;

    public SignInHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IAccessTokenService accessTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accessTokenService = accessTokenService;
    }

    public async Task<AuthResponse> Handle(
        SignInCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            throw new UnauthorizedException(InvalidCredentialsMessage);

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
            throw new UnauthorizedException(InvalidCredentialsMessage);

        var roles = (await _userManager.GetRolesAsync(user)).ToArray();
        var token = _accessTokenService.Create(user, roles);

        return new AuthResponse(
            token.Value,
            "Bearer",
            token.ExpiresAtUtc,
            new AuthUserDto(
                user.Id,
                user.FullName,
                user.Email!,
                roles));
    }
}
