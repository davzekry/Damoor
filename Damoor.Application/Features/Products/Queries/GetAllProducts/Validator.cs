using FluentValidation;

namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsValidator : AbstractValidator<GetAllProductsQuery>
{
    private static readonly string[] AllowedSortFields =
        ["name", "brand", "price", "createdat"];

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

        RuleFor(x => x.SortBy)
            .Must(s => AllowedSortFields.Contains(s.ToLower()))
            .WithMessage(
                $"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
    }
}