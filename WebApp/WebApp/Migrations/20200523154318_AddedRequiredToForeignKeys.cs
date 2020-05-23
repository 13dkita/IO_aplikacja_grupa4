using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class AddedRequiredToForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedPatients_User_DoctorId",
                table: "SharedPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedPatients_Patient_PatientId",
                table: "SharedPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistory_User_DoctorId",
                table: "TreatmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistory_Patient_PatientId",
                table: "TreatmentHistory");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "TreatmentHistory",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "TreatmentHistory",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "SharedPatients",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "SharedPatients",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPatients_User_DoctorId",
                table: "SharedPatients",
                column: "DoctorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPatients_Patient_PatientId",
                table: "SharedPatients",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistory_User_DoctorId",
                table: "TreatmentHistory",
                column: "DoctorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistory_Patient_PatientId",
                table: "TreatmentHistory",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedPatients_User_DoctorId",
                table: "SharedPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedPatients_Patient_PatientId",
                table: "SharedPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistory_User_DoctorId",
                table: "TreatmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistory_Patient_PatientId",
                table: "TreatmentHistory");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "TreatmentHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "TreatmentHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "SharedPatients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "SharedPatients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPatients_User_DoctorId",
                table: "SharedPatients",
                column: "DoctorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPatients_Patient_PatientId",
                table: "SharedPatients",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistory_User_DoctorId",
                table: "TreatmentHistory",
                column: "DoctorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistory_Patient_PatientId",
                table: "TreatmentHistory",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
