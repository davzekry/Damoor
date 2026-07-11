using Damoor.Application.Common.Models;
using Damoor.Application.Features.Authentication.Common;
using MediatR;

namespace Damoor.Application.Features.Authentication.SignIn;

public sealed record SignInCommand(
    string Email,
    string Password) : IRequest<ApiResponse<AuthResponse>>;
