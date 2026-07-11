using Damoor.Application.Common.Models;
using Damoor.Application.Features.Authentication.Common;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Damoor.Application.Features.Authentication.SignIn;

public sealed class SignInHandler
    : IRequestHandler<SignInCommand, ApiResponse<AuthResponse>>
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

    public async Task<ApiResponse<AuthResponse>> Handle(
        SignInCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return ApiResponse<AuthResponse>.Fail(InvalidCredentialsMessage);

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
            return ApiResponse<AuthResponse>.Fail(InvalidCredentialsMessage);

        var roles = (await _userManager.GetRolesAsync(user)).ToArray();
        var token = _accessTokenService.Create(user, roles);

        return ApiResponse<AuthResponse>.Ok(
            new AuthResponse(
                token.Value,
                "Bearer",
                token.ExpiresAtUtc,
                new AuthUserDto(
                    user.Id,
                    user.FullName,
                    user.Email!,
                    roles)),
            "Signed in successfully.");
    }
}
