using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Account.Models;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Damoor.Application.Features.Account.Commands.UpdateMe;

public sealed class UpdateMeHandler
    : IRequestHandler<UpdateMeCommand, AccountResult>
{
    private readonly UserManager<AppUser> _userManager;

    public UpdateMeHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AccountResult> Handle(
        UpdateMeCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            throw new NotFoundException("User", request.UserId);

        user.FullName = request.FullName.Trim();

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new BadRequestException(
                "Unable to update the account.",
                updateResult.Errors.Select(x => x.Description));
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new AccountResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Roles = roles.ToArray()
        };
    }
}
