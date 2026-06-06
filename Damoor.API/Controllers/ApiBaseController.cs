using Microsoft.AspNetCore.Mvc;
using Damoor.Application.Common.Models;

namespace Damoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesErrorResponseType(typeof(ApiResponse<object>))]
public abstract class ApiBaseController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResponse<T>(
        T data, string message = "Success")
        => Ok(ApiResponse<T>.Ok(data, message));

    protected ActionResult<ApiResponse<List<T>>> OkPaged<T>(
        PaginatedList<T> paged, string message = "Success")
        => Ok(ApiResponse<List<T>>.OkPaged(
            paged.Items,
            ApiPaginationMeta.From(paged),
            message));

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(
        T data, string message = "Created successfully.")
        => StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<T>.Ok(data, message));

    protected ActionResult<ApiResponse<object>> NoContentResponse(
        string message = "Operation completed.")
        => Ok(ApiResponse<object>.Ok(null!, message));
}