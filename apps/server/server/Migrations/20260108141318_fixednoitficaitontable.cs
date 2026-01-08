using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    public partial class fixednoitficaitontable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // STAP 1: Voeg de kolom eerst handmatig toe, want hij ontbreekt blijkbaar
            // We gebruiken 'nullable: true' zodat oude notificaties niet crashen
            migrationBuilder.AddColumn<int>(
                name: "AccessRequestId",
                table: "Notifications",
                type: "int",
                nullable: true);

            // STAP 2: Update de rollen (standaard EF gedrag)
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "94ea324e-0d32-4853-94e1-8be96e1d8fb9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "c4cc015a-d6bc-413b-a6ed-8f9d2f814017");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "fed1b08b-0cc9-493c-b0f3-0568af651bd1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "ce828c89-4f87-4d31-b19a-dadcfefad98b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "ebfad2f2-284c-46ed-b693-b372a93d6cc8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "fb341aaf-3064-4143-9d63-53c789840de4");

            // STAP 3: Leg nu pas de verbinding (Foreign Key) naar de tabel met 3 s-en
            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AccesssRequests_AccessRequestId",
                table: "Notifications",
                column: "AccessRequestId",
                principalTable: "AccesssRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AccesssRequests_AccessRequestId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AccessRequestId",
                table: "Notifications");
        }
    }
}