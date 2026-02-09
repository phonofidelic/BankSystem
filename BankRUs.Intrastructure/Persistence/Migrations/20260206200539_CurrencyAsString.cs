using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SEK_kr_Svensk krona_Swedish Krona",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SEK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SEK",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SEK_kr_Svensk krona_Swedish Krona");
        }
    }
}
