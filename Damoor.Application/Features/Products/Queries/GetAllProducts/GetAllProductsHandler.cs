using Damoor.Application.Common.Models;
using Damoor.Infrastructure.Interfaces;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public sealed class GetAllProductsHandler
    : IRequestHandler<GetAllProductsQuery, PaginatedList<GetAllProductsResult>>
{
    private readonly DamoorDbContext _db;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllProductsHandler> _logger;

    public GetAllProductsHandler(
        DamoorDbContext db,
        ICacheService cache,
        ILogger<GetAllProductsHandler> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PaginatedList<GetAllProductsResult>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var size = request.Size?.Trim();
        var color = request.Color?.Trim();
        var sortBy = request.SortBy.ToLowerInvariant();
        var isCacheable = string.IsNullOrWhiteSpace(search);
        var cacheKey = BuildCacheKey(request, search, size, color, sortBy);

        //if (isCacheable)
        //{
        //    var cached =
        //        await _cache.GetAsync<PaginatedList<GetAllProductsResult>>(cacheKey);

        //    if (cached is not null)
        //    {
        //        _logger.LogInformation(
        //            "Cache hit for product list key {CacheKey}.",
        //            cacheKey);

        //        return cached;
        //    }
        //}

        var query = _db.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (request.MinPrice.HasValue ||
            request.MaxPrice.HasValue ||
            !string.IsNullOrWhiteSpace(size) ||
            !string.IsNullOrWhiteSpace(color) ||
            request.InStock.HasValue)
        {
            query = query.Where(p => p.Variants.Any(v =>
                (!request.MinPrice.HasValue || v.Price >= request.MinPrice.Value) &&
                (!request.MaxPrice.HasValue || v.Price <= request.MaxPrice.Value) &&
                (string.IsNullOrWhiteSpace(size) || v.Size == size) &&
                (string.IsNullOrWhiteSpace(color) || v.Color == color) &&
                (!request.InStock.HasValue ||
                    (request.InStock.Value
                        ? v.StockQuantity > 0
                        : v.StockQuantity == 0))));
        }

        query = (sortBy, request.Asc) switch
        {
            ("price", true) => query
                .OrderBy(p => p.Variants.Select(v => (decimal?)v.Price).Min())
                .ThenBy(p => p.Id),
            ("price", false) => query
                .OrderByDescending(p => p.Variants.Select(v => (decimal?)v.Price).Min())
                .ThenBy(p => p.Id),
            ("createdat", true) => query
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id),
            ("createdat", false) => query
                .OrderByDescending(p => p.CreatedAt)
                .ThenBy(p => p.Id),
            (_, true) => query
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id),
            (_, false) => query
                .OrderByDescending(p => p.Name)
                .ThenBy(p => p.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new GetAllProductsResult
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                MainImageUrl = p.Images
                    .Where(i => i.ProductVariantId == null)
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.Id)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),
                MinPrice = p.Variants
                    .Select(v => (decimal?)v.Price)
                    .Min(),
                TotalStockQuantity = p.Variants
                    .Select(v => (int?)v.StockQuantity)
                    .Sum() ?? 0
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedList<GetAllProductsResult>(
            items,
            totalCount,
            request.Page,
            request.PageSize);

        //if (isCacheable)
        //{
        //    await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        //    _logger.LogInformation(
        //        "Cached {Count} products under key {CacheKey}.",
        //        items.Count,
        //        cacheKey);
        //}

        return result;
    }

    private static string BuildCacheKey(
        GetAllProductsQuery request,
        string? search,
        string? size,
        string? color,
        string sortBy)
        => "products" +
           $":p:{request.Page}" +
           $":s:{request.PageSize}" +
           $":search:{search}" +
           $":category:{request.CategoryId}" +
           $":min:{request.MinPrice}" +
           $":max:{request.MaxPrice}" +
           $":size:{size}" +
           $":color:{color}" +
           $":stock:{request.InStock}" +
           $":sort:{sortBy}" +
           $":asc:{request.Asc}";
}
