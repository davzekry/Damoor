using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Damoor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWishlistItemToVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Products_ProductId",
                table: "WishlistItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "WishlistItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_WishlistId_ProductId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_WishlistId_ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_ProductVariants_ProductVariantId",
                table: "WishlistItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_ProductVariants_ProductVariantId",
                table: "WishlistItems");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "WishlistItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_WishlistId_ProductVariantId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_WishlistId_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_ProductVariantId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Products_ProductId",
                table: "WishlistItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
