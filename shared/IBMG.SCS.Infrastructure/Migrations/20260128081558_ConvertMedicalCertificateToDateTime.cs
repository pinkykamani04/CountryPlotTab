using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBMG.SCS.Infrastructure.Entities
{
    /// <inheritdoc />
    public partial class ConvertMedicalCertificateToDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MedicalCertificate",
                table: "PilotInformation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MedicalCertificate",
                table: "PilotInformation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
