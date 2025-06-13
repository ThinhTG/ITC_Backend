using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class BPDVCertificate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslatorCertificate_Users_ApplicationUserId",
                table: "TranslatorCertificate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslatorCertificate",
                table: "TranslatorCertificate");

            migrationBuilder.RenameTable(
                name: "TranslatorCertificate",
                newName: "TranslatorCertificates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslatorCertificates",
                table: "TranslatorCertificates",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslatorCertificates_Users_ApplicationUserId",
                table: "TranslatorCertificates",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslatorCertificates_Users_ApplicationUserId",
                table: "TranslatorCertificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslatorCertificates",
                table: "TranslatorCertificates");

            migrationBuilder.RenameTable(
                name: "TranslatorCertificates",
                newName: "TranslatorCertificate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslatorCertificate",
                table: "TranslatorCertificate",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslatorCertificate_Users_ApplicationUserId",
                table: "TranslatorCertificate",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
