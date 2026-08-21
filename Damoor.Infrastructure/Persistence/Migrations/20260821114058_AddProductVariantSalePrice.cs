using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Damoor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantSalePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ProductVariants_SalePrice_Valid",
                table: "ProductVariants",
                sql: "[SalePrice] IS NULL OR ([SalePrice] >= 0 AND [SalePrice] <= [Price])");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ProductVariants_SalePrice_Valid",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "ProductVariants");
        }
    }
}
