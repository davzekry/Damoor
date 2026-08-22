using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Checkout.Commands.Checkout;
using Damoor.Application.Features.Orders.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Checkout;

public sealed partial class CheckoutController
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<OrderDetailsResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<OrderDetailsResult>>> Create(
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        [FromBody] CheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CheckoutCommand(
                sessionToken,
                User.GetUserId(),
                request.ShippingAddress,
                request.CustomerName,
                request.WhatsAppNumber,
                request.BackupPhoneNumber,
                request.Notes),
            cancellationToken);

        return CreatedResponse(result, "Order placed successfully.");
    }
}

public sealed record CheckoutRequest(
    string ShippingAddress,
    string CustomerName,
    string WhatsAppNumber,
    string? BackupPhoneNumber,
    string? Notes);
