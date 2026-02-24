using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequiredConstraintsFromBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId",
                table: "BankAccounts");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "BankAccounts",
                newName: "CustomerAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_BankAccounts_CustomerId",
                table: "BankAccounts",
                newName: "IX_BankAccounts_CustomerAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Customers_CustomerAccountId",
                table: "BankAccounts",
                column: "CustomerAccountId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Customers_CustomerAccountId",
                table: "BankAccounts");

            migrationBuilder.RenameColumn(
                name: "CustomerAccountId",
                table: "BankAccounts",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_BankAccounts_CustomerAccountId",
                table: "BankAccounts",
                newName: "IX_BankAccounts_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId",
                table: "BankAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
