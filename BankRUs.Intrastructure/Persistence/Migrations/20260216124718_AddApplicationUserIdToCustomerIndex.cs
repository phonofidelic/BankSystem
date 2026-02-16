using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserIdToCustomerIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_SocialSecurityNumber",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ApplicationUserId_SocialSecurityNumber",
                table: "Customers",
                columns: new[] { "ApplicationUserId", "SocialSecurityNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_ApplicationUserId_SocialSecurityNumber",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_SocialSecurityNumber",
                table: "Customers",
                column: "SocialSecurityNumber",
                unique: true);
        }
    }
}
