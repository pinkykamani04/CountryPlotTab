using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBMG.SCS.Infrastructure.Entities
{
    /// <inheritdoc />
    public partial class UpdateAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "AuditLogs",
                newName: "CreatedOnTime");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "AuditLogs",
                newName: "CreatedOnDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOnTime",
                table: "AuditLogs",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDate",
                table: "AuditLogs",
                newName: "Date");
        }
    }
}
