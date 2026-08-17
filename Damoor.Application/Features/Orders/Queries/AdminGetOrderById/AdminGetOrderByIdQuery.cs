using Damoor.Application.Features.Orders.Models;
using MediatR;

namespace Damoor.Application.Features.Orders.Queries.AdminGetOrderById;

public sealed record AdminGetOrderByIdQuery(int Id) : IRequest<AdminOrderDetailsResult>;
