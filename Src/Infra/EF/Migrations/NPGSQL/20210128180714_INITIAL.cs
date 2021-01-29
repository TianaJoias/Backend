using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.EF.Migrations.NPGSQL
{
    public partial class INITIAL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    User_Password = table.Column<string>(type: "text", nullable: true),
                    User_Email = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address_CEP = table.Column<string>(type: "text", nullable: true),
                    Address_Line1 = table.Column<string>(type: "text", nullable: true),
                    Address_Line2 = table.Column<string>(type: "text", nullable: true),
                    Address_State = table.Column<string>(type: "text", nullable: true),
                    Address_City = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EAN",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastCode = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAN", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentyQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: true),
                    EAN = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SKU = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalWithdrawal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProviders_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LotSupplier",
                columns: table => new
                {
                    LotsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuppliersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotSupplier", x => new { x.LotsId, x.SuppliersId });
                    table.ForeignKey(
                        name: "FK_LotSupplier_Lots_LotsId",
                        column: x => x.LotsId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LotSupplier_Suppliers_SuppliersId",
                        column: x => x.SuppliersId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => new { x.CategoriesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_Categories_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Categories_Tags_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ItemsQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ItemsAddedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    SoldQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    ValuedAt = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentCatalogId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agents_Catalogs_CurrentCatalogId",
                        column: x => x.CurrentCatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    InitialQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantitySold = table.Column<decimal>(type: "numeric", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: true),
                    EAN = table.Column<string>(type: "text", nullable: true),
                    LongDescription = table.Column<string>(type: "text", nullable: true),
                    ValueSold = table.Column<decimal>(type: "numeric", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CatalogId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CATALOG_ITEM",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_CurrentCatalogId",
                table: "Agents",
                column: "CurrentCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogId",
                table: "CatalogItems",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_AgentId",
                table: "Catalogs",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ProductsId",
                table: "Categories",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProviders_AccountId",
                table: "IdentityProviders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LotSupplier_SuppliersId",
                table: "LotSupplier",
                column: "SuppliersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Agents_AgentId",
                table: "Catalogs",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Catalogs_CurrentCatalogId",
                table: "Agents");

            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "EAN");

            migrationBuilder.DropTable(
                name: "IdentityProviders");

            migrationBuilder.DropTable(
                name: "LotSupplier");

            migrationBuilder.DropTable(
                name: "ProductStock");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "Agents");
        }
    }
}
