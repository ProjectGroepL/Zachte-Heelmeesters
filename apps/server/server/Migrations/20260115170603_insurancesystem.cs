using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class insurancesystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppointmentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InsuranceInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentReportId = table.Column<int>(type: "int", nullable: false),
                    InsurerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoveredAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceInvoices_AppointmentReports_AppointmentReportId",
                        column: x => x.AppointmentReportId,
                        principalTable: "AppointmentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "096328ea-06d8-407e-b074-f3958ceefdda");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "286288d3-186d-4a45-a41d-5d44edf77be2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "6b6711b8-dbd7-4b47-af00-37f5b30e5da8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "187d30df-ca19-470f-9658-c04cbec7cbca");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "60071534-06b1-400a-97d2-1c814f5b30cc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "991fe37c-8b35-4214-9723-d1041bbc6830");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceInvoices_AppointmentReportId",
                table: "InsuranceInvoices",
                column: "AppointmentReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceInvoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppointmentReports");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "59c3e36c-1c63-4fc5-b2f9-3015cf6ec36c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "452925ac-bb69-45cd-892a-4ea6543733b7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "746dd3b6-dae5-478e-a63a-2ed6cf50ccd6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "47d53239-23c4-4b44-afe2-2de973e862a2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "c7637870-a546-481b-8068-a8df91d4fe71");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "90dc0f8a-369b-48dd-bcf6-45e369993f49");
        }
    }
}
