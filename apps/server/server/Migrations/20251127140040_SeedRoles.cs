using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "194bf3f6-dd00-4076-af0f-69c5f7aa1b5d", "Patiënt die gebruik maakt van het systeem voor medische zorg en behandelingen", "Patient", "PATIENT" },
                    { 2, "25f8f186-6489-401c-9ebf-22b89649967d", "Medisch specialist die gespecialiseerde zorg verleent in een specifiek vakgebied", "Specialist", "SPECIALIST" },
                    { 3, "a1fe9b11-ba4d-49b2-b378-2572166c4b4c", "Huisarts die eerste lijn zorg verleent en patiënten doorverwijst naar specialisten", "Huisarts", "HUISARTS" },
                    { 4, "8b8a07bd-6324-400e-a2e1-128f529f2ea4", "Medewerker van zorgverzekeraar die verantwoordelijk is voor vergoedingen en polisbeheer", "Zorgverzekeraar", "ZORGVERZEKERAAR" },
                    { 5, "0946d713-bd28-4758-80b6-5d955fcb875f", "Systeembeheerder met volledige toegang tot alle functionaliteiten en gebruikersbeheer", "Systeembeheerder", "SYSTEEMBEHEERDER" },
                    { 6, "9c8f21b4-1376-4c1b-9c13-8dc6a4cff9e7", "Administratief medewerker in ziekenhuis die ondersteuning biedt bij balieservice en patiëntenzorg", "Administratie", "ADMINISTRATIE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
