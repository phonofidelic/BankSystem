using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Currency_EnglishName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Placeholder Currency");

            migrationBuilder.AddColumn<string>(
                name: "Currency_ISOSymbol",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "DEF");

            migrationBuilder.AddColumn<string>(
                name: "Currency_NativeName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Placeholder Currency");

            migrationBuilder.AddColumn<string>(
                name: "Currency_Symbol",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "def");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency_EnglishName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Currency_ISOSymbol",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Currency_NativeName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Currency_Symbol",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
