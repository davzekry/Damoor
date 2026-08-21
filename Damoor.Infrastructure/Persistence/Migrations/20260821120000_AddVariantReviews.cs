using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Damoor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProductId_UserId",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId_UserId",
                table: "Reviews",
                columns: new[] { "ProductId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [ProductVariantId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductVariantId",
                table: "Reviews",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductVariantId_UserId",
                table: "Reviews",
                columns: new[] { "ProductVariantId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [ProductVariantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ProductVariants_ProductVariantId",
                table: "Reviews",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ProductVariants_ProductVariantId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProductId_UserId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProductVariantId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProductVariantId_UserId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId_UserId",
                table: "Reviews",
                columns: new[] { "ProductId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
