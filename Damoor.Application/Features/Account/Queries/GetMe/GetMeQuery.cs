using Damoor.Application.Features.Account.Models;
using MediatR;

namespace Damoor.Application.Features.Account.Queries.GetMe;

public sealed record GetMeQuery(int UserId) : IRequest<AccountResult>;
