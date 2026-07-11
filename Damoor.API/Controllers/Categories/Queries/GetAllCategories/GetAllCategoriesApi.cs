using Damoor.Application.Common.Models;
using Damoor.Application.Features.Categories.Queries.GetAllCategories;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Categories;

public sealed partial class CategoriesController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<List<GetAllCategoriesResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetAllCategoriesResult>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllCategoriesQuery(),
            cancellationToken);

        return OkResponse(result, $"Found {result.Count} category(s).");
    }
}
