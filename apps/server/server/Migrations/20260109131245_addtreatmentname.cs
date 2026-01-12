using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class addtreatmentname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Voeg Name toe aan Treatments
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Treatments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            // 2️⃣ (optioneel maar netjes) bestaande rows vullen
            migrationBuilder.Sql(@"
                UPDATE Treatments
                SET Name = Description
                WHERE Name = '';
            ");

            // 3️⃣ bestaande Role seed updates (laat deze staan)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Kolom netjes verwijderen
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Treatments"
            );

            // Role stamps terugzetten
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "a9fd1c4d-3d5f-42f7-a463-8e4026fc5661");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "7f9d9c45-a9ce-4ca4-bce9-d35bbbc7e16a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "4da1db70-47bc-472c-b769-daa1bcaf189e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "b8073c21-a58b-4be0-aa5d-915f3ae37c97");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "db6f6415-a002-4fec-907f-71f95731cbda");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "a5f2385c-56b1-4817-a2cc-d97b759809e9");
        }
    }
}
