using Microsoft.AspNetCore.Mvc;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Queries.GetAllProducts;

public sealed partial class ProductsController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<GetAllProductsDto>>>> GetAll(
        [FromQuery] GetAllProductsQuery query, CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(query, ct);
            return OkPaged(result, $"Found {result.TotalCount} product(s).");
        }
        catch (Exception ex)
        {
            return NotFound("The APi returned exception.");
        }
    }
}