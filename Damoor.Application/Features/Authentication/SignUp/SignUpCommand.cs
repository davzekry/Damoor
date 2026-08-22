using Damoor.Application.Features.Authentication.Common;
using MediatR;

namespace Damoor.Application.Features.Authentication.SignUp;

public sealed record SignUpCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword) : IRequest<AuthResponse>;
