using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Account.Models;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Damoor.Application.Features.Account.Queries.GetMe;

public sealed class GetMeHandler
    : IRequestHandler<GetMeQuery, AccountResult>
{
    private readonly UserManager<AppUser> _userManager;

    public GetMeHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AccountResult> Handle(
        GetMeQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            throw new NotFoundException("User", request.UserId);

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
