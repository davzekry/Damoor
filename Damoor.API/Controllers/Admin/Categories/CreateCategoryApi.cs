using Damoor.Application.Common.Models;
using Damoor.Application.Features.Categories.Commands.CreateCategory;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminCategoriesController
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<CreateCategoryResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CreateCategoryResult>>> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedResponse(result, "Category created successfully.");
    }
}
