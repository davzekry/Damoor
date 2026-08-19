using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Damoor.Application.Features.Authentication.ChangePassword;

public sealed class ChangePasswordHandler
    : IRequestHandler<ChangePasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public ChangePasswordHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            throw new NotFoundException("User", request.UserId);

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (result.Succeeded)
            return;

        if (result.Errors.Any(x => x.Code == "PasswordMismatch"))
            throw new BadRequestException("The current password is incorrect.");

        throw new BadRequestException(
            "Unable to change the password.",
            result.Errors.Select(x => x.Description));
    }
}
