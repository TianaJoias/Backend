using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Products.Migrations.SqlLite
{
    public partial class Barcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Variants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Variants",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Variants");
        }
    }
}
