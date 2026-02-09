using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBankAccountCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "Currency_EnglishName",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Placeholder Currency");

            migrationBuilder.AddColumn<string>(
                name: "Currency_ISOSymbol",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "DEF");

            migrationBuilder.AddColumn<string>(
                name: "Currency_NativeName",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Placeholder Currency");

            migrationBuilder.AddColumn<string>(
                name: "Currency_Symbol",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "def");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency_EnglishName",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Currency_ISOSymbol",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Currency_NativeName",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Currency_Symbol",
                table: "BankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SEK_kr_Svensk krona_Swedish Krona");
        }
    }
}
