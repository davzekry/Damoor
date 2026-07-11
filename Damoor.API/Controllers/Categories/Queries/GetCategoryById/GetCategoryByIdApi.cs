using Damoor.Application.Common.Models;
using Damoor.Application.Features.Categories.Queries.GetCategoryById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Categories;

public sealed partial class CategoriesController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<GetCategoryByIdResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GetCategoryByIdResult>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoryByIdQuery(id),
            cancellationToken);

        return OkResponse(result);
    }
}
