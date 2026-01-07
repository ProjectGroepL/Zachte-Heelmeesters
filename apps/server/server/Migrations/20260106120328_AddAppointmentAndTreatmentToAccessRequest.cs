using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentAndTreatmentToAccessRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "AccesssRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TreatmentId",
                table: "AccesssRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "6184ef70-f581-48d2-9d83-cb40290af4ad");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "f28db865-d1ec-406f-a0d0-03290ab8c3ca");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "cabcc39e-a718-4498-9148-57405e54d640");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "ba8605e0-a98c-4402-8d3c-9edb41139a4f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "51cb5d2f-b85c-4524-9073-eef52861dfa1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "1f9ce344-9e06-4c27-9b66-36c59a9361bc");

            migrationBuilder.CreateIndex(
                name: "IX_AccesssRequests_AppointmentId",
                table: "AccesssRequests",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccesssRequests_Appointments_AppointmentId",
                table: "AccesssRequests",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.CreateIndex(
                name: "IX_AccesssRequests_TreatmentId",
                table: "AccesssRequests",
                column: "TreatmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccesssRequests_Treatments_TreatmentId",
                table: "AccesssRequests",
                column: "TreatmentId",
                principalTable: "Treatments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
            migrationBuilder.DropForeignKey(
                name: "FK_AccesssRequests_Appointments_AppointmentId",
                table: "AccesssRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AccesssRequests_Treatments_TreatmentId",
                table: "AccesssRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccesssRequests_AppointmentId",
                table: "AccesssRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccesssRequests_TreatmentId",
                table: "AccesssRequests");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "AccesssRequests");

            migrationBuilder.DropColumn(
                name: "TreatmentId",
                table: "AccesssRequests");
        }
    }
}
