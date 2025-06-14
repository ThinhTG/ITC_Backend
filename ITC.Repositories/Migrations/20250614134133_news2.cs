using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class news2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslatorCertificates_ApplicationUserId",
                table: "TranslatorCertificates");

            migrationBuilder.CreateIndex(
                name: "IX_TranslatorCertificates_ApplicationUserId",
                table: "TranslatorCertificates",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslatorCertificates_ApplicationUserId",
                table: "TranslatorCertificates");

            migrationBuilder.CreateIndex(
                name: "IX_TranslatorCertificates_ApplicationUserId",
                table: "TranslatorCertificates",
                column: "ApplicationUserId",
                unique: true);
        }
    }
}
