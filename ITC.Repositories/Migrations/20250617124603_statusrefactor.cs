using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class statusrefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_SelectedInterpreterId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SelectedInterpreterId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsPaidToInterpreter",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelectedInterpreterId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "JobApplications",
                newName: "ApplicationStatus");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAt",
                table: "JobApplications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletionOffsetMinutes",
                table: "JobApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IndividualFee",
                table: "JobApplications",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndividualResultFileUrl",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "JobApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaidAt",
                table: "JobApplications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartedAt",
                table: "JobApplications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkStatus",
                table: "JobApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CompletionOffsetMinutes",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IndividualFee",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IndividualResultFileUrl",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "WorkStatus",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "ApplicationStatus",
                table: "JobApplications",
                newName: "Status");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidToInterpreter",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedInterpreterId",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SelectedInterpreterId",
                table: "Jobs",
                column: "SelectedInterpreterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_SelectedInterpreterId",
                table: "Jobs",
                column: "SelectedInterpreterId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
