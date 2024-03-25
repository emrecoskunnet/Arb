using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Arb.Product.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.CreateTable(
                name: "IntegrationEventLogs",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeName = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    TimesSent = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLogs", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "MarketPlaces",
                schema: "Product",
                columns: table => new
                {
                    MarketPlaceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketPlaceName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPlaces", x => x.MarketPlaceId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductCodeType = table.Column<int>(type: "integer", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "MarketPlaceCategories",
                schema: "Product",
                columns: table => new
                {
                    MarketPlaceId = table.Column<int>(type: "integer", nullable: false),
                    MarketPlaceCategoryId = table.Column<int>(type: "integer", nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    MarketPlaceParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    MarketPlaceMainCategoryId = table.Column<int>(type: "integer", nullable: true),
                    Leaf = table.Column<bool>(type: "boolean", nullable: false),
                    HierarchyPath = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPlaceCategories", x => new { x.MarketPlaceId, x.MarketPlaceCategoryId });
                    table.ForeignKey(
                        name: "FK_MarketPlaceCategories_MarketPlaceCategories_MarketPlaceId_M~",
                        columns: x => new { x.MarketPlaceId, x.MarketPlaceParentCategoryId },
                        principalSchema: "Product",
                        principalTable: "MarketPlaceCategories",
                        principalColumns: new[] { "MarketPlaceId", "MarketPlaceCategoryId" });
                    table.ForeignKey(
                        name: "FK_MarketPlaceCategories_MarketPlaces_MarketPlaceId",
                        column: x => x.MarketPlaceId,
                        principalSchema: "Product",
                        principalTable: "MarketPlaces",
                        principalColumn: "MarketPlaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketPlaceMerchants",
                schema: "Product",
                columns: table => new
                {
                    MarketPlaceId = table.Column<int>(type: "integer", nullable: false),
                    MarketPlaceMerchantId = table.Column<int>(type: "integer", nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FrontStoreLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPlaceMerchants", x => new { x.MarketPlaceId, x.MarketPlaceMerchantId });
                    table.ForeignKey(
                        name: "FK_MarketPlaceMerchants_MarketPlaces_MarketPlaceId",
                        column: x => x.MarketPlaceId,
                        principalSchema: "Product",
                        principalTable: "MarketPlaces",
                        principalColumn: "MarketPlaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketPlaceProducts",
                schema: "Product",
                columns: table => new
                {
                    MarketPlaceId = table.Column<int>(type: "integer", nullable: false),
                    MarketPlaceProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ProductLink = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ProductImageLink = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    MarketPlaceCategoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPlaceProducts", x => new { x.MarketPlaceId, x.MarketPlaceProductId });
                    table.ForeignKey(
                        name: "FK_MarketPlaceProducts_MarketPlaces_MarketPlaceId",
                        column: x => x.MarketPlaceId,
                        principalSchema: "Product",
                        principalTable: "MarketPlaces",
                        principalColumn: "MarketPlaceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketPlaceProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketPlaceProductSales",
                schema: "Product",
                columns: table => new
                {
                    MarketPlaceProductSaleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketPlaceId = table.Column<int>(type: "integer", nullable: false),
                    MarketPlaceProductId = table.Column<int>(type: "integer", nullable: false),
                    CategorySalesRank = table.Column<int>(type: "integer", nullable: false),
                    MainCategorySalesRank = table.Column<int>(type: "integer", nullable: false),
                    BuyBox = table.Column<decimal>(type: "numeric", nullable: false),
                    PeriodSalesCount = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPlaceProductSales", x => x.MarketPlaceProductSaleId);
                    table.ForeignKey(
                        name: "FK_MarketPlaceProductSales_MarketPlaceProducts_MarketPlaceId_M~",
                        columns: x => new { x.MarketPlaceId, x.MarketPlaceProductId },
                        principalSchema: "Product",
                        principalTable: "MarketPlaceProducts",
                        principalColumns: new[] { "MarketPlaceId", "MarketPlaceProductId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketPlaceProductSaleMerchants",
                schema: "Product",
                columns: table => new
                {
                    MarketPlaceProductSaleMerchantId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketPlaceId = table.Column<int>(type: "integer", nullable: false),
                    MarketPlaceProductSaleId = table.Column<int>(type: "integer", nullable: false),
                    MarketPlaceMerchantId = table.Column<int>(type: "integer", nullable: false),
                    BuyBox = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: true),
                    MerchantTotalSaleCount = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPlaceProductSaleMerchants", x => x.MarketPlaceProductSaleMerchantId);
                    table.ForeignKey(
                        name: "FK_MarketPlaceProductSaleMerchants_MarketPlaceMerchants_Market~",
                        columns: x => new { x.MarketPlaceId, x.MarketPlaceMerchantId },
                        principalSchema: "Product",
                        principalTable: "MarketPlaceMerchants",
                        principalColumns: new[] { "MarketPlaceId", "MarketPlaceMerchantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketPlaceProductSaleMerchants_MarketPlaceProductSales_Mar~",
                        column: x => x.MarketPlaceProductSaleId,
                        principalSchema: "Product",
                        principalTable: "MarketPlaceProductSales",
                        principalColumn: "MarketPlaceProductSaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketPlaceCategories_MarketPlaceId_MarketPlaceParentCatego~",
                schema: "Product",
                table: "MarketPlaceCategories",
                columns: new[] { "MarketPlaceId", "MarketPlaceParentCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketPlaceProducts_ProductId",
                schema: "Product",
                table: "MarketPlaceProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketPlaceProductSaleMerchants_MarketPlaceId_MarketPlaceMe~",
                schema: "Product",
                table: "MarketPlaceProductSaleMerchants",
                columns: new[] { "MarketPlaceId", "MarketPlaceMerchantId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketPlaceProductSaleMerchants_MarketPlaceProductSaleId",
                schema: "Product",
                table: "MarketPlaceProductSaleMerchants",
                column: "MarketPlaceProductSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketPlaceProductSales_MarketPlaceId_MarketPlaceProductId",
                schema: "Product",
                table: "MarketPlaceProductSales",
                columns: new[] { "MarketPlaceId", "MarketPlaceProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCodeType_ProductCode",
                schema: "Product",
                table: "Products",
                columns: new[] { "ProductCodeType", "ProductCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLogs");

            migrationBuilder.DropTable(
                name: "MarketPlaceCategories",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "MarketPlaceProductSaleMerchants",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "MarketPlaceMerchants",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "MarketPlaceProductSales",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "MarketPlaceProducts",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "MarketPlaces",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Product");
        }
    }
}
