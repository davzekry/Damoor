using Damoor.Application.Common.Models;
using Damoor.Application.Features.Categories.Commands.UpdateCategory;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminCategoriesController
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<UpdateCategoryResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UpdateCategoryResult>>> Update(
        int id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateCategoryCommand(id, request.Name, request.Description),
            cancellationToken);

        return OkResponse(result, "Category updated successfully.");
    }
}

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description);
