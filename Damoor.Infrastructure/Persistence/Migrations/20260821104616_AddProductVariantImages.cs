using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Damoor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId_Main",
                table: "ProductImages");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_Main",
                table: "ProductImages",
                column: "ProductId",
                unique: true,
                filter: "[IsMain] = 1 AND [ProductVariantId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductVariantId_Active",
                table: "ProductImages",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductVariantId_Main",
                table: "ProductImages",
                column: "ProductVariantId",
                unique: true,
                filter: "[IsMain] = 1 AND [ProductVariantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductVariants_ProductVariantId",
                table: "ProductImages",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductVariants_ProductVariantId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId_Main",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductVariantId_Active",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductVariantId_Main",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "ProductImages");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_Main",
                table: "ProductImages",
                column: "ProductId",
                unique: true,
                filter: "[IsMain] = 1");
        }
    }
}
