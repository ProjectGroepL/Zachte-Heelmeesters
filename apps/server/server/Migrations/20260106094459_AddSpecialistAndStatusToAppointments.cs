using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialistAndStatusToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // 🔴 ADD COLUMNS (DIT ONTBRAK)
    migrationBuilder.AddColumn<int>(
        name: "SpecialistId",
        table: "Appointments",
        type: "int",
        nullable: false,
        defaultValue: 0);

    migrationBuilder.AddColumn<int>(
        name: "Status",
        table: "Appointments",
        type: "int",
        nullable: false,
        defaultValue: 1);

    // 🔴 INDEX
    migrationBuilder.CreateIndex(
        name: "IX_Appointments_SpecialistId",
        table: "Appointments",
        column: "SpecialistId");

    // 🔴 FK NAAR AspNetUsers (User)
    migrationBuilder.AddForeignKey(
        name: "FK_Appointments_AspNetUsers_SpecialistId",
        table: "Appointments",
        column: "SpecialistId",
        principalTable: "AspNetUsers",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);

    // ✅ EXISTING ROLE UPDATES (LATEN STAAN)
    migrationBuilder.UpdateData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: 1,
        column: "ConcurrencyStamp",
        value: "89b6272a-54d9-4867-97ca-528b0a0d5548");

    migrationBuilder.UpdateData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: 2,
        column: "ConcurrencyStamp",
        value: "c59ca908-b8c8-4012-8475-49799335373e");

    migrationBuilder.UpdateData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: 3,
        column: "ConcurrencyStamp",
        value: "2186b16c-9f9d-4a27-9149-a7971f4b95b0");

    migrationBuilder.UpdateData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: 4,
        column: "ConcurrencyStamp",
        value: "9e560e23-c297-45e0-a564-02a4d813b2aa");

    migrationBuilder.UpdateData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: 5,
        column: "ConcurrencyStamp",
        value: "605d46ef-9dbe-48d4-a5d5-7bdb09bdba80");

    migrationBuilder.UpdateData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: 6,
        column: "ConcurrencyStamp",
        value: "eab007b5-ccdc-4555-b2de-0f47846b3c03");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_SpecialistId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SpecialistId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SpecialistId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appointments");
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "358dcb96-cf04-4266-99e4-31afea3b725b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "0433dbe3-a089-4ce6-ba12-75e6e234ca5f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "b750cd9e-1d84-4b78-be82-71cf15a02bbc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "c386ffb9-4100-4de6-9df7-13582505b01b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "2565768d-02f2-4d35-9674-6485e8e71744");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "0f85a77f-ad9e-47eb-b3d2-1c8734ae2868");
        }
    }
}
