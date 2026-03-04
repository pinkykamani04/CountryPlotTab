using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBMG.SCS.Infrastructure.Entities
{
    /// <inheritdoc />
    public partial class updateOperativeForSpendLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DailyLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OverrideDailyLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OverrideEndDate",
                table: "Operatives",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverrideMonthlyLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverrideTnxLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverrideWeeklyLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TnxLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyLimit",
                table: "Operatives",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "MonthlyLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "OverrideDailyLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "OverrideEndDate",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "OverrideMonthlyLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "OverrideTnxLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "OverrideWeeklyLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "TnxLimit",
                table: "Operatives");

            migrationBuilder.DropColumn(
                name: "WeeklyLimit",
                table: "Operatives");
        }
    }
}
