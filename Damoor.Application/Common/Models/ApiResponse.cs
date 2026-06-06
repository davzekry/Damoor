namespace Damoor.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public ApiPaginationMeta? Pagination { get; init; }
        public IEnumerable<string>? Errors { get; init; }

        // ── Static factories ────────────────────────────────────────────

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> OkPaged(
            T data,
            ApiPaginationMeta pagination,
            string message = "Success")
            => new() { Success = true, Message = message, Data = data, Pagination = pagination };

        public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }

    public class ApiPaginationMeta
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }

        public static ApiPaginationMeta From<T>(PaginatedList<T> list)
            => new()
            {
                Page = list.Page,
                PageSize = list.PageSize,
                TotalCount = list.TotalCount,
                TotalPages = list.TotalPages,
                HasNextPage = list.HasNextPage,
                HasPreviousPage = list.HasPreviousPage
            };
    }
}