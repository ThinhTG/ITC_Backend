using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class selectedTalent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_SelectedInterpreterId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SelectedInterpreterId",
                table: "Jobs");
        }
    }
}
