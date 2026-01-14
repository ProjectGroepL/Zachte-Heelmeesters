using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class TreatmentsSeederAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "990bedf8-14b6-43f2-892c-81a9bad68170");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "ceb337d0-baff-475d-910e-6b954d95c652");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "814dc0d6-917d-46ca-99bf-5f13b734b1fd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "d15d2096-f533-4b90-98b5-96fc460e11a9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "cee812e1-4136-4b86-b4aa-f7e8128ac7f4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "8f083d89-168b-4c2c-b17a-5830ebed2ad8");

            migrationBuilder.InsertData(
                table: "Treatments",
                columns: new[] { "Id", "Description", "Instructions", "Name" },
                values: new object[,]
                {
                    { 1, "Behandeling voor lichamelijke klachten", "Patiënt moet comfortabele kleding dragen", "Fysiotherapie" },
                    { 2, "Behandeling voor mentale gezondheidsproblemen", "Patiënt moet wekelijks sessies bijwonen", "Psychotherapie" },
                    { 3, "Kijkoperatie van de mensicus", "Patiënt moet nuchter zijn", "Artroscopie" },
                    { 4, "Behandeling die zich richt op het verbeteren van de dagelijkse vaardigheden en zelfstandigheid van patiënten", "Patiënt moet wekelijks sessies bijwonen", "Ergotherapie" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Treatments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Treatments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Treatments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Treatments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "feb7ce0f-ada8-4996-bec3-68c0d55c8ffb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "ae133c37-1ce1-4f84-a507-6bc21e7ce012");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "73581610-932b-4826-82bf-517d4cfb3015");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "3cebb487-5bdc-4e0e-a9a5-fb45d125f7dd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "9852fd64-e287-42bd-a266-07780970120b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "ff146f58-71b3-4a13-af0b-a15df1d9b0a5");
        }
    }
}
