using Microsoft.Extensions.Logging;
using MediatR;
using Damoor.Infrastructure.Interfaces;
using Damoor.Application.Common.Models;
using Damoor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsHandler
    : IRequestHandler<GetAllProductsQuery, PaginatedList<GetAllProductsDto>>
{
    // ── Fix 1: use the real DbContext name exactly as declared ───────
    private readonly PrStoreDbContext _db;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllProductsHandler> _logger;

    public GetAllProductsHandler(
        PrStoreDbContext db,
        ICacheService cache)
        //ILogger<GetAllProductsHandler> logger)
    {
        _db = db;
        _cache = cache;
        //_logger = logger;
    }

    public async Task<PaginatedList<GetAllProductsDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken ct)
    {
        var isCacheable = string.IsNullOrWhiteSpace(request.Search);
        var cacheKey = $"products:p:{request.Page}" +
                          $":s:{request.PageSize}" +
                          $":sort:{request.SortBy}" +
                          $":asc:{request.Asc}";

        if (isCacheable)
        {
            var cached =
                await _cache.GetAsync<PaginatedList<GetAllProductsDto>>(cacheKey);

            if (cached is not null)
            {
                // ── Fix 2: structured logging — message first,
                //   then one value per {placeholder}, no method groups ─
                //_logger.LogInformation(
                //    "Cache HIT for key {CacheKey} — returning {Count} products",
                //    cacheKey,           // ← value for {CacheKey}
                //    cached.Items.Count);// ← value for {Count}

                return cached;
            }
        }

        // ── Fix 2 (same rule): three placeholders → three values ─────
        //_logger.LogInformation(
        //    "Cache MISS — hitting database. Page={Page} PageSize={PageSize} Search={Search}",
        //    request.Page,               // ← {Page}
        //    request.PageSize,           // ← {PageSize}
        //    request.Search ?? "(none)");// ← {Search}

        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p =>
                p.Name.Contains(request.Search) ||
                p.Description.Contains(request.Search));

        query = (request.SortBy.ToLower(), request.Asc) switch
        {
            ("price", true) => query.OrderBy(p => p.Price),
            ("price", false) => query.OrderByDescending(p => p.Price),
            ("createdat", true) => query.OrderBy(p => p.CreatedAt),
            ("createdat", false) => query.OrderByDescending(p => p.CreatedAt),
            (_, true) => query.OrderBy(p => p.Name),
            (_, false) => query.OrderByDescending(p => p.Name),
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new GetAllProductsDto
            {
                CategoryName = p.Category.Name,
                Description = p.Description,
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            })
            .ToListAsync(ct);

        var result = new PaginatedList<GetAllProductsDto>(
            items, totalCount, request.Page, request.PageSize);

        if (isCacheable)
        {
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            _logger.LogInformation(
                "Cached {Count} products under key {CacheKey}",
                items.Count,
                cacheKey);
        }

        return result;
    }
}