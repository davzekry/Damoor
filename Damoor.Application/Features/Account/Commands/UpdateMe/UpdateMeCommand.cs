using Damoor.Application.Features.Account.Models;
using MediatR;

namespace Damoor.Application.Features.Account.Commands.UpdateMe;

public sealed record UpdateMeCommand(
    int UserId,
    string FullName) : IRequest<AccountResult>;
