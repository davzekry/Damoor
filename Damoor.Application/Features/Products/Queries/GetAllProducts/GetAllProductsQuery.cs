using Damoor.Application.Common.Models;
using MediatR;

namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    int? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Size = null,
    string? Color = null,
    bool? InStock = null,
    string SortBy = "name",
    bool Asc = true) : IRequest<PaginatedList<GetAllProductsResult>>;
