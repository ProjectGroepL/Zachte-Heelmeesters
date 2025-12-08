using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class RevertedTreatmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "2078fd50-2034-4e6e-83b3-c697e4e6d467");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "fd1f5e6f-f69c-4d36-9290-a7294658f078");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "59c356ed-e187-4803-b1b6-b2518d6deb6f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "1fd4d8fd-a84e-42d3-8aa6-a2119119155d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "08bff71d-9660-4931-8e93-25d890bdbd8d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "894d29c3-8b65-459e-9a8a-1cdc562e00c1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "bf85a925-4682-4c55-8773-a952e2837a08");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "f433a10e-de23-4052-8c92-2bf224a846c6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "92dd4992-0170-4200-8fb8-4178e5c6f01a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "d52a7110-ce74-4e0f-a70a-4d81d645d705");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "b8940ba8-853e-43e6-aa8c-18b66c60f7c9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "8a39ee30-7aaa-4b9c-9878-dc8802087273");
        }
    }
}
