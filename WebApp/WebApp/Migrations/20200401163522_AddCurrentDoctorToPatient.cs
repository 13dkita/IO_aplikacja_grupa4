using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class AddCurrentDoctorToPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrenctDoctorId",
                table: "Patient",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_CurrenctDoctorId",
                table: "Patient",
                column: "CurrenctDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_User_CurrenctDoctorId",
                table: "Patient",
                column: "CurrenctDoctorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_User_CurrenctDoctorId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_CurrenctDoctorId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "CurrenctDoctorId",
                table: "Patient");
        }
    }
}
