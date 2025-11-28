using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class UserGeneralPractitionerRelationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GeneralPractitionerId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GeneralPractitionerId",
                table: "Users",
                column: "GeneralPractitionerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_GeneralPractitionerId",
                table: "Users",
                column: "GeneralPractitionerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_GeneralPractitionerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GeneralPractitionerId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "GeneralPractitionerId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
