using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.EntityFramework.Migrations.SqlLite
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BarCode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CostValue = table.Column<decimal>(nullable: false),
                    SalePrice = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    Weight = table.Column<decimal>(nullable: true),
                    Supplier = table.Column<Guid>(nullable: false),
                    Typologies = table.Column<string>(nullable: true),
                    Colors = table.Column<string>(nullable: true),
                    Categories = table.Column<string>(nullable: true),
                    Thematics = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
