using FluentValidation;

namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public sealed class GetAllProductsValidator : AbstractValidator<GetAllProductsQuery>
{
    private static readonly string[] AllowedSortFields =
        ["name", "price", "createdat"];

    public GetAllProductsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must not exceed 100 characters.")
            .When(x => x.Search is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue ||
                       !x.MaxPrice.HasValue ||
                       x.MinPrice <= x.MaxPrice)
            .WithMessage("MinPrice must be less than or equal to MaxPrice.");

        RuleFor(x => x.Size)
            .MaximumLength(32)
            .When(x => x.Size is not null);

        RuleFor(x => x.Color)
            .MaximumLength(64)
            .When(x => x.Color is not null);

        RuleFor(x => x.SortBy)
            .Must(s => AllowedSortFields.Contains(s.ToLowerInvariant()))
            .WithMessage(
                $"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
    }
}
