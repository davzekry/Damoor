using MediatR;

namespace Damoor.Application.Features.Authentication.ChangePassword;

public sealed record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;
