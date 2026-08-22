# Damoor — Architecture & API Reference

This document is the detailed, implementation-level companion to [README.md](README.md). Where the README explains *what the product does*, this file explains *how the code is organized and where to make a change*. Read this before refactoring anything — it is meant to make every part of the system locatable without re-deriving it from scratch.

For the required workflow and construction pattern to follow when adding or modifying anything described here, see [AGENTS.md](AGENTS.md).

---

## 1. Layers and project responsibilities

Clean Architecture, four projects, dependency direction flows inward:

```
Damoor.API  ──▶  Damoor.Application  ──▶  Damoor.Domain
     │                                         ▲
     └──────────▶  Damoor.Infrastructure ──────┘
```

| Project | Contains | Depends on |
| --- | --- | --- |
| `Damoor.API` | Controllers (partial classes, one file per action), Program.cs, middleware, filters, auth/versioning/rate-limit/Swagger extensions, `JwtAccessTokenService` | Application, Infrastructure |
| `Damoor.Application` | MediatR commands/queries/handlers, FluentValidation validators, result/DTO models, pipeline behaviours, `ApiResponse<T>` / `PaginatedList<T>` | Domain, Infrastructure (for `DamoorDbContext`, `UserManager<AppUser>`, `ICacheService`) |
| `Damoor.Domain` | Entities, enums, base entity abstractions (`BaseEntity`, `ISoftDeletable`, `INonDeletable`) | Nothing (no project references) |
| `Damoor.Infrastructure` | EF Core `DbContext`, entity configurations, migrations, ASP.NET Identity setup, Redis cache service, local file service | Domain |

**Note on the Application layer:** there is no `Admin` subfolder or namespace segment anywhere in `Damoor.Application`. Admin-only behavior is entirely an API-layer concern — a command like `CreateCategoryCommand` is used by `AdminCategoriesController` under `[Authorize(Policy = PolicyNames.AdminOnly)]`, but the command itself doesn't know it's "admin." Keep new admin operations in the same feature folder as their public counterparts unless the shape of the data genuinely differs (see `Orders`, which has both customer-facing and `Admin*`-prefixed models/handlers side by side because admins see fields customers must not).

---

## 2. Construction pattern (what every feature slice looks like)

Every vertical slice in `Damoor.Application/Features/<Feature>/...` follows this shape:

```
Features/<Feature>/
├── Commands/<Verb><Entity>/
│   ├── <Verb><Entity>Command.cs      (sealed record : IRequest<TResult> or IRequest)
│   ├── <Verb><Entity>Handler.cs      (sealed class : IRequestHandler<TCommand, TResult>)
│   ├── <Verb><Entity>Result.cs       (only if the result is unique to this command)
│   └── Validator.cs                  (sealed class Validator : AbstractValidator<TCommand>)
├── Queries/<Get...>/                 (same shape, IRequest<TResult>)
├── Models/                           (result DTOs shared across 2+ handlers in the feature)
└── Common/                           (internal static helper classes shared across handlers —
                                        e.g. CartAccessor, OrderAccessor, WishlistAccessor, ReviewMapper)
```

Rules that are load-bearing (don't deviate without a reason):

- Handlers talk to `DamoorDbContext` directly. **There is no repository layer** — the DbContext *is* the unit of work. Don't introduce `IRepository<T>` abstractions.
- `Common/` helper classes are declared `internal static` — this works across feature folders because C# `internal` is assembly-scoped, not namespace-scoped. `CheckoutHandler` (in `Features.Checkout`) legitimately calls `CartAccessor` (in `Features.Carts.Common`) this way.
- Queries use `.AsNoTracking()`. Commands that mutate load tracked entities (no `.AsNoTracking()`).
- Every command/query has its own `Validator` (FluentValidation), registered automatically via `services.AddValidatorsFromAssembly(...)` in `Damoor.Application/DependencyInjection.cs` — no manual registration needed, so the file just needs to exist with a public `AbstractValidator<T>` type.
- Validation errors flow through `ValidationBehaviour<TRequest,TResponse>` (a MediatR pipeline behavior) → thrown `FluentValidation.ValidationException` → caught by `ExceptionHandlingMiddleware` → `400` with `ApiResponse<object>.Fail(...)`.
- Domain exceptions (`NotFoundException`, `ConflictException`, `BadRequestException`, `UnauthorizedException` — all in `Damoor.Application/Common/Exceptions`) are thrown directly from handlers and mapped to status codes by the same middleware (404 / 409 / 400 / 401 respectively).
- API layer: one controller class per resource, declared `sealed partial`, holding only the constructor + `ISender _sender`. Every action lives in **its own file** under a `Commands/<Action>/` or `Queries/<Action>/` subfolder, as a second `partial` declaration of the same controller class. Request body shapes that don't map 1:1 to the command go in that same action file as a `public sealed record <Action>Request(...)`.
- Controllers never talk to `DamoorDbContext`, `UserManager`, or any service directly — only `ISender.Send(...)`.
- Responses always go through `ApiBaseController` helpers: `OkResponse`, `OkPaged`, `CreatedResponse`, `NoContentResponse` — all wrap `ApiResponse<T>`.

### The `[AllowAnonymous]`-can't-be-overridden gotcha

`[AllowAnonymous]` on a controller **disables auth checks for every action in that controller**, even one individually marked `[Authorize]` — ASP.NET Core's `AllowAnonymousFilter` short-circuits the whole authorization pipeline regardless of attribute order or specificity. This project hits that boundary twice, and both times the fix was the same: split into a second controller mapped to the *same route prefix* (different HTTP verb, so no routing conflict):

- `ProductsController` (`[AllowAnonymous]`, public GET endpoints incl. `GET /Products/{id}/reviews`) vs. `ProductReviewsController` (`[Authorize]`, `POST /Products/{productId}/reviews`) — both resolve to `api/v1/Products/{id}/reviews`.
- `AuthController` (`[AllowAnonymous]`, sign-up/sign-in) vs. `AuthAccountController` (`[Authorize]`, `POST /Auth/change-password`) — both resolve under `api/v1/Auth`.

If a future endpoint needs to mix anonymous and authenticated actions under one resource name, split the controller the same way rather than fighting the attribute.

### Guest/session identity pattern (Cart, Checkout, Orders)

Cart, Checkout, and the customer-facing Orders endpoints are not behind `[Authorize]` — they're `[AllowAnonymous]` and instead resolve identity from **either** a JWT **or** an `X-Shopping-Session` header, so the same storefront code path works for guests and signed-in users:

- `ClaimsPrincipalExtensions.GetUserId()` (`Damoor.API/Extensions`) reads the `sub` claim → `int?`, `null` if unauthenticated.
- `CartAccessor.ResolveCartIdAsync(db, sessionToken, userId, ct)` looks up the `ShoppingSession` by token, checks it hasn't expired, and — if the session is bound to a `UserId` — requires the caller's JWT `sub` to match it (`UnauthorizedException` otherwise). This is what lets a guest cart and a logged-in cart share one code path safely.
- `OrderAccessor.EnsureAccessible(order, userId, sessionToken)` does the equivalent check for orders: authenticated → `order.UserId` must match; guest → `order.SessionToken` must match the header exactly.
- Every Cart/Checkout/Orders command therefore carries both `string? SessionToken` and `int? UserId` fields, even though only one is populated per request.

---

## 3. Domain model

### Entity relationship overview

```
Category ──< Product ──< ProductVariant ──< CartItem >── Cart ── ShoppingSession (1:1)
                │  │                  \──< OrderItem >── Order
                │  └──< ProductImage                        (UserId? XOR SessionToken)
                │  └──< Review >── AppUser
                └──< WishlistItem >── Wishlist ── AppUser (1:1)

AppUser ──< ShoppingSession, Order, Review, Wishlist (1:1)
```

### Base abstractions (`Damoor.Domain/Common`)

| Type | Purpose |
| --- | --- |
| `BaseEntity` | `int Id`, `DateTime CreatedAt`, `DateTime? UpdatedAt` — every entity inherits this. |
| `ISoftDeletable` | `bool IsDeleted`, `DateTime? DeletedAt`. |
| `SoftDeletableEntity : BaseEntity, ISoftDeletable` | Base for entities that soft-delete. |
| `INonDeletable` | Marker interface (no members). Entities implementing it throw `InvalidOperationException` if a delete is attempted. |

**How soft-delete/non-delete actually gets enforced:** `AuditableEntityInterceptor` (`Damoor.Infrastructure/Persistence/Interceptors`) is an EF Core `SaveChangesInterceptor` registered on `DamoorDbContext`. On every `SaveChanges(Async)` it walks `ChangeTracker.Entries<BaseEntity>()`:
- `State == Deleted` + `INonDeletable` → throws.
- `State == Deleted` + `ISoftDeletable` → flips `State` to `Modified`, sets `IsDeleted = true`, `DeletedAt = utcNow` instead of actually deleting.
- `State == Added` → stamps `CreatedAt`, resets soft-delete fields to false/null.
- `State == Modified` → stamps `UpdatedAt`, normalizes `DeletedAt` to match `IsDeleted`.
- `AppUser` (which implements `ISoftDeletable` directly, not via `SoftDeletableEntity`, since it inherits `IdentityUser<int>`) gets the identical treatment in a second loop.

Because of this, **`_db.Whatever.Remove(entity)` is always the correct call** for soft-deletable entities in a handler — never manually set `IsDeleted` yourself, the interceptor does it. `_db.CartItems`/`OrderItem` etc. that don't implement `ISoftDeletable` really do hard-delete on `Remove`.

### Entities

| Entity | Base | Key fields | Notes |
| --- | --- | --- | --- |
| `Category` | `SoftDeletableEntity` | `Name` (unique, filtered `IsDeleted=0`), `Description?` | |
| `Product` | `SoftDeletableEntity` | `Name`, `Description`, `CategoryId` | Has `Variants`, `Images`, `WishlistItems`, `Reviews` |
| `ProductVariant` | `SoftDeletableEntity` | `SKU` (unique, filtered), `Size`, `Color`, `Price`, `StockQuantity` | Unique `(ProductId, Size, Color)` filtered. Check constraints: `Price >= 0`, `StockQuantity >= 0`. Query filter excludes soft-deleted variant **or** soft-deleted parent product. |
| `ProductImage` | `BaseEntity` | `ImageUrl`, `IsMain` | Unique filtered index: at most one `IsMain=1` row per product. Hard-deleted (not soft-deletable). |
| `AppUser` | `IdentityUser<int>, ISoftDeletable` | `FullName`, audit + soft-delete fields | Lives in `Damoor.Infrastructure/Identity`, not `Damoor.Domain`, because it's an Identity type. |
| `ShoppingSession` | `BaseEntity` | `SessionToken` (unique), `UserId?`, `ExpiresAt` | 1:1 with `Cart`. 30-day lifetime, set in `CreateShoppingSessionHandler`. |
| `Cart` | `BaseEntity` | `ShoppingSessionId` (unique FK, 1:1) | |
| `CartItem` | `BaseEntity` | `CartId`, `ProductVariantId`, `Quantity` | Unique `(CartId, ProductVariantId)` — this is what makes "add same variant twice" merge instead of duplicate. Check constraint `Quantity > 0`. |
| `Wishlist` | `BaseEntity` | `UserId` (unique, 1:1) | |
| `WishlistItem` | `BaseEntity` | `WishlistId`, `ProductId` | Unique `(WishlistId, ProductId)`. |
| `Order` | `BaseEntity, INonDeletable` | `UserId?`, `SessionToken?`, `TotalAmount`, `Status` (`OrderStatus`, stored as string), `ShippingAddress` | **Check constraint `CK_Orders_CustomerIdentifier`: exactly one of `UserId`/`SessionToken` must be non-null, never both, never neither.** Never delete an order — status transitions only. |
| `OrderItem` | `BaseEntity, INonDeletable` | `OrderId`, `ProductVariantId?`, `ProductName`, `VariantDescription`, `Quantity`, `UnitPrice` | Snapshot fields (`ProductName`/`VariantDescription`/`UnitPrice`) are copied at checkout time and never re-derived from the live `ProductVariant` — that's the point, so historical orders stay accurate after a price change or variant deletion. `ProductVariantId` is nullable and goes `null`-ish (filtered out by the variant's own query filter) if the variant is later soft-deleted. |
| `Review` | `SoftDeletableEntity` | `ProductId`, `UserId`, `Rating` (1–5, check constraint), `Comment?` (max 2000) | Unique filtered index `(ProductId, UserId)` where `IsDeleted=0` — one active review per user per product, but re-reviewing after a soft-deleted review is allowed. |

### Enums

| Enum | Values | Notes |
| --- | --- | --- |
| `OrderStatus` (`Damoor.Domain/Entities/Enums`) | `Pending=1, Confirmed=2, Processing=3, Shipped=4, Delivered=5, Cancelled=6` | Stored as `nvarchar` via `HasConversion<string>`. Serialized as the string name in JSON (see §6, `JsonStringEnumConverter`). Valid transitions are enforced in code, not the DB — see `UpdateOrderStatusHandler.AllowedTransitions`: `Pending→{Confirmed,Cancelled}`, `Confirmed→{Processing,Cancelled}`, `Processing→{Shipped,Cancelled}`, `Shipped→{Delivered}`, `Delivered`/`Cancelled` terminal. Cancelling from either the customer or admin endpoint restocks all `OrderItem`s whose `ProductVariant` still exists (`OrderAccessor.RestockItems`). |

---

## 4. Infrastructure services

| Service | Interface | Registration | Purpose |
| --- | --- | --- | --- |
| `RedisCacheService` | `ICacheService` (`Get/SetAsync<T>` JSON-based) | `CachingExtensions.AddCaching` — `AddStackExchangeRedisCache` + health check | Only consumer today is `GetAllProductsHandler`, and it's currently **commented out** there (see §7 gotchas). |
| `LocalFileService` | `IFileService` | `DependencyInjection.AddInfrastructure` | Writes uploaded streams under `wwwroot/uploads`. **Not wired to any endpoint** — `AdminProductImagesController`/`CreateProductImageApi` accept an image **URL**, not a file upload. If a real upload endpoint is ever added, this is the service to call. |
| `JwtAccessTokenService` | `IAccessTokenService` | `AuthExtensions.AddJwtAuthentication` | Lives in `Damoor.API/Services` (not Infrastructure) since it needs `JwtSettings` bound from configuration at the API layer. Issues HMAC-SHA256 JWTs: claims are `sub` (user id), `email`, `name` (full name), `jti`, and one `role` claim per assigned role. |
| `UserManager<AppUser>` / `SignInManager<AppUser>` | ASP.NET Identity | `IdentityExtensions.AddIdentityServices` | Used directly by Auth and Account handlers — see §2, no wrapper around it. |
| `AuditableEntityInterceptor` | `SaveChangesInterceptor` | `DatabaseExtensions.AddDatabase` | See §3. |
| `IdentityInitializer` | — | called from `Program.cs` via `app.Services.InitializeIdentityAsync(...)` at startup | Ensures `User`/`Admin` roles exist; optionally seeds an admin account from `AdminSeed:*` config. |

### Identity/auth constants (`Damoor.Infrastructure/Identity/AuthConstants.cs`)

```csharp
RoleNames.User = "User";  RoleNames.Admin = "Admin";  RoleNames.All = [User, Admin];
PolicyNames.AdminOnly = "AdminOnly";   // policy: RequireRole(Admin)
```

JWT validation: `NameClaimType = "name"`, `RoleClaimType = "role"` (set in `AuthExtensions`) — this is why `User.IsInRole(RoleNames.Admin)` and `[Authorize(Policy = PolicyNames.AdminOnly)]` work correctly against tokens issued by `JwtAccessTokenService`.

Password policy (`IdentityExtensions`): min length 5, requires digit + uppercase, no non-alphanumeric requirement. Lockout: 5 failed attempts → 15 minute lockout.

---

## 5. Cross-cutting API infrastructure

| Concern | Where | Behavior |
| --- | --- | --- |
| Response envelope | `Damoor.Application/Common/Models/ApiResponse.cs` | `{ success, message, data, pagination?, errors? }` for every response, success or failure. `ApiPaginationMeta.From(PaginatedList<T>)` builds the pagination block. |
| Pagination | `Damoor.Application/Common/Models/PaginatedList.cs` | `Items`, `TotalCount`, `Page`, `PageSize`, computed `TotalPages`/`HasNextPage`/`HasPreviousPage`. Used by `GetAllProducts` and `AdminGetOrders` — nothing else is paginated (Cart, Wishlist, customer Orders list, Reviews list are all plain `List<T>`, deliberately, since per-user collections are expected to stay small). |
| Exception → status mapping | `Damoor.API/Middleware/ExceptionHandlingMiddleware.cs` | `ValidationException`→400, `BadRequestException`→400, `UnauthorizedException`→401, `ConflictException`→409, `NotFoundException`→404, anything else→500 (logged). |
| Enum JSON format | `Program.cs` — `AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))` | Applies globally. The only enum currently exposed in any DTO is `OrderStatus`; it serializes as `"Pending"` etc., not `1`. If you add another enum to a response DTO, it inherits this for free — no per-DTO attribute needed. |
| API versioning | `Damoor.API/Extensions/VersioningExtensions.cs` | URL-segment versioning, default `1.0`. Every controller carries `[ApiVersion("1.0")]` and `api/v{version:apiVersion}/...` in its route. |
| Rate limiting | `Damoor.API/Extensions/RateLimitingExtensions.cs` | Named policies `"fixed"` (10 req/10s) and `"strict"` (5 req/min sliding window, keyed by remote IP). Only `sign-up`/`sign-in` currently apply `[EnableRateLimiting("strict")]`. |
| Swagger | `SwaggerExtensions` + `HttpMethodOperationFilter` | Dev-only. The operation filter stamps expected error status codes (400/401/403/404) onto every operation purely from the HTTP verb — it does not inspect `[Authorize]`, so Swagger shows 401 on GETs even for anonymous endpoints. Don't read too much into that when reviewing generated docs. |
| Idempotency | `Damoor.API/Filters/IdempotencyFilter.cs` | Registered (`AddScoped<IdempotencyFilter>()`) but **not applied to any action** — dead code path unless something opts in. |
| Structured logging | `LoggingBehaviour<TRequest,TResponse>` (MediatR pipeline) | Logs request name/payload/elapsed time for every command/query. Serilog packages are referenced but registration + `UseSerilogRequestLogging()` are commented out in `Program.cs` — the app currently uses the built-in `ILogger` only. |
| Current-user extraction | `Damoor.API/Extensions/ClaimsPrincipalExtensions.GetUserId()` | Parses the JWT `sub` claim to `int?`. Used everywhere a controller needs the caller's id; `!.Value` is used on endpoints behind `[Authorize]` (framework guarantees the claim is present for any token this service itself issued), left nullable and handler-checked on `[AllowAnonymous]` guest/auth-mixed endpoints. |

---

## 6. Full API surface

All routes are prefixed `api/v1/...` (URL-segment versioning). "Auth" column: `Anon` = `[AllowAnonymous]`, `User` = `[Authorize]` (any authenticated user), `Admin` = `[Authorize(Policy = AdminOnly)]`, `Mixed` = `[AllowAnonymous]` but reads JWT-or-`X-Shopping-Session` internally (see §2).

### Auth — `AuthController` / `AuthAccountController`

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| POST | `/Auth/sign-up` | Anon (`strict` rate limit) | `SignUpCommand` → `SignUpHandler` |
| POST | `/Auth/sign-in` | Anon (`strict` rate limit) | `SignInCommand` → `SignInHandler` |
| POST | `/Auth/change-password` | User | `ChangePasswordCommand` → `ChangePasswordHandler` |

### Account — `AccountController`

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| GET | `/Account/me` | User | `GetMeQuery` → `GetMeHandler` |
| PUT | `/Account/me` | User | `UpdateMeCommand` → `UpdateMeHandler` (FullName only — email/password have their own flows) |

### Categories — `CategoriesController` (public) / `AdminCategoriesController`

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| GET | `/Categories` | Anon | `GetAllCategoriesQuery` |
| GET | `/Categories/{id}` | Anon | `GetCategoryByIdQuery` |
| POST | `/Admin/Categories` | Admin | `CreateCategoryCommand` |
| PUT | `/Admin/Categories/{id}` | Admin | `UpdateCategoryCommand` |
| DELETE | `/Admin/Categories/{id}` | Admin | `DeleteCategoryCommand` (soft delete) |

### Products — `ProductsController` (public) / `AdminProductsController` / `AdminProductVariantsController` / `AdminProductImagesController`

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| GET | `/Products` | Anon | `GetAllProductsQuery` — params: `Page`(≥1,d1), `PageSize`(1-100,d10), `Search`(≤100), `CategoryId`, `MinPrice`, `MaxPrice`, `Size`(≤32), `Color`(≤64), `InStock`, `SortBy`(`name`\|`price`\|`createdat`), `Asc`(d true) |
| GET | `/Products/{id}` | Anon | `GetProductByIdQuery` — includes `AverageRating`, `ReviewCount`, `Images`, `Variants` |
| GET | `/Products/{id}/variants` | Anon | `GetProductVariantsQuery` |
| GET | `/Products/{id}/reviews` | Anon | `GetProductReviewsQuery` |
| POST | `/Admin/Products` | Admin | `CreateProductCommand` |
| PUT | `/Admin/Products/{id}` | Admin | `UpdateProductCommand` |
| DELETE | `/Admin/Products/{id}` | Admin | `DeleteProductCommand` (soft delete) |
| POST | `/Admin/Products/{productId}/variants` | Admin | `CreateProductVariantCommand` — enforces unique SKU, unique `(ProductId,Size,Color)` |
| PUT | `/Admin/ProductVariants/{id}` | Admin | `UpdateProductVariantCommand` |
| DELETE | `/Admin/ProductVariants/{id}` | Admin | `DeleteProductVariantCommand` (soft delete) |
| POST | `/Admin/Products/{productId}/images` | Admin | `CreateProductImageCommand` — image by URL, not upload |
| PUT | `/Admin/ProductImages/{id}/main` | Admin | `SetMainProductImageCommand` |
| DELETE | `/Admin/ProductImages/{id}` | Admin | `DeleteProductImageCommand` (hard delete — `ProductImage` isn't soft-deletable) |

### Reviews — `ProductReviewsController` (create) / `ReviewsController` (update/delete)

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| POST | `/Products/{productId}/reviews` | User | `CreateReviewCommand` — 409 if already reviewed; Rating 1–5 |
| PUT | `/Reviews/{id}` | User, owner only | `UpdateReviewCommand` — 401 if not the review's author |
| DELETE | `/Reviews/{id}` | User, owner **or** Admin | `DeleteReviewCommand(Id, UserId, IsAdmin)` — soft delete |

### Shopping Sessions & Cart — `ShoppingSessionsController` / `CartController`

Both `[AllowAnonymous]`; every Cart action requires the `X-Shopping-Session` header (see §2 "guest/session identity pattern").

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| POST | `/ShoppingSessions` | Mixed | `CreateShoppingSessionCommand` — creates session + its Cart in one insert, 30-day expiry, binds `UserId` if caller is authenticated |
| GET | `/Cart` | Mixed | `GetCartQuery` |
| POST | `/Cart/items` | Mixed | `AddCartItemCommand` — merges quantity if variant already in cart; validates stock, does **not** decrement it |
| PUT | `/Cart/items/{id}` | Mixed | `UpdateCartItemCommand` |
| DELETE | `/Cart/items/{id}` | Mixed | `RemoveCartItemCommand` |
| DELETE | `/Cart` | Mixed | `ClearCartCommand` |

### Checkout & Orders — `CheckoutController` / `OrdersController`

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| POST | `/Checkout` | Mixed | `CheckoutCommand(SessionToken, UserId, ShippingAddress)` — re-checks stock, decrements it, snapshots `OrderItem`s, clears the cart, all in one atomic `SaveChangesAsync` (see §7 for why not an explicit transaction) |
| GET | `/Orders` | Mixed | `GetOrdersQuery` — authenticated → own orders; guest → orders matching `X-Shopping-Session` |
| GET | `/Orders/{id}` | Mixed | `GetOrderByIdQuery` — 401 via `OrderAccessor.EnsureAccessible` if not yours |
| POST | `/Orders/{id}/cancel` | Mixed | `CancelOrderCommand` — only from `Pending`/`Confirmed`; restocks variants |

### Admin Orders — `AdminOrdersController`

| Method | Route | Auth | Handler |
| --- | --- | --- | --- |
| GET | `/Admin/Orders` | Admin | `AdminGetOrdersQuery` — params: `Page`, `PageSize`, `Status?`, `Search?` (matches guest `SessionToken` or user `FullName`/`Email` via `LEFT JOIN Users`), `Asc` (sorts `CreatedAt`, default newest-first) |
| GET | `/Admin/Orders/{id}` | Admin | `AdminGetOrderByIdQuery` — includes customer identity + `ProductVariantId` link |
| PUT | `/Admin/Orders/{id}/status` | Admin | `UpdateOrderStatusCommand` — enforced transition map (§3), restocks on transition to `Cancelled` |

### Wishlist — `WishlistController`

`[Authorize]` (registered users only).

| Method | Route | Handler |
| --- | --- | --- |
| GET | `/Wishlist` | `GetWishlistQuery` |
| POST | `/Wishlist/items` | `AddWishlistItemCommand` — **idempotent** on duplicate add (documented choice, see `AddWishlistItemHandler`) |
| DELETE | `/Wishlist/items/{productId}` | `RemoveWishlistItemCommand` — keyed by `productId`, not the item's own id; 404 if not present |
| DELETE | `/Wishlist` | `ClearWishlistCommand` |

Every Wishlist handler routes through `WishlistAccessor.EnsureWishlistIdAsync`, which gets-or-creates the caller's wishlist — so "wishlist doesn't exist yet" is never a state any handler has to special-case.

### Health

| Method | Route | Auth |
| --- | --- | --- |
| GET | `/health` | Anon — checks EF Core `DbContext` + Redis |

---

## 7. Known gaps / things to check before relying on them

- **Redis caching is wired but inert.** `GetAllProductsHandler` has its cache read/write block fully commented out. If you re-enable it, remember to invalidate on every Admin product/variant/image mutation — nothing currently does.
- **Serilog is referenced but not active.** `builder.AddSerilog()` and `app.UseSerilogRequestLogging()` are commented out in `Program.cs`. Logging currently goes through the built-in `ILogger` (console) only.
- **`IFileService`/`LocalFileService` has no caller.** Product images are created by URL (`CreateProductImageCommand`), not multipart upload.
- **`IdempotencyFilter` is registered but unused.** No action currently carries whatever attribute would opt into it.
- **No transaction is opened explicitly in `CheckoutHandler`**, on purpose: `DatabaseExtensions` enables `sql.EnableRetryOnFailure(3)`, and EF Core throws if you call `Database.BeginTransactionAsync()` without wrapping it in `Database.CreateExecutionStrategy().ExecuteAsync(...)`. Checkout instead tracks Order + OrderItems + stock decrements + cart-item removal in one `DbContext` and commits them via a single `SaveChangesAsync()`, which is already atomic. `SignUpHandler` is the one place that *does* need a real multi-step transaction (user create + role assignment) — look there for the `CreateExecutionStrategy` pattern if you need it elsewhere.
- **No test project exists in the solution.** `dotnet test Damoor.sln` has nothing to run yet.
- **No repository/unit-of-work abstraction.** This is intentional per AGENTS.md ("do not introduce a new architectural style... unless clearly needed"), not an oversight — don't add one for a single feature.
- **Payments, shipping/tracking, address book, coupons, brands, inventory reservation, refresh tokens, forgot/reset password, email confirmation, admin analytics** are all explicitly out of scope until their own entities/migrations are designed (see README's roadmap doc, "Plan 9 / Optional Production Commerce Features").

---

## 8. Quick file-finding index

| I need to... | Look at |
| --- | --- |
| Add a new public read endpoint | Existing `Get*Query`/`Get*Handler` in the closest feature folder for the pattern; controller action goes in that resource's public controller |
| Add a new admin-only mutation | Command/Handler/Validator in the feature folder (no "Admin" folder needed); controller action goes in the `Admin*Controller` |
| Change how guest vs. authenticated identity is resolved | `CartAccessor` (Carts), `OrderAccessor.EnsureAccessible` (Orders), `ClaimsPrincipalExtensions.GetUserId()` (API) |
| Change JWT claims or expiry | `Damoor.API/Services/JwtAccessTokenService.cs`, `Damoor.Infrastructure/Identity/AuthSettings.cs` |
| Change password/lockout rules | `Damoor.Infrastructure/Extensions/IdentityExtensions.cs` |
| Change a DB constraint / index | `Damoor.Infrastructure/Persistence/Configurations/<Entity>Configuration.cs`, then `dotnet ef migrations add ...` |
| Change soft-delete/non-delete behavior globally | `Damoor.Infrastructure/Persistence/Interceptors/AuditableEntityInterceptor.cs` |
| Change the response envelope shape | `Damoor.Application/Common/Models/ApiResponse.cs`, `ApiBaseController` |
| Change exception → HTTP status mapping | `Damoor.API/Middleware/ExceptionHandlingMiddleware.cs` |
| Add a new order status / transition | `Damoor.Domain/Entities/Enums/OrderStatus.cs` (+ check constraint in `OrderConfiguration`) and `UpdateOrderStatusHandler.AllowedTransitions` |
