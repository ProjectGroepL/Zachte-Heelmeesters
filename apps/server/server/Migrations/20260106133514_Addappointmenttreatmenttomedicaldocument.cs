using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class Addappointmenttreatmenttomedicaldocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Columns
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "MedicalDocuments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TreatmentId",
                table: "MedicalDocuments",
                nullable: true);

            // Indexes
            migrationBuilder.CreateIndex(
                name: "IX_MedicalDocuments_AppointmentId",
                table: "MedicalDocuments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDocuments_TreatmentId",
                table: "MedicalDocuments",
                column: "TreatmentId");

           // Foreign keys — NO CASCADE PATHS
            migrationBuilder.AddForeignKey(
                name: "FK_MedicalDocuments_Appointments_AppointmentId",
                table: "MedicalDocuments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalDocuments_Treatments_TreatmentId",
                table: "MedicalDocuments",
                column: "TreatmentId",
                principalTable: "Treatments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "ec75eb0e-a0fe-4f31-ad9e-be6dcf5f8d9d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "4587a958-edea-4d51-a408-8bce2602a3c7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "ff398d93-786b-4806-a06d-5b24c97cc11a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "43349188-25e2-4df7-a3c4-84719440903d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "7390a58b-738e-4dc3-91b7-5605568b8220");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "76b01a5d-ca31-4cf3-80ee-80b321fbd653");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalDocuments_Appointments_AppointmentId",
                table: "MedicalDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalDocuments_Treatments_TreatmentId",
                table: "MedicalDocuments");

            migrationBuilder.DropIndex(
                name: "IX_MedicalDocuments_AppointmentId",
                table: "MedicalDocuments");

            migrationBuilder.DropIndex(
                name: "IX_MedicalDocuments_TreatmentId",
                table: "MedicalDocuments");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "MedicalDocuments");

            migrationBuilder.DropColumn(
                name: "TreatmentId",
                table: "MedicalDocuments");
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
        }
    }
}
