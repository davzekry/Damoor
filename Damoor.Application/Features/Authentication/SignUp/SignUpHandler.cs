using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Authentication.Common;
using Damoor.Infrastructure.Identity;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Authentication.SignUp;

public sealed class SignUpHandler
    : IRequestHandler<SignUpCommand, AuthResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DamoorDbContext _dbContext;
    private readonly IAccessTokenService _accessTokenService;

    public SignUpHandler(
        UserManager<AppUser> userManager,
        DamoorDbContext dbContext,
        IAccessTokenService accessTokenService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _accessTokenService = accessTokenService;
    }

    public async Task<AuthResponse> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        AppUser? user = null;
        var executionStrategy =
            _dbContext.Database.CreateExecutionStrategy();

        try
        {
            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(
                        cancellationToken);

                user = new AppUser
                {
                    FullName = request.FullName.Trim(),
                    Email = email,
                    UserName = email,
                    PhoneNumber = request.PhoneNumber.Trim()
                };

                var createResult = await _userManager.CreateAsync(
                    user,
                    request.Password);

                if (!createResult.Succeeded)
                    ThrowIdentityFailure(createResult);

                var roleResult = await _userManager.AddToRoleAsync(
                    user,
                    RoleNames.User);

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Unable to assign the default user role: " +
                        string.Join(
                            "; ",
                            roleResult.Errors.Select(x => x.Description)));
                }

                await transaction.CommitAsync(cancellationToken);
            });
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException
            {
                Number: 2601 or 2627
            })
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        if (user is null)
        {
            throw new InvalidOperationException(
                "The account could not be created.");
        }

        var roles = new[] { RoleNames.User };
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

    private static void ThrowIdentityFailure(IdentityResult result)
    {
        if (result.Errors.Any(x =>
                x.Code is "DuplicateEmail" or "DuplicateUserName"))
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        throw new BadRequestException(
            "Unable to create the account.",
            result.Errors.Select(x => x.Description));
    }
}
