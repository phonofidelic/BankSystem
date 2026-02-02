using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.AlterTable(
                name: "Transactions")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TransactionHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Transactions")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "Transactions")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AlterTable(
                name: "Transactions")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "TransactionHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Transactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISOSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
