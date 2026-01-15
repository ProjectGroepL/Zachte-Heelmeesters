using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Treatments",
                columns: new[] { "Id", "Description", "Instructions", "Name" },
                values: new object[,]
                {
                    { 1, "Behandeling gericht op het verbeteren van de mobiliteit en het verminderen van pijn door middel van oefeningen en therapieën.", null, "Fysiotherapie" },
                    { 2, "Gesprekstherapie gericht op het behandelen van mentale gezondheidsproblemen zoals depressie, angst en trauma.", null, "Psychotherapie" },
                    { 3, "Medische ingreep waarbij operaties worden uitgevoerd om aandoeningen te behandelen of te verhelpen.", null, "Chirurgie" }
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
